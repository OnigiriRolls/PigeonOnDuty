using UnityEngine;

public class ThrowCalculator : MonoBehaviour
{
    [SerializeField] private Transform throwPoint;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float launchAngle = 0.25f;

    public Vector3 ThrowPosition => throwPoint.position;

    private Vector3 GetLaunchDirection()
    {
        return (throwPoint.forward + throwPoint.up * launchAngle).normalized;
    }

    private Vector3 GetInheritedVelocity()
    {
        float forwardSpeed = Vector3.Dot(playerController.Velocity, throwPoint.forward);
        return throwPoint.forward * Mathf.Max(0f, forwardSpeed);
    }

    public Vector3 GetLaunchVelocity(float throwForce)
    {
        return GetInheritedVelocity() + GetLaunchDirection() * throwForce;
    }
}
