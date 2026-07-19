using System.Collections.Generic;
using UnityEngine;

public class DogPatrolManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PatrolZone patrolZonePrefab;
    [SerializeField] private DogController dogPrefab;

    [Header("City")]
    [SerializeField] private Vector2 citySize = new Vector2(600f, 600f);
    [SerializeField] private Vector3 cityCenter = Vector3.zero;

    [Header("Dog Zones")]
    [SerializeField] private int gridX = 2;
    [SerializeField] private int gridZ = 2;
    [SerializeField] private int dogZoneCount = 4;
    [SerializeField] private float patrolRadius = 60f;

    [Header("Obstacle Check")]
    [SerializeField] private LayerMask obstacleMask;

    private readonly List<PatrolZone> patrolZones = new();

    private void Start()
    {
        GenerateDogs();
    }

    public void GenerateDogs()
    {
        patrolZones.Clear();

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

        PatrolZone zone = Instantiate(
            patrolZonePrefab,
            position,
            Quaternion.identity,
            transform);

        zone.SetRadius(patrolRadius);

        DogController dog = Instantiate(
            dogPrefab,
            position,
            Quaternion.identity,
            transform);

        dog.PatrolZone = zone;

        patrolZones.Add(zone);
    }

    private List<Bounds> CreateGridCells()
    {
        List<Bounds> cells = new();

        float cellWidth = citySize.x / gridX;
        float cellDepth = citySize.y / gridZ;

        Vector3 origin = cityCenter -
            new Vector3(citySize.x * 0.5f, 0f, citySize.y * 0.5f);

        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                Vector3 center = origin +
                    new Vector3(
                        x * cellWidth + cellWidth * 0.5f,
                        0f,
                        z * cellDepth + cellDepth * 0.5f);

                cells.Add(new Bounds(
                    center,
                    new Vector3(cellWidth, 0f, cellDepth)));
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
}
