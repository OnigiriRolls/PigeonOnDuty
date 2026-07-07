using UnityEngine;

public class EndlessRunManager : MonoBehaviour
{
    public Transform CurrentObjective => currentObjective;

    [SerializeField] private CollectibleManager collectibleManager;
    [SerializeField] private GameObject postCheckpointHigh;
    [SerializeField] private GameObject postCheckpointMid;
    [SerializeField] private GameObject postCheckpointLow;
    [SerializeField] private GameObject gpsCheckpoint;
    [SerializeField] private Transform checkpointParent;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float expectedSpeedKmh = 50f;
    [SerializeField] private float extraTimeBuffer = 10f;
    [SerializeField] private AudioClip lowMusic;

    private Waypoint[] waypoints;
    private Waypoint currentWaypoint;
    private Transform currentObjective;

    private void Start()
    {
        waypoints = FindObjectsByType<Waypoint>();
        AudioManager.Instance.PlayMusic(lowMusic);
        SaveManager.Instance.AddRun();
    }

    public void SpawnNextCheckpointAndCollectibles()
    {
        SpawnNextCheckpoint();
        collectibleManager.SpawnCollectibles(currentObjective, playerTransform.position);
    }

    public void SpawnNextObjectiveAndCollectibles(Transform objectiveTransform)
    {
        SpawnNextObjective(objectiveTransform);
        collectibleManager.SpawnCollectibles(currentObjective, playerTransform.position);
    }

    private void SpawnNextCheckpoint()
    {
        if (waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints found!");
            return;
        }

        if (currentObjective != null)
        {
            Destroy(currentObjective.gameObject);
        }

        currentWaypoint = GetRandomWaypoint();
        GameObject checkpointPrefab = GetPostCheckpointPrefab(currentWaypoint.AltitudeLayer);
        GameObject currentCheckpoint = Instantiate(checkpointPrefab, currentWaypoint.transform.position, GetObjectiveRotation(currentWaypoint.transform.position), checkpointParent);
        currentObjective = currentCheckpoint.transform;
    }

    private void SpawnNextObjective(Transform objectiveTransform)
    {
        if (objectiveTransform == null)
        {
            Debug.LogWarning("No objective found!");
            return;
        }
        if (currentObjective != null)
        {
            Destroy(currentObjective.gameObject);
        }

        GameObject currentCheckpoint = Instantiate(gpsCheckpoint, objectiveTransform.position, objectiveTransform.rotation, checkpointParent);
        currentObjective = currentCheckpoint.transform;
    }

    private GameObject GetPostCheckpointPrefab(AltitudeLayer layer)
    {
        if (layer == AltitudeLayer.High)
            return postCheckpointHigh;
        if (layer == AltitudeLayer.Mid)
            return postCheckpointMid;
        return postCheckpointLow;
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

    private Quaternion GetObjectiveRotation(Vector3 checkpointPosition)
    {
        Vector3 direction = playerTransform.position - checkpointPosition;
        direction.y = 0f;
        if (direction == Vector3.zero)
            return Quaternion.identity;
        return Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f, 0f); ;
    }

    public float CalculateTimeForTarget(Transform target)
    {
        float distance = Vector3.Distance(playerTransform.position, target.position);
        float expectedSpeedMs = expectedSpeedKmh / 3.6f;
        return distance / expectedSpeedMs + extraTimeBuffer;
    }

    public void StartCheckpointTimer(MissionTimer timer, float multiplier = 1f)
    {
        float duration = CalculateTimeForTarget(currentObjective);
        timer.StartTimer(duration, multiplier);
    }

    public void SetCurrentObjective(Transform target)
    {
        currentObjective = target;
    }

    public void CleanCurrentObjective()
    {
        currentObjective = null;
    }
}
