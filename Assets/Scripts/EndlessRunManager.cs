using UnityEngine;

public class EndlessRunManager : MonoBehaviour
{
    public GameObject CurrentCheckpoint => currentCheckpoint;

    [SerializeField] private GameObject buildingCheckpointPrefab;
    [SerializeField] private GameObject postCheckpointPrefab;
    [SerializeField] private CrowManager crowManager;
    [SerializeField] private Transform checkpointParent;

    private Waypoint[] waypoints;
    private GameObject currentCheckpoint;
    private Waypoint currentWaypoint;

    private void Start()
    {
        waypoints = FindObjectsOfType<Waypoint>();
        SpawnNextCheckpoint();
        //crowManager.TrySpawnCrow();
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
        if (currentWaypoint.name.Contains("Post"))
            currentCheckpoint = Instantiate(postCheckpointPrefab, currentWaypoint.transform.position, GetRandomWaypointRotation(), checkpointParent);
        else currentCheckpoint = Instantiate(buildingCheckpointPrefab, currentWaypoint.transform.position, GetRandomWaypointRotation(), checkpointParent);
        Debug.Log(currentCheckpoint.name);
        currentCheckpoint.GetComponent<Checkpoint>().Initialize(this);
    }

    private Waypoint GetRandomWaypoint()
    {
        Waypoint randomWaypoint;
        do
        {
            randomWaypoint = waypoints[Random.Range(0, waypoints.Length)];
        }
        while (randomWaypoint == currentWaypoint);
        return randomWaypoint;
    }

    private Quaternion GetRandomWaypointRotation()
    {
        float randomY = Random.Range(0f, 360f);
        return Quaternion.Euler(0f, randomY, 0f);
    }
}
