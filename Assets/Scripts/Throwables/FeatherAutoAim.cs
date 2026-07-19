using UnityEngine;
using System;

public class FeatherAutoAim : MonoBehaviour
{
    public StealerCrowController CurrentTarget => currentTarget;

    [SerializeField] private ThrowTrajectoryPreview preview;
    [SerializeField] private LayerMask crowLayer;
    [SerializeField] private float autoAimRadius = 2.5f;

    private StealerCrowController currentTarget;

    private void Update()
    {
        if (!preview.IsVisible)
            return;
        UpdateTarget();
    }

    private void UpdateTarget()
    {
        StealerCrowController bestTarget = null;
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
            Collider[] hits = Physics.OverlapCapsule(a, b, autoAimRadius, crowLayer);
            foreach (Collider hit in hits)
            {
                if (!hit.TryGetComponent(out StealerCrowController crow))
                    continue;
                float d = Vector3.Distance(a, crow.transform.position);
                if (d < bestDistance)
                {
                    bestDistance = d;
                    bestTarget = crow;
                }
            }
        }

        SetCurrentTarget(bestTarget);
    }

    private void SetCurrentTarget(StealerCrowController newTarget)
    {
        //Debug.Log(newTarget);
        if (currentTarget == newTarget)
            return;
        if (currentTarget != null)
        {
            //Debug.Log("ShowTargetRing(false)");
            currentTarget.ShowTargetRing(false);
        }
        currentTarget = newTarget;
        if (currentTarget != null)
        {
            //Debug.Log("ShowTargetRing(true)");
            currentTarget.ShowTargetRing(true);
        }
    }

    public void ClearTarget()
    {
        SetCurrentTarget(null);
    }
}
