using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CheckpointsManager : MonoBehaviour
{
    public Transform CurrentObjective => currentObjective;
    public event Action<Transform> OnCheckpointReached;

    [SerializeField] private CollectibleManager collectibleManager;
    [SerializeField] private MinimapMissionController minimapController;
    [SerializeField] private TravelTimeCalculator travelTimeCalculator;
    [SerializeField] private WaypointGenerator waypointGenerator;
    [SerializeField] private GameObject postCheckpointHigh;
    [SerializeField] private GameObject postCheckpointMid;
    [SerializeField] private GameObject postCheckpointLow;
    [SerializeField] private GameObject gpsCheckpoint;
    [SerializeField] private Transform checkpointParent;
    [SerializeField] private Transform gpsCheckpointParent;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private AudioClip lowMusic;

    private Waypoint currentWaypoint;
    private Transform currentObjective;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(lowMusic);
        SaveManager.Instance.AddRun();
    }

    public void SpawnNextCheckpoint()
    {
        if (waypointGenerator.Waypoints.Count == 0)
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
        minimapController.ShowCheckpoint(currentCheckpoint.transform);
    }

    public void SpawnNextObjective(Transform objectiveTransform)
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

        Vector3 spawnPosition = objectiveTransform.position;
        GameObject currentCheckpoint = Instantiate(gpsCheckpoint, spawnPosition, objectiveTransform.rotation, gpsCheckpointParent);
        currentObjective = currentCheckpoint.transform;
        minimapController.ShowCheckpoint(currentCheckpoint.transform);
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
            randomWaypoint = waypointGenerator.Waypoints[Random.Range(0, waypointGenerator.Waypoints.Count)];
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

    public void StartCheckpointTimer(float timeBuffer, float multiplier = 1f)
    {
        float duration = travelTimeCalculator.CalculateTime(currentObjective, timeBuffer);
        MissionTimer.Instance.StartTimer(duration, multiplier);
    }

    public void SetCurrentObjective(Transform target)
    {
        currentObjective = target;
        minimapController.ShowCheckpoint(target.transform);
    }

    public void CleanCurrentObjective()
    {
        currentObjective = null;
    }

    public void ClearCheckpoint()
    {
        if (currentObjective != null)
            minimapController.HideCheckpoint(currentObjective.transform);
    }

    public void NotifyCheckpointReached(Transform checkpoint)
    {
        if (currentObjective != checkpoint)
            return;
        ClearCheckpoint();
        CleanCurrentObjective();
        OnCheckpointReached?.Invoke(checkpoint);
    }
}
