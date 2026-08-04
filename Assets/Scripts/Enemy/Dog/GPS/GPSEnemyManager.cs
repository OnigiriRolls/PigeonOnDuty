using System.Collections.Generic;
using UnityEngine;

public enum GPSEnemyType
{
    Empty,
    Dog,
    Crow
}

public class GPSEnemyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PatrolZone patrolZonePrefab;
    [SerializeField] private GPSDogController dogPrefab;
    [SerializeField] private PatrolCrowController crowPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private Transform despawnPoint;
    [SerializeField] private Transform enemyParent;

    [Header("City")]
    [SerializeField] private Vector2 worldMin;
    [SerializeField] private Vector2 worldMax;
    [SerializeField] private float height = 0.1f;
    [SerializeField] private float minHeight = 0.1f;
    [SerializeField] private float maxHeight = 0.1f;

    [Header("Enemy Zones")]
    [SerializeField] private int gridX = 2;
    [SerializeField] private int gridZ = 2;
    [SerializeField] private int dogCount = 4;
    [SerializeField] private int crowCount = 3;
    [SerializeField] private float patrolRadius = 60f;
    [SerializeField] private float safeRadiusFromTraveler = 80f;
    [SerializeField] private float safeRadiusFromDestination = 80f;

    private readonly List<PatrolZone> patrolZones = new();
    private readonly List<GPSDogController> dogs = new();
    private readonly List<PatrolCrowController> crows = new();

    public void GenerateEnemies(Vector3 travelerPos, Vector3 destinationPos)
    {
        Clear();

        List<Bounds> cells = CreateGridCells();
        cells.RemoveAll(cell =>
            Vector3.Distance(cell.center, travelerPos) < safeRadiusFromTraveler ||
            Vector3.Distance(cell.center, destinationPos) < safeRadiusFromDestination);
        Shuffle(cells);
        int index = 0;
        for (int i = 0; i < dogCount && index < cells.Count; i++)
        {
            CreateCell(cells[index++], GPSEnemyType.Dog);
        }

        for (int i = 0; i < crowCount && index < cells.Count; i++)
        {
            CreateCell(cells[index++], GPSEnemyType.Crow);
        }
    }

    private void CreateCell(Bounds cell, GPSEnemyType type)
    {
        Vector3 position = cell.center;
        switch (type)
        {
            case GPSEnemyType.Dog:
                PatrolZone zone1 = Instantiate(patrolZonePrefab, position, Quaternion.identity, enemyParent);
                patrolZones.Add(zone1);
                zone1.InitNavMeshPoints(patrolRadius);
                GPSDogController dog = Instantiate(dogPrefab, position, Quaternion.identity, enemyParent);
                dog.PatrolZone = zone1;
                dogs.Add(dog);
                break;
            case GPSEnemyType.Crow:
                position.y = Random.Range(minHeight, maxHeight);
                PatrolZone zone2 = Instantiate(patrolZonePrefab, position, Quaternion.identity, enemyParent);
                patrolZones.Add(zone2);
                zone2.InitPatrolPoints(patrolRadius);
                PatrolCrowController crow = Instantiate(crowPrefab, position, Quaternion.identity, enemyParent);
                crow.Initialize(player, zone2, despawnPoint);
                crows.Add(crow);
                break;
        }
    }

    private List<Bounds> CreateGridCells()
    {
        List<Bounds> cells = new();
        float cityWidth = worldMax.x - worldMin.x;
        float cityDepth = worldMax.y - worldMin.y;
        float cellWidth = cityWidth / gridX;
        float cellDepth = cityDepth / gridZ;
        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                float minCellX = worldMin.x + x * cellWidth;
                float maxCellX = minCellX + cellWidth;
                float minCellZ = worldMin.y + z * cellDepth;
                float maxCellZ = minCellZ + cellDepth;
                Bounds cell = new Bounds();
                cell.SetMinMax(
                    new Vector3(minCellX, height, minCellZ),
                    new Vector3(maxCellX, height, maxCellZ));
                cells.Add(cell);
            }
        }
        return cells;
    }

    private void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public void Clear()
    {
        foreach (GPSDogController dog in dogs)
        {
            if (dog == null)
                continue;
            dog.Deactivate();
            Destroy(dog.gameObject);
        }
        dogs.Clear();
        foreach (PatrolCrowController crow in crows)
        {
            if (crow != null)
            {
                crow.Deactivate();
                Destroy(crow.gameObject);
            }
        }

        crows.Clear();

        foreach (PatrolZone zone in patrolZones)
        {
            if (zone != null)
                Destroy(zone.gameObject);
        }
        patrolZones.Clear();
    }
}
