using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField] private ThrowTrajectoryPreview trajectoryPreview;
    [SerializeField] private AnimationCurve chargeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private ThrowableInventory inventory;
    [SerializeField] private ThrowCalculator throwCalculator;
    [SerializeField] private FeatherAutoAim autoAim;

    private float currentForce;
    private float chargeTime;
    private bool isCharging;
    private bool ThrowPressed => Input.GetKeyDown(KeyCode.LeftShift);
    private bool ThrowReleased => Input.GetKeyUp(KeyCode.LeftShift);
    private bool CancelPressed => Input.GetKeyDown(KeyCode.Z);
    private bool PreviousPressed => Input.GetKeyDown(KeyCode.R);
    private bool NextPressed => Input.GetKeyDown(KeyCode.E);


    private void Update()
    {
        HandleSelection();
        if (!isCharging)
        {
            if (ThrowPressed)
                StartCharging();

            return;
        }

        UpdateCharge();
        if (ThrowReleased)
            Throw();
        if (CancelPressed)
            ResetThrow();
    }

    private void HandleSelection()
    {
        if (PreviousPressed)
            inventory.SelectPrevious();
        if (NextPressed)
            inventory.SelectNext();
    }

    private void StartCharging()
    {
        isCharging = true;
        ThrowableData item = inventory.SelectedItem;
        if (item == null)
            return;
        if (inventory.GetAmount(item) <= 0)
            return;
        chargeTime = 0f;
        currentForce = item.minForce;
        trajectoryPreview.Show();
        trajectoryPreview.UpdateTrajectory(currentForce);
    }

    private void UpdateCharge()
    {
        ThrowableData item = inventory.SelectedItem;
        if (item == null)
            return;
        chargeTime += Time.deltaTime;
        float t = Mathf.Clamp01(chargeTime / item.chargeDuration);
        currentForce = Mathf.Lerp(item.minForce, item.maxForce, chargeCurve.Evaluate(t));
        trajectoryPreview.UpdateTrajectory(currentForce);
        trajectoryPreview.SetMaxCharge(t >= 0.999f);
    }

    private void Throw()
    {
        ThrowableData item = inventory.SelectedItem;
        if (item == null)
            return;
        if (!inventory.TryConsume(item))
            return;
        ThrowableProjectile projectile = Instantiate(item.projectilePrefab, throwCalculator.ThrowPosition, Quaternion.identity);
        if (projectile is FeatherProjectile feather && autoAim.CurrentTarget != null)
        {
            feather.SetTarget(autoAim.CurrentTarget);
        }
        projectile.Launch(throwCalculator.GetLaunchVelocity(currentForce));
        ResetThrow();
    }

    private void ResetThrow()
    {
        isCharging = false;
        chargeTime = 0f;
        currentForce = 0f;
        trajectoryPreview.Hide();
        trajectoryPreview.SetMaxCharge(false);
        autoAim.ClearTarget();
    }
}
