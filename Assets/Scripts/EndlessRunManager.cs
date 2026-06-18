using UnityEngine;

public class EndlessRunManager : MonoBehaviour
{
    public float CheckpointTimer { get; private set; }
    public bool TimerStarted => timerStarted;
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
    [SerializeField] private float minimumStartSpeed = 5f;
    [SerializeField] private AudioClip lowMusic;

    private Waypoint[] waypoints;
    private Waypoint currentWaypoint;
    private bool timerStarted;
    private PlayerController playerController;
    private GameManager gameManager;
    private Transform currentObjective;

    private void Start()
    {
        playerController = playerTransform.GetComponent<PlayerController>();
        waypoints = FindObjectsByType<Waypoint>();
        gameManager = FindAnyObjectByType<GameManager>();
        AudioManager.Instance.PlayMusic(lowMusic);
        SaveManager.Instance.AddRun();
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
        SetupCheckpointTimer(currentObjective);
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
        SetupCheckpointTimer(currentObjective);
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

    private void SetupCheckpointTimer(Transform checkpoint)
    {
        float distance = Vector3.Distance(playerTransform.position, checkpoint.position);
        float expectedSpeedMs = expectedSpeedKmh / 3.6f;
        CheckpointTimer = distance / expectedSpeedMs + extraTimeBuffer;
    }

    private void HandleMissionSelected(MissionData mission)
    {
        if (mission is DeliveryMission deliveryMission)
        {
            CheckpointTimer *= deliveryMission.timerMultiplier;
        }
    }

    public void SetCurrentObjectiveAndTimer(Transform target)
    {
        currentObjective = target;
        SetupCheckpointTimer(target);
    }

    public void CleanCurrentObjective()
    {
        currentObjective = null;
    }
}
