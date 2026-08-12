using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogPatrolManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PatrolZone patrolZonePrefab;
    [SerializeField] private DogController dogPrefab;
    [SerializeField] private Transform dogParent;

    [Header("City")]
    [SerializeField] private Vector2 worldMin;
    [SerializeField] private Vector2 worldMax;
    [SerializeField] private float height = 0.1f;

    [Header("Dog Zones")]
    [SerializeField] private int gridX = 2;
    [SerializeField] private int gridZ = 2;
    [SerializeField] private int dogZoneCount = 4;
    [SerializeField] private float patrolRadius = 60f;

    private readonly List<PatrolZone> patrolZones = new();
    private readonly List<DogController> dogs = new();

    public void GenerateDogs()
    {
        ClearDogs();
        ClearPatrolZones();
        List<Bounds> cells = CreateGridCells();
        Shuffle(cells);
        int count = Mathf.Min(dogZoneCount, cells.Count);
        for (int i = 0; i < count; i++)
        {
            CreateDogZone(cells[i]);
        }
    }

    private void CreateDogZone(Bounds cell)
    {
        Vector3 position = RandomPointInside(cell);
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            //Debug.LogWarning($"Could not find NavMesh near {position}. Dog will not spawn.");
            return;
        }
        PatrolZone zone = Instantiate(patrolZonePrefab, position, Quaternion.identity, dogParent);
        zone.InitNavMeshPoints(patrolRadius);
        DogController dog = Instantiate(dogPrefab, position, Quaternion.identity, dogParent);
        dogs.Add(dog);
        dog.PatrolZone = zone;
        patrolZones.Add(zone);
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
                cell.SetMinMax(new Vector3(minCellX, height, minCellZ), new Vector3(maxCellX, height, maxCellZ));
                cells.Add(cell);
            }
        }
        return cells;
    }

    private Vector3 RandomPointInside(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.center.y,
            Random.Range(bounds.min.z, bounds.max.z));
    }

    private void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public void ClearDogs()
    {
        foreach (DogController dog in dogs)
        {
            if (dog != null)
            {
                dog.Deactivate();
                Destroy(dog.gameObject);
            }
        }
        dogs.Clear();
    }

    public void ClearPatrolZones()
    {
        foreach (PatrolZone zone in patrolZones)
        {
            if (zone != null)
                Destroy(zone.gameObject);
        }
        patrolZones.Clear();
    }
}
