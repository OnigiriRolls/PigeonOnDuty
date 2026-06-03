using UnityEngine;

public class EndlessRunManager : MonoBehaviour
{
    public float CheckpointTimer { get; private set; }
    public GameObject CurrentCheckpoint => currentCheckpoint;

    [SerializeField] private CollectibleManager collectibleManager;
    [SerializeField] private GameObject postCheckpointHigh;
    [SerializeField] private GameObject postCheckpointMid;
    [SerializeField] private GameObject postCheckpointLow;
    [SerializeField] private Transform checkpointParent;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float expectedSpeedKmh = 50f;
    [SerializeField] private float extraTimeBuffer = 10f;
    [SerializeField] private float minimumStartSpeed = 5f;

    private Waypoint[] waypoints;
    private GameObject currentCheckpoint;
    private Waypoint currentWaypoint;
    private bool timerStarted;
    private PlayerController playerController;
    private GameManager gameManager;

    private void Start()
    {
        playerController = playerTransform.GetComponent<PlayerController>();
        waypoints = FindObjectsByType<Waypoint>();
        gameManager = FindAnyObjectByType<GameManager>();
        SpawnNextCheckpointAndCollectibles();
    }

    private void Update()
    {
        if (!timerStarted)
        {
            TryStartTimer();
            return;
        }
        CheckpointTimer -= Time.deltaTime;
        if (CheckpointTimer <= 0f)
        {
            //Debug.Log("Game Over - Time's up!");
            gameManager.GameOver();
        }
    }

    private void TryStartTimer()
    {
        float speed = playerController.Velocity.magnitude;
        if (speed < minimumStartSpeed)
            return;
        timerStarted = true;
    }

    public void SpawnNextCheckpointAndCollectibles()
    {
        SpawnNextCheckpoint();
        collectibleManager.SpawnCollectibles(currentCheckpoint.transform, playerTransform.position);
    }

    private void SpawnNextCheckpoint()
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

        currentWaypoint = GetRandomWaypoint();
        GameObject checkpointPrefab = GetPostCheckpointPrefab(currentWaypoint.AltitudeLayer);
        currentCheckpoint = Instantiate(checkpointPrefab, currentWaypoint.transform.position, GetCheckpointRotation(currentWaypoint.transform.position), checkpointParent);
        currentCheckpoint.GetComponent<Checkpoint>().Initialize(this);
        SetupCheckpointTimer(currentCheckpoint.transform);
    }

    private GameObject GetPostCheckpointPrefab(AltitudeLayer layer)
    {
        if (currentWaypoint.AltitudeLayer == AltitudeLayer.High)
            return postCheckpointHigh;
        if (currentWaypoint.AltitudeLayer == AltitudeLayer.Mid)
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

    private Quaternion GetCheckpointRotation(Vector3 checkpointPosition)
    {
        Vector3 direction = playerTransform.position - checkpointPosition;
        direction.y = 0f;
        if (direction == Vector3.zero)
            return Quaternion.identity;
        return Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f, 0f); ;
    }

    private void SetupCheckpointTimer(Transform checkpoint)
    {
        float distance = Vector3.Distance(playerTransform.position, checkpoint.position);
        float expectedSpeedMs = expectedSpeedKmh / 3.6f;
        CheckpointTimer = distance / expectedSpeedMs + extraTimeBuffer;
    }
}
