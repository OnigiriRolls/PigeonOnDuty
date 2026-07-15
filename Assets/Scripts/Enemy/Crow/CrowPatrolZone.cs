using UnityEngine;

public class CrowPatrolZone
{
    public Vector3 Center { get; }
    public float Radius { get; }

    private readonly LayerMask obstacleMask;
    private readonly float checkRadius;
    private readonly int maxAttempts;

    public CrowPatrolZone(Vector3 center, float radius, LayerMask obstacleMask, float checkRadius = 5f, int maxAttempts = 10)
    {
        Center = center;
        Radius = radius;
        this.obstacleMask = obstacleMask;
        this.checkRadius = checkRadius;
        this.maxAttempts = maxAttempts;
    }

    public Vector3 GetRandomPoint()
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 offset = Random.insideUnitCircle * Radius;
            Vector3 point = Center + new Vector3(offset.x, 0f, offset.y);
            if (!Physics.CheckSphere(point, checkRadius, obstacleMask))
                return point;
        }
        Debug.Log("return center");
        return Center;
    }
}
