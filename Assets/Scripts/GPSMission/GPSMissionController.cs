using UnityEngine;

public class GPSMissionController : MonoBehaviour
{
    public bool IsEscortActive => state == GPSMissionState.EscortTraveler;

    [SerializeField] private HumanFollower humanPrefab;
    [SerializeField] private Transform humanSpawnPoint;
    [SerializeField] private Transform player;
    [SerializeField] private Transform gpsSpawnPointsParent;
    [SerializeField] private float reachTravelerDistance = 5f;
    [SerializeField] private Transform gpsDestinationParent;

    private GPSMissionState state;
    private GPSMission activeMission;
    private HumanFollower currentHuman;
    private float lostTimer;
    private bool missionRunning;
    private EndlessRunManager endlessRunManager;
    private GameManager gameManager;
    private HintUI hintUI;

    public enum GPSMissionState
    {
        Inactive,
        ReachTraveler,
        EscortTraveler
    }

    private void Start()
    {
        endlessRunManager = FindAnyObjectByType<EndlessRunManager>();
        gameManager = FindAnyObjectByType<GameManager>();
        hintUI = FindAnyObjectByType<HintUI>();
    }

    public void StartMission(GPSMission mission)
    {
        activeMission = mission;
        if (currentHuman != null)
            Destroy(currentHuman.gameObject);

        Transform spawnPoint = GetClosestSpawnPoint();
        currentHuman = Instantiate(humanPrefab, spawnPoint.position, Quaternion.identity);
        lostTimer = 0f;
        missionRunning = true;
        state = GPSMissionState.ReachTraveler;
        endlessRunManager.SetCurrentObjectiveAndTimer(currentHuman.transform);
        hintUI.Show("Find the Human");
        Debug.Log("Reach the traveler");
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
        state = GPSMissionState.EscortTraveler;
        currentHuman.Initialize(player);
        currentHuman.ShowMessage("Let's go!");
        GPSDestination destination = GetRandomDestination();
        endlessRunManager.CleanCurrentObjective();
        endlessRunManager.SpawnNextObjectiveAndCollectibles(destination.transform);
        hintUI.Show("Escort the Human", 4f);
        Debug.Log("Traveler is now following");
    }

    private GPSDestination GetRandomDestination()
    {
        GPSDestination[] destinations = gpsDestinationParent.GetComponentsInChildren<GPSDestination>();
        return destinations[Random.Range(0, destinations.Length)];
    }

    public void StopMission()
    {
        missionRunning = false;
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
        float distance = Vector3.Distance(player.position, currentHuman.transform.position);
        if (distance <= reachTravelerDistance)
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

    private void FailMission()
    {
        missionRunning = false;
        Debug.Log("GPS Mission Failed");
        gameManager.GameOver(DeathReason.TimeUp);
    }

    public float GetRemainingLostTime()
    {
        if (activeMission == null)
            return 0f;
        return Mathf.Max(0f, activeMission.lostTime - lostTimer);
    }

    public float GetDistanceToHuman()
    {
        if (currentHuman == null)
            return 0f;
        return Vector3.Distance(player.position, currentHuman.transform.position);
    }
}
