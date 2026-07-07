using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField] private ThrowTrajectoryPreview trajectoryPreview;
    [SerializeField] private AnimationCurve chargeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private ThrowableData equippedThrowable;
    [SerializeField] private ThrowCalculator throwCalculator;
    [SerializeField] private PickupManager pickupManager;

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
        isCharging = true;
        chargeTime = 0f;
        currentForce = equippedThrowable.minForce;
        trajectoryPreview.Show();
        trajectoryPreview.UpdateTrajectory(currentForce);
    }

    private void UpdateCharge()
    {
        chargeTime += Time.deltaTime;
        float t = Mathf.Clamp01(chargeTime / equippedThrowable.chargeDuration);
        currentForce = Mathf.Lerp(equippedThrowable.minForce, equippedThrowable.maxForce, chargeCurve.Evaluate(t));
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
        ThrowableProjectile projectile = Instantiate(equippedThrowable.projectilePrefab, throwCalculator.ThrowPosition, Quaternion.identity);
        projectile.Initialize(pickupManager);
        projectile.Launch(throwCalculator.GetLaunchVelocity(currentForce));
    }
}
