using System.Collections.Generic;
using UnityEngine;

public class HelicopterPatrolManager : MonoBehaviour
{
    [SerializeField] private PatrolHelicopterController helicopterPrefab;
    [SerializeField] private Transform patrolPointsParent;
    [SerializeField] private Transform helicopterParent;
    [SerializeField] private Transform player;
    [SerializeField] private int helicopterCount = 3;
    [SerializeField] private float minHeight = 151f;
    [SerializeField] private float maxHeight = 250f;
    [SerializeField] private float minimumSpawnDistance = 160f;

    private readonly List<Transform> patrolPoints = new();
    private readonly List<PatrolHelicopterController> helicopters = new();

    private void Awake()
    {
        foreach (Transform child in patrolPointsParent)
        {
            patrolPoints.Add(child);
        }
    }

    public void SpawnHelicopters()
    {
        Clear();
        if (patrolPoints.Count == 0)
        {
            Debug.LogWarning("No helicopter patrol points found!");
            return;
        }
        List<Transform> availablePoints = GetValidSpawnPoints();
        if (availablePoints.Count == 0)
        {
            Debug.LogWarning("No helicopter spawn points are far enough from the player.");
            return;
        }

        int count = Mathf.Min(helicopterCount, availablePoints.Count);
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, availablePoints.Count);
            Transform spawnPoint = availablePoints[index];
            availablePoints.RemoveAt(index);
            float randomY = Random.Range(minHeight, maxHeight);
            Vector3 spawnPosition = new Vector3(spawnPoint.position.x, randomY, spawnPoint.position.z);
            PatrolHelicopterController helicopter = Instantiate(helicopterPrefab, spawnPosition, Quaternion.identity, helicopterParent);
            helicopter.Initialize(player, spawnPoint, this);
            helicopters.Add(helicopter);
        }
    }

    private List<Transform> GetValidSpawnPoints()
    {
        List<Transform> validPoints = new();
        Vector3 playerPosition = player.position;
        foreach (Transform point in patrolPoints)
        {
            Vector2 pointXZ = new Vector2(point.position.x, point.position.z);
            Vector2 playerXZ = new Vector2(playerPosition.x, playerPosition.z);
            float distance = Vector2.Distance(pointXZ, playerXZ);
            if (distance >= minimumSpawnDistance)
            {
                validPoints.Add(point);
            }
        }
        return validPoints;
    }

    public Transform GetNextPatrolPoint(Transform currentPoint, Transform previousPoint)
    {
        List<Transform> available = new();
        foreach (Transform point in patrolPoints)
        {
            if (point == currentPoint)
                continue;
            if (point == previousPoint)
                continue;
            available.Add(point);
        }

        if (available.Count == 0)
            return patrolPoints[Random.Range(0, patrolPoints.Count)];
        return available[Random.Range(0, available.Count)];
    }

    public void Clear()
    {
        foreach (PatrolHelicopterController helicopter in helicopters)
        {
            if (helicopter != null)
                Destroy(helicopter.gameObject);
        }
        helicopters.Clear();
    }
}
