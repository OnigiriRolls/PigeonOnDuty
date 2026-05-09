using System.Collections.Generic;
using UnityEngine;

public class EndlessRunManager : MonoBehaviour
{
    public GameObject CurrentCheckpoint => currentCheckpoint;

    [SerializeField] private GameObject checkpointPrefab;

    private Waypoint[] waypoints;
    private GameObject currentCheckpoint;
    private Waypoint currentWaypoint;

    private void Start()
    {
        waypoints = FindObjectsOfType<Waypoint>();
        SpawnNextCheckpoint();
    }

    public void SpawnNextCheckpoint()
    {
        if (waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints found!");
            return;
        }

        if (currentCheckpoint != null)
        {
            Destroy(currentCheckpoint);
        }

        Waypoint nextWaypoint = GetRandomWaypoint();
        currentWaypoint = nextWaypoint;
        currentCheckpoint = Instantiate(
            checkpointPrefab,
            currentWaypoint.transform.position,
            Quaternion.Euler(0f, 0f, 90f)
        );
        currentCheckpoint
            .GetComponent<Checkpoint>()
            .Initialize(this);
    }

    private Waypoint GetRandomWaypoint()
    {
        Waypoint randomWaypoint;
        do
        {
            randomWaypoint =
                waypoints[Random.Range(0, waypoints.Length)];
        }
        while (randomWaypoint == currentWaypoint);
        return randomWaypoint;
    }
}
