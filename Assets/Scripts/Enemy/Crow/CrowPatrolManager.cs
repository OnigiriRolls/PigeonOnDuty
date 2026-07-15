using System.Collections.Generic;
using UnityEngine;

public class CrowPatrolManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StealerCrowController crowStealerPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private Transform spawnParent;

    [Header("World")]
    [SerializeField] private Vector2 worldMin;
    [SerializeField] private Vector2 worldMax;
    [SerializeField] private float minHeight = 20f;
    [SerializeField] private float maxHeight = 35f;

    [Header("Crow Density")]
    [SerializeField] private float crowsPerSquareKilometer = 12f;

    [Header("Spawn")]
    [SerializeField] private float zoneSpacing = 20f;
    [SerializeField] private float patrolRadius = 60f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float obstacleCheckRadius = 5f;
    [SerializeField] private int maxSpawnAttempts = 20;
    [SerializeField] private Transform despawnPoint;

    private readonly List<CrowPatrolZone> patrolZones = new();

    private void Start()
    {
        SpawnCrowZones();
    }

    private void SpawnCrowZones()
    {
        float width = worldMax.x - worldMin.x;
        float height = worldMax.y - worldMin.y;
        float areaKm2 = (width * height) / 1_000_000f;
        int crowCount = Mathf.Max(1, Mathf.RoundToInt(areaKm2 * crowsPerSquareKilometer));
        float cellSize = patrolRadius * 2f + zoneSpacing;
        int columns = Mathf.FloorToInt(width / cellSize);
        int rows = Mathf.FloorToInt(height / cellSize);

        List<Vector2Int> cells = new();
        for (int x = 0; x < columns; x++)
        {
            for (int z = 0; z < rows; z++)
            {
                cells.Add(new Vector2Int(x, z));
            }
        }

        Shuffle(cells);

        crowCount = Mathf.Min(crowCount, cells.Count);
        for (int i = 0; i < crowCount; i++)
        {
            SpawnCrowInCell(cells[i], cellSize);
        }
        Debug.Log(patrolZones.Count);
    }

    private void SpawnCrowInCell(Vector2Int cell, float cellSize)
    {
        float maxOffset = (cellSize - patrolRadius * 2f) * 0.5f;
        Vector3 cellCenter = new(worldMin.x + (cell.x + 0.5f) * cellSize, 0f, worldMin.y + (cell.y + 0.5f) * cellSize);
        //for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        //{
            Vector2 offset = Random.insideUnitCircle * maxOffset;
            Vector3 position = cellCenter + new Vector3(offset.x, Random.Range(minHeight, maxHeight), offset.y);

            //if (Physics.CheckSphere(position, obstacleCheckRadius, obstacleMask))
            //    continue;

            CrowPatrolZone zone = new(position, patrolRadius, obstacleMask, obstacleCheckRadius, maxSpawnAttempts);
            patrolZones.Add(zone);
            StealerCrowController crow = Instantiate(crowStealerPrefab, position, Quaternion.identity, spawnParent);
            crow.Initialize(player, zone, despawnPoint);
        //    return;
        //}

        //Debug.Log("Couldn't spawn crow in cell.");
    }

    private void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
