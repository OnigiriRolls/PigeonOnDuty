using System.Collections.Generic;
using UnityEngine;

public class WaypointNetwork : MonoBehaviour
{
    public IReadOnlyList<PedestrianWaypoint> Waypoints => waypoints;

    [SerializeField] private List<PedestrianWaypoint> waypoints = new();
    [SerializeField] private Transform waypointsParent;

#if UNITY_EDITOR
    private void OnValidate()
    {
        waypoints.Clear();
        waypoints.AddRange(waypointsParent.GetComponentsInChildren<PedestrianWaypoint>());
    }
#endif

    public PedestrianWaypoint GetRandomWaypoint()
    {
        if (waypoints.Count == 0)
            return null;
        return waypoints[Random.Range(0, waypoints.Count)];
    }
}
