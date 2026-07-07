using UnityEngine;

public class ThrowTrajectoryPreview : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int pointCount = 30;
    [SerializeField] private float timeBetweenPoints = 0.1f;
    [SerializeField] private float gravityMultiplier = 1f;
    [SerializeField] private ThrowCalculator throwCalculator;

    private void Awake()
    {
        lineRenderer.enabled = false;
    }

    public void Show()
    {
        lineRenderer.positionCount = pointCount;
        lineRenderer.enabled = true;
    }

    public void Hide()
    {
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
    }

    public void UpdateTrajectory(float throwForce)
    {
        Vector3 start = throwCalculator.ThrowPosition;
        Vector3 velocity = throwCalculator.GetLaunchVelocity(throwForce);
        for (int i = 0; i < pointCount; i++)
        {
            float t = i * timeBetweenPoints;
            Vector3 gravity = Physics.gravity * gravityMultiplier;
            Vector3 point = start + velocity * t + 0.5f * t * t * gravity;
            lineRenderer.SetPosition(i, point);
        }
    }
}
