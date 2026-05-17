using UnityEngine;

public class WaypointValidator : MonoBehaviour
{
    public LayerMask buildingLayer;
    public LayerMask waypointLayer;
    public float minDistanceBetweenWaypoints = 10f;
    public float buildingCheckRadius = 3f;
    public Vector3 mapMinBounds;
    public Vector3 mapMaxBounds;

    public bool IsValidPositionForBuilding(Vector3 position)
    {
        bool insideBounds =
            position.x >= mapMinBounds.x &&
            position.x <= mapMaxBounds.x &&
            position.y >= mapMinBounds.y &&
            position.y <= mapMaxBounds.y &&
            position.z >= mapMinBounds.z &&
            position.z <= mapMaxBounds.z;
        if (!insideBounds)
            return false;

        return IsValidPositionForPost(position);
    }

    public bool IsValidPositionForPost(Vector3 position)
    {
        bool nearBuilding = Physics.CheckSphere(position, buildingCheckRadius, buildingLayer);
        if (nearBuilding)
            return false;

        bool nearWaypoint = Physics.CheckSphere(position, minDistanceBetweenWaypoints, waypointLayer);
        if (nearWaypoint)
            return false;

        return true;
    }
}
