using UnityEngine;

public class EndlessRunManager : MonoBehaviour
{
    public float CheckpointTimer { get; private set; }
    public GameObject CurrentCheckpoint => currentCheckpoint;
    public bool TimerStarted => timerStarted;

    [SerializeField] private DeliveryMissionManager deliveryMissionManager;
    [SerializeField] private CollectibleManager collectibleManager;
    [SerializeField] private GameObject postCheckpointHigh;
    [SerializeField] private GameObject postCheckpointMid;
    [SerializeField] private GameObject postCheckpointLow;
    [SerializeField] private Transform checkpointParent;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float expectedSpeedKmh = 50f;
    [SerializeField] private float extraTimeBuffer = 10f;
    [SerializeField] private float minimumStartSpeed = 5f;
    [SerializeField] private AudioClip lowMusic;

    private Waypoint[] waypoints;
    private GameObject currentCheckpoint;
    private Waypoint currentWaypoint;
    private bool timerStarted;
    private PlayerController playerController;
    private GameManager gameManager;

    private void Start()
    {
        deliveryMissionManager.OnMissionSelected += HandleMissionSelected;
        playerController = playerTransform.GetComponent<PlayerController>();
        waypoints = FindObjectsByType<Waypoint>();
        gameManager = FindAnyObjectByType<GameManager>();
        AudioManager.Instance.PlayMusic(lowMusic);
        SaveManager.Instance.AddRun();
        SpawnNextCheckpointAndCollectibles();
        deliveryMissionManager.RequestMissionSelection();
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
            gameManager.GameOver(DeathReason.TimeUp);
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

    private void HandleMissionSelected(DeliveryMission mission)
    {
        CheckpointTimer *= mission.timerMultiplier;
    }

    private void OnDestroy()
    {
        deliveryMissionManager.OnMissionSelected -= HandleMissionSelected;
    }
}
