using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField] private ThrowTrajectoryPreview trajectoryPreview;
    [SerializeField] private AnimationCurve chargeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private ThrowableInventory inventory;
    [SerializeField] private ThrowCalculator throwCalculator;

    private float currentForce;
    private bool isCharging;
    private float chargeTime;
    private bool ThrowPressed => Input.GetKeyDown(KeyCode.LeftShift);
    private bool ThrowReleased => Input.GetKeyUp(KeyCode.LeftShift);

    private void Update()
    {
        HandleInput();
        if (!isCharging)
            return;
        UpdateCharge();
    }

    private void HandleInput()
    {
        if (ThrowPressed)
            StartCharging();
        if (ThrowReleased)
            ReleaseThrow();
    }

    private void StartCharging()
    {
        ThrowableData item = inventory.EquippedItem;
        if (item == null)
            return;
        if (inventory.GetAmount(item) <= 0)
            return;
        isCharging = true;
        chargeTime = 0f;
        currentForce = item.minForce;
        trajectoryPreview.Show();
        trajectoryPreview.UpdateTrajectory(currentForce);
    }

    private void UpdateCharge()
    {
        ThrowableData item = inventory.EquippedItem;
        if (item == null)
            return;
        chargeTime += Time.deltaTime;
        float t = Mathf.Clamp01(chargeTime / item.chargeDuration);
        currentForce = Mathf.Lerp(item.minForce, item.maxForce, chargeCurve.Evaluate(t));
        trajectoryPreview.UpdateTrajectory(currentForce);
    }

    private void ReleaseThrow()
    {
        if (!isCharging)
            return;
        isCharging = false;
        trajectoryPreview.Hide();
        Throw();
    }

    private void Throw()
    {
        ThrowableData item = inventory.EquippedItem;
        if (item == null)
            return;
        if (!inventory.TryConsume(item))
            return;
        ThrowableProjectile projectile = Instantiate(item.projectilePrefab, throwCalculator.ThrowPosition, Quaternion.identity);
        projectile.Launch(throwCalculator.GetLaunchVelocity(currentForce));
    }
}
