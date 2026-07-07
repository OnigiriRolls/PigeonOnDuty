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
    [SerializeField] private float minimumDistance = 300f;
    [SerializeField] private MissionTimer missionTimer;

    private GPSMissionState state;
    private GPSMission activeMission;
    private HumanFollower currentHuman;
    private float lostTimer;
    private bool missionRunning;
    private EndlessRunManager endlessRunManager;
    private GameManager gameManager;

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
        missionTimer.OnTimerExpired += FailMission;
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
        endlessRunManager.SetCurrentObjective(currentHuman.transform);
        float duration = endlessRunManager.CalculateTimeForTarget(currentHuman.transform);
        missionTimer.StartTimer(duration);
        hintUI.Show("Find the Human");
        currentHuman.ShowInteractionCircle(Color.yellow);
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
        currentHuman.HideInteractionCircle();
        currentHuman.ShowMessage("Let's go!");
        GPSDestination destination = GetRandomDestination();
        paradeManager.SpawnParades(currentHuman.transform.position, destination.transform.position);
        endlessRunManager.CleanCurrentObjective();
        endlessRunManager.SpawnNextObjectiveAndCollectibles(destination.transform);
        float duration = endlessRunManager.CalculateTimeForTarget(destination.transform);
        missionTimer.StartTimer(duration);
        hintUI.Show("Escort the Human", 4f);
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

    private void FailMission()
    {
        ClearMission();
        Debug.Log("GPS Mission Failed");
        gameManager.GameOver(DeathReason.TimeUp);
    }

    public float GetRemainingLostTime()
    {
        if (activeMission == null)
            return 0f;
        return Mathf.Max(0f, activeMission.lostTime - lostTimer);
    }

    private void ClearMission()
    {
        missionTimer.StopTimer();
        paradeManager.ClearAllParades();
        missionRunning = false;
    }

    private void OnDestroy()
    {
        missionTimer.OnTimerExpired -= FailMission;
    }
}
