using UnityEngine;
using System;

public class FeatherAutoAim : MonoBehaviour
{
    public IThrowTarget CurrentTarget => currentTarget;

    [SerializeField] private ThrowTrajectoryPreview preview;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float autoAimRadius = 2.5f;

    private IThrowTarget currentTarget;

    private void Update()
    {
        if (!preview.IsVisible)
            return;
        UpdateTarget();
    }

    private void UpdateTarget()
    {
        IThrowTarget bestTarget = null;
        float bestDistance = float.MaxValue;
        var points = preview.TrajectoryPoints;
        if (points.Count < 2)
        {
            SetCurrentTarget(null);
            return;
        }

        for (int i = 1; i < points.Count; i++)
        {
            Vector3 a = points[i - 1];
            Vector3 b = points[i];
            Collider[] hits = Physics.OverlapCapsule(a, b, autoAimRadius, targetLayer);
            foreach (Collider hit in hits)
            {
                if (!hit.TryGetComponent(out IThrowTarget target))
                    continue;
                float d = Vector3.Distance(a, target.AimPoint.position);
                if (d < bestDistance)
                {
                    bestDistance = d;
                    bestTarget = target;
                }
            }
        }

        SetCurrentTarget(bestTarget);
    }

    private void SetCurrentTarget(IThrowTarget newTarget)
    {
        if (currentTarget == newTarget)
            return;
        if (currentTarget != null)
        {
            currentTarget.ShowTargetRing(false);
        }
        currentTarget = newTarget;
        if (currentTarget != null)
        {
            currentTarget.ShowTargetRing(true);
        }
    }

    public void ClearTarget()
    {
        SetCurrentTarget(null);
    }
}
