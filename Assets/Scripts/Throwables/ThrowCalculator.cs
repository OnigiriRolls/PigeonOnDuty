using UnityEngine;

public class ThrowCalculator : MonoBehaviour
{
    [SerializeField] private Transform throwPoint;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float launchAngle = 0.25f;
    [SerializeField, Range(0f, 1f)] private float inheritedVelocityMultiplier = 0.5f;

    public Vector3 ThrowPosition => throwPoint.position;

    private Vector3 GetLaunchDirection()
    {
        return (throwPoint.forward + throwPoint.up * launchAngle).normalized;
    }

    private Vector3 GetInheritedVelocity()
    {
        float forwardSpeed = Vector3.Dot(playerController.Velocity, throwPoint.forward);
        forwardSpeed = Mathf.Max(0f, forwardSpeed);
        return inheritedVelocityMultiplier * forwardSpeed * throwPoint.forward;
    }

    public Vector3 GetLaunchVelocity(float throwForce)
    {
        return GetInheritedVelocity() + GetLaunchDirection() * throwForce;
    }

    public Vector3 GetAutoAimVelocity(Transform target, float throwForce)
    {
        Vector3 direction = (target.position - ThrowPosition).normalized;
        return GetInheritedVelocity() + direction * throwForce;
    }
}
