using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GPSMissionController : MonoBehaviour
{
    [SerializeField] private HumanFollower humanPrefab;
    [SerializeField] private Transform humanSpawnPoint;
    [SerializeField] private Transform player;
    [SerializeField] private Transform gpsSpawnPointsParent;
    [SerializeField] private Transform gpsDestinationParent;
    [SerializeField] private HintUI hintUI;
    [SerializeField] private ParadeManager paradeManager;
    [SerializeField] private CityParadeManager cityParadeManager;
    [SerializeField] private float minimumDistance = 300f;
    [SerializeField] private float bufferTimePerEnemy = 10f;
    [SerializeField] private MissionTimer missionTimer;
    [SerializeField] private TravelTimeCalculator travelTimeCalculator;
    [SerializeField] private CollectibleManager collectibleManager;
    [SerializeField] private MinimapMissionController minimapController;
    [SerializeField] private GPSEnemyManager dogManager;
    [SerializeField] private ThrowableInventory playerInventory;
    [SerializeField] private ThrowableData featherData;
    [SerializeField] private ThrowableData newspaperData;
    [SerializeField] private FeatherPickupSpawner featherSpawner;

    private GPSMissionState state;
    private GPSMission activeMission;
    private HumanFollower currentHuman;
    private float lostTimer;
    private bool missionRunning;
    private CheckpointsManager checkpointsManager;
    private GPSDestination currentDestination;
    private GameManager gameManager;

    public enum GPSMissionState
    {
        Inactive,
        ReachTraveler,
        EscortTraveler
    }

    private void Start()
    {
        checkpointsManager = FindAnyObjectByType<CheckpointsManager>();
    }

    public void SetGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public void StartMission(GPSMission mission)
    {
        activeMission = mission;
        if (currentHuman != null)
            Destroy(currentHuman.gameObject);

        Transform spawnPoint = GetClosestSpawnPoint();
        currentHuman = Instantiate(humanPrefab, spawnPoint.position, Quaternion.identity);
        currentHuman.OnTrustDepleted += HandleHumanTrustDepleted;
        lostTimer = 0f;
        missionRunning = true;
        state = GPSMissionState.ReachTraveler;
        checkpointsManager.SetCurrentObjective(currentHuman.transform);
        float duration = travelTimeCalculator.CalculateTime(currentHuman.transform, activeMission.timeBuffer);
        missionTimer.StartTimer(duration);
        hintUI.Show("Find the Human");
        currentHuman.ShowInteractionCircle(Color.yellow);
        TutorialManager.Instance.TryShow("tutorial_mission_gps", "Guide the player", "Find the human then guide it to the destination. Don't lose its trust!");
    }

    private void HandleHumanTrustDepleted()
    {
        FailMission(DeathReason.NPCTrustLost);
    }

    private Transform GetClosestSpawnPoint()
    {
        Transform closest = null;
        float bestDistance = float.MaxValue;
        foreach (Transform spawnPoint in gpsSpawnPointsParent)
        {
            float distance = Vector3.Distance(player.position, spawnPoint.position);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                closest = spawnPoint;
            }
        }
        return closest;
    }

    private void StartEscort()
    {
        currentDestination = GetRandomDestination();
        cityParadeManager.Initialize(player.transform, currentDestination.transform.position);
        state = GPSMissionState.EscortTraveler;
        checkpointsManager.ClearCheckpoint();
        ParadeController defaultParade = cityParadeManager.GetRandomParade();
        PlayerInputController playerInputController = player.gameObject.GetComponent<PlayerInputController>();
        currentHuman.Initialize(player, defaultParade, hintUI, playerInputController);
        currentHuman.HideInteractionCircle();
        currentHuman.ShowMessage("Let's go!");
        minimapController.ShowClient(currentHuman.transform);
        dogManager.GenerateEnemies(currentHuman.transform.position, currentDestination.transform.position);
        playerInventory.SetAmount(newspaperData, 0);
        playerInventory.SetAmount(featherData, 3);
        playerInventory.SetEnabled(newspaperData, false);
        playerInventory.SetEnabled(featherData, true);
        playerInventory.Select(featherData);
        //paradeManager.SpawnParades(currentHuman.transform.position, currentDestination.transform.position);
        checkpointsManager.CleanCurrentObjective();
        checkpointsManager.SpawnNextObjective(currentDestination.transform);
        collectibleManager.StartContinuousCoinSpawning(player);
        float buffer = activeMission.timeBuffer + bufferTimePerEnemy * dogManager.EnemyCount;
        float duration = travelTimeCalculator.CalculateTime(currentDestination.transform, buffer, 30f);
        missionTimer.StartTimer(duration);
        featherSpawner.StartSpawn();
        hintUI.Show("Escort the Human", 4f);
        TutorialManager.Instance.TryShow("tutorial_leave_npc", "New Mechanic", "The human follows you now. Use [F] to leave the human behind and scout ahead. Use [F] to resume following.");
    }

    private GPSDestination GetRandomDestination()
    {
        GPSDestination[] destinations = gpsDestinationParent.GetComponentsInChildren<GPSDestination>();
        List<GPSDestination> validDestinations = destinations
            .Where(d => Vector3.Distance(player.position, d.transform.position) >= minimumDistance)
            .ToList();

        if (validDestinations.Count == 0)
        {
            Debug.LogWarning("No valid destinations found.");
            return destinations[Random.Range(0, destinations.Length)];
        }
        return validDestinations[Random.Range(0, validDestinations.Count)];
    }

    public void StopMission()
    {
        ClearMission();
        if (currentHuman != null)
            Destroy(currentHuman.gameObject);
    }

    private void Update()
    {
        if (!missionRunning)
            return;
        if (currentHuman == null)
            return;
        switch (state)
        {
            case GPSMissionState.ReachTraveler:
                UpdateReachTraveler();
                break;

            case GPSMissionState.EscortTraveler:
                UpdateEscortTraveler();
                break;
        }
    }

    private void UpdateReachTraveler()
    {
        if (currentHuman.CanInteract())
        {
            StartEscort();
        }
    }

    private void UpdateEscortTraveler()
    {
        float distance = Vector3.Distance(player.position, currentHuman.transform.position);
        if (distance > activeMission.maxDistance)
        {
            lostTimer += Time.deltaTime;
            if (lostTimer >= activeMission.lostTime)
                FailMission();
        }
        else
        {
            lostTimer = 0f;
        }
    }

    public void FailMission(DeathReason reason = DeathReason.TimeUp)
    {
        ClearMission();
        Debug.Log("GPS Mission Failed");
        gameManager.GameOver(reason);
    }

    public float GetRemainingLostTime()
    {
        if (activeMission == null)
            return 0f;
        return Mathf.Max(0f, activeMission.lostTime - lostTimer);
    }

    private void ClearMission()
    {
        minimapController.HideClient(currentHuman.transform);
        missionTimer.StopTimer();
        //paradeManager.ClearAllParades();
        cityParadeManager.Clear();
        missionRunning = false;
        checkpointsManager.ClearCheckpoint();
        collectibleManager.StopContinuousCoinSpawning();
        dogManager.Clear();
        playerInventory.SetAmount(featherData, 0);
        playerInventory.SetEnabled(featherData, false);
        featherSpawner.StopSpawn();
        if (currentHuman != null)
        {
            currentHuman.OnTrustDepleted -= HandleHumanTrustDepleted;
        }
        WarningManager.Instance.Hide();
    }

    private void OnDisable()
    {
        if (currentHuman != null)
        {
            currentHuman.OnTrustDepleted -= HandleHumanTrustDepleted;
        }
    }
}
