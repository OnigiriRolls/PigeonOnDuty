using UnityEngine;

public class ThrowCalculator : MonoBehaviour
{
    [SerializeField] private Transform throwPoint;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float launchAngle = 0.25f;

    public Vector3 ThrowPosition => throwPoint.position;

    public Vector3 GetLaunchDirection()
    {
        return (throwPoint.forward + throwPoint.up * launchAngle).normalized;
    }

    public Vector3 GetLaunchVelocity(float throwForce)
    {
        return playerController.Velocity + GetLaunchDirection() * throwForce;
    }
}
