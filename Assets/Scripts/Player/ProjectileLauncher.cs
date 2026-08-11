using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField] private ThrowTrajectoryPreview trajectoryPreview;
    [SerializeField] private AnimationCurve chargeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private ThrowableInventory inventory;
    [SerializeField] private ThrowCalculator throwCalculator;
    [SerializeField] private FeatherAutoAim autoAim;
    [SerializeField] private ThrowableData newspaperItem;
    [SerializeField] private AudioClip inventorySound;
    [SerializeField] private PlayerInputController playerInputController;

    private float currentForce;
    private float chargeTime;
    private bool isCharging;

    private void OnEnable()
    {
        if (playerInputController == null)
            return;

        playerInputController.OnThrowPressed += StartCharging;
        playerInputController.OnThrowReleased += HandleThrowReleased;
        playerInputController.OnCancelThrow += ResetThrow;
        playerInputController.OnSelectItem += SelectNext;
    }

    private void HandleThrowReleased()
    {
        if (isCharging)
            Throw();
    }

    private void SelectNext()
    {
        inventory.SelectNext();
    }

    private void Update()
    {
        if (!isCharging)
            return;
        UpdateCharge();
    }

    private void StartCharging()
    {
        ThrowableData item = inventory.SelectedItem;
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
        if (item == null || !inventory.TryConsume(item))
        {
            AudioManager.Instance.PlaySFX(inventorySound);
            return;
        }
        ThrowableProjectile projectile = Instantiate(item.projectilePrefab, throwCalculator.ThrowPosition, Quaternion.identity);
        if (autoAim.CurrentTarget != null)
        {
            projectile.SetTarget(autoAim.CurrentTarget);
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

    public void DropOneNewspaper()
    {
        if (inventory.TryConsume(newspaperItem))
        {
            ThrowableProjectile projectile = Instantiate(newspaperItem.projectilePrefab, transform.position, Quaternion.identity);
            Vector3 velocity = transform.forward * 2f + Vector3.down * 4f;
            projectile.Launch(velocity);
        }
    }

    private void OnDisable()
    {
        if (playerInputController == null)
            return;

        playerInputController.OnThrowPressed -= StartCharging;
        playerInputController.OnThrowReleased -= HandleThrowReleased;
        playerInputController.OnCancelThrow -= ResetThrow;
        playerInputController.OnSelectItem -= SelectNext;
    }
}
