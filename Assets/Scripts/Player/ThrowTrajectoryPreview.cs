using UnityEngine;

public class ThrowTrajectoryPreview : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int pointCount = 30;
    [SerializeField] private float timeBetweenPoints = 0.1f;
    [SerializeField] private float gravityMultiplier = 1f;
    [SerializeField] private ThrowCalculator throwCalculator;
    [SerializeField] private Gradient normalGradient;
    [SerializeField] private Gradient maxChargeGradient;
    [SerializeField] private AudioClip maxChargeClip;
    [SerializeField] private LandingMarker landingMarker;
    [SerializeField] private LayerMask collisionMask;

    private bool maxChargeReached;

    private void Awake()
    {
        lineRenderer.enabled = false;
    }

    public void Show()
    {
        lineRenderer.positionCount = pointCount;
        lineRenderer.enabled = true;
        maxChargeReached = false;
        lineRenderer.colorGradient = normalGradient;
    }

    public void Hide()
    {
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
        landingMarker.Hide();
    }

    public void UpdateTrajectory(float throwForce)
    {
        lineRenderer.positionCount = pointCount;
        Vector3 start = throwCalculator.ThrowPosition;
        Vector3 velocity = throwCalculator.GetLaunchVelocity(throwForce);
        Vector3 previousPoint = start;
        for (int i = 0; i < pointCount; i++)
        {
            float t = i * timeBetweenPoints;
            Vector3 gravity = Physics.gravity * gravityMultiplier;
            Vector3 point = start + velocity * t + 0.5f * t * t * gravity;
            if (Physics.Linecast(previousPoint, point, out RaycastHit hit, collisionMask))
            {
                lineRenderer.positionCount = i + 1;
                lineRenderer.SetPosition(i, hit.point);
                if (hit.normal.y > 0.7f)
                    landingMarker.Show(hit.point);
                else
                    landingMarker.Hide();
                return;
            }
            lineRenderer.SetPosition(i, point);
            previousPoint = point;
        }
        landingMarker.Hide();
        lineRenderer.positionCount = pointCount;
    }

    public void SetMaxCharge(bool reached)
    {
        if (maxChargeReached == reached)
            return;
        maxChargeReached = reached;
        lineRenderer.colorGradient = reached ? maxChargeGradient : normalGradient;
        if (reached)
            AudioManager.Instance.PlaySFX(maxChargeClip);
    }
}
