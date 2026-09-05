using System.Collections.Generic;
using UnityEngine;

public class RandomCityGenerator : MonoBehaviour
{
    [Header("City Size")]
    [SerializeField] private int width = 30;
    [SerializeField] private int height = 20;

    [Header("Building Zones")]
    [SerializeField] private int buildingZoneCount = 5;

    [Header("Main Roads")]
    [SerializeField] private int mainRoadCount = 5;
    [SerializeField] private int minMainRoadLength = 5;

    [Header("Dimensions")]
    [SerializeField] private float cellSize = 6f;
    [SerializeField] private int buildingZoneSize = 4;

    [Header("Prefabs")]
    [SerializeField] private GameObject straightRoadPrefab;
    [SerializeField] private GameObject[] buildingZonePrefabs;
    [SerializeField] private GameObject cornerRoadPrefab;
    [SerializeField] private GameObject intersectionPrefab;
    [SerializeField] private GameObject[] grassPrefabs;
    [SerializeField] private Transform roadParent;
    [SerializeField] private Transform buildingParent;
    [SerializeField] private Transform grassParent;

    private CellType[,] grid;
    private readonly List<Vector2Int> buildingZoneStarts = new List<Vector2Int>();
    private struct RoadConnections
    {
        public bool left;
        public bool right;
        public bool bottom;
        public bool top;
        public int Count => (left ? 1 : 0) + (right ? 1 : 0) + (bottom ? 1 : 0) + (top ? 1 : 0);
    }
    private struct RoadStart
    {
        public Vector2Int position;
        public Vector2Int direction;
    }
    private enum Side { Bottom, Top, Left, Right }

    private void Start()
    {
        ClearCity();
        GenerateCity();
    }

    public void GenerateCity()
    {
        CreateGrid();
        GenerateBorderRoad();
        GenerateBuildingZones();
        GenerateMainRoads();
        GeneratePrefabs();
    }

    private void CreateGrid()
    {
        grid = new CellType[width, height];
        buildingZoneStarts.Clear();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = CellType.Empty;
            }
        }
    }

    private void GenerateBorderRoad()
    {
        for (int x = 0; x < width; x++)
        {
            grid[x, 0] = CellType.Road;
            grid[x, height - 1] = CellType.Road;
        }
        for (int y = 0; y < height; y++)
        {
            grid[0, y] = CellType.Road;
            grid[width - 1, y] = CellType.Road;
        }
    }

    private void GenerateBuildingZones()
    {
        int generatedZones = 0;
        int attempts = 0;
        int maxAttempts = buildingZoneCount * 100;

        while (generatedZones < buildingZoneCount && attempts < maxAttempts)
        {
            attempts++;
            int startX = Random.Range(1, width - buildingZoneSize);
            int startY = Random.Range(1, height - buildingZoneSize);
            if (!CanPlaceBuildingZone(startX, startY))
                continue;
            PlaceBuildingZone(startX, startY);
            generatedZones++;
        }
    }

    private bool CanPlaceBuildingZone(int startX, int startY)
    {
        for (int x = startX; x < startX + buildingZoneSize; x++)
        {
            for (int y = startY; y < startY + buildingZoneSize; y++)
            {
                if (grid[x, y] != CellType.Empty)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void PlaceBuildingZone(int startX, int startY)
    {
        for (int x = startX; x < startX + buildingZoneSize; x++)
        {
            for (int y = startY; y < startY + buildingZoneSize; y++)
            {
                grid[x, y] = CellType.BuildingZone;
            }
        }
        buildingZoneStarts.Add(new Vector2Int(startX, startY));
    }

    private void GenerateMainRoads()
    {
        int generatedRoads = 0;
        int attempts = 0;
        int maxAttempts = mainRoadCount * 20;
        while (generatedRoads < mainRoadCount && attempts < maxAttempts)
        {
            attempts++;
            if (TryGenerateMainRoad())
            {
                generatedRoads++;
            }
        }
    }

    private bool TryGenerateMainRoad()
    {
        RoadStart start = GetRandomRoadStartFromSides();
        List<Vector2Int> newRoadCells = GenerateMainRoadCells(start.position, start.direction);
        if (newRoadCells.Count < minMainRoadLength)
        {
            RemoveMainRoadCells(newRoadCells);
            return false;
        }
        return true;
    }

    private RoadStart GetRandomRoadStartFromSides()
    {
        int side = Random.Range(0, 4);
        switch (side)
        {
            case (int)Side.Bottom:
                return new RoadStart
                {
                    position = new Vector2Int(Random.Range(1, width - 1), 0),
                    direction = Vector2Int.up
                };
            case (int)Side.Top:
                return new RoadStart
                {
                    position = new Vector2Int(Random.Range(1, width - 1), height - 1),
                    direction = Vector2Int.down
                };
            case (int)Side.Left:
                return new RoadStart
                {
                    position = new Vector2Int(0, Random.Range(1, height - 1)),
                    direction = Vector2Int.right
                };
            default:
                return new RoadStart
                {
                    position = new Vector2Int(width - 1, Random.Range(1, height - 1)),
                    direction = Vector2Int.left
                };
        }
    }

    private List<Vector2Int> GenerateMainRoadCells(Vector2Int startPosition, Vector2Int direction)
    {
        List<Vector2Int> newRoadCells = new List<Vector2Int>();
        Vector2Int currentPosition = startPosition;
        int maxLength = Mathf.Max(width, height) * 2;

        for (int i = 0; i < maxLength; i++)
        {
            Vector2Int nextPosition = currentPosition + direction;
            int nextX = nextPosition.x;
            int nextY = nextPosition.y;

            if (!CanPlaceMainRoadCell(nextX, nextY, direction.x, direction.y))
                break;

            grid[nextX, nextY] = CellType.Road;
            newRoadCells.Add(nextPosition);
            currentPosition = nextPosition;
        }
        return newRoadCells;
    }

    private void RemoveMainRoadCells(List<Vector2Int> roadCells)
    {
        foreach (Vector2Int cell in roadCells)
        {
            grid[cell.x, cell.y] = CellType.Empty;
        }
    }

    private bool CanPlaceMainRoadCell(int x, int y, int directionX, int directionY)
    {
        if (x <= 0 || x >= width - 1 || y <= 0 || y >= height - 1)
            return false;
        if (grid[x, y] == CellType.Road)
            return false;
        if (grid[x, y] == CellType.BuildingZone)
            return false;

        if (directionY != 0)
        {
            if (HasRoadNeighboarsOnX(x, y))
                return false;
        }
        if (directionX != 0)
        {
            if (HasRoadNeighboarsOnY(x, y))
                return false;
        }

        return true;
    }

    private bool HasRoadNeighboarsOnX(int x, int y)
    {
        if (IsRoad(x - 1, y))
            return true;
        if (IsRoad(x + 1, y))
            return true;
        return false;
    }

    private bool HasRoadNeighboarsOnY(int x, int y)
    {
        if (IsRoad(x, y - 1))
            return true;
        if (IsRoad(x, y + 1))
            return true;
        return false;
    }

    private void GeneratePrefabs()
    {
        GenerateRoadPrefabs();
        GenerateBuildingZonePrefabs();
        GenerateGrassPrefabs();
    }

    private void GenerateRoadPrefabs()
    {
        if (straightRoadPrefab == null)
            return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] != CellType.Road)
                    continue;

                GenerateRoadPrefab(x, y);
            }
        }
    }

    private void GenerateRoadPrefab(int x, int y)
    {
        Vector3 position = GetCellWorldPosition(x, y);
        RoadConnections connections = GetRoadConnections(x, y);

        if (IsIntersection(connections))
        {
            Instantiate(intersectionPrefab, position, Quaternion.identity, roadParent);
            return;
        }
        if (IsCorner(connections))
        {
            GenerateCornerPrefab(connections, position);
            return;
        }
        if (connections.left || connections.right)
        {
            Instantiate(straightRoadPrefab, position, Quaternion.Euler(0f, 90f, 0f), roadParent);
            return;
        }
        if (connections.bottom || connections.top)
        {
            Instantiate(straightRoadPrefab, position, Quaternion.identity, roadParent);
        }
    }

    private void GenerateCornerPrefab(RoadConnections connections, Vector3 position)
    {
        if (cornerRoadPrefab == null)
            return;

        float rotationY = 0f;
        if (connections.left && connections.bottom)
            rotationY = 270f;
        else if (connections.right && connections.bottom)
            rotationY = 180f;
        else if (connections.right && connections.top)
            rotationY = 90f;
        else if (connections.left && connections.top)
            rotationY = 0f;

        Instantiate(cornerRoadPrefab, position, Quaternion.Euler(0f, rotationY, 0f), roadParent);
    }

    private bool IsCorner(RoadConnections connections)
    {
        if (connections.Count != 2)
            return false;
        bool hasHorizontalRoad = connections.left || connections.right;
        bool hasVerticalRoad = connections.bottom || connections.top;
        return hasHorizontalRoad && hasVerticalRoad;
    }

    private RoadConnections GetRoadConnections(int x, int y)
    {
        return new RoadConnections { left = IsRoad(x - 1, y), right = IsRoad(x + 1, y), bottom = IsRoad(x, y - 1), top = IsRoad(x, y + 1) };
    }

    private bool IsIntersection(RoadConnections connections)
    {
        return connections.Count >= 3;
    }

    private bool IsRoad(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return false;
        return grid[x, y] == CellType.Road;
    }

    private void GenerateBuildingZonePrefabs()
    {
        foreach (Vector2Int startPosition in buildingZoneStarts)
        {
            GenerateBuildingZonePrefab(startPosition.x, startPosition.y);
        }
    }

    private void GenerateBuildingZonePrefab(int startX, int startY)
    {
        if (buildingZonePrefabs == null || buildingZonePrefabs.Length == 0)
            return;

        float offset = (buildingZoneSize - 1) * 0.5f;
        Vector3 position = buildingParent.position + new Vector3(
            (startX + offset) * cellSize,
            0f,
            (startY + offset) * cellSize
        );

        GameObject prefab = buildingZonePrefabs[Random.Range(0, buildingZonePrefabs.Length)];
        float rotationY = Random.Range(0, 4) * 90f;

        Instantiate(prefab, position, Quaternion.Euler(0f, rotationY, 0f), buildingParent);
    }

    private Vector3 GetCellWorldPosition(int x, int y)
    {
        return roadParent.position + new Vector3(x * cellSize, 0f, y * cellSize);
    }

    private void GenerateGrassPrefabs()
    {
        if (grassPrefabs == null || grassPrefabs.Length == 0)
            return;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] != CellType.Empty)
                    continue;
                GameObject prefab = grassPrefabs[Random.Range(0, grassPrefabs.Length)];
                Vector3 position = GetCellWorldPosition(x, y);
                float rotationY = Random.Range(0, 4) * 90f;
                Instantiate(prefab, position, Quaternion.Euler(0f, rotationY, 0f), grassParent);
            }
        }
    }

    public void ClearCity()
    {
        ClearChildren(roadParent);
        ClearChildren(buildingParent);
        ClearChildren(grassParent);
    }

    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            GameObject child = parent.GetChild(i).gameObject;
            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }
    }

    private void PrintGrid()
    {
        string result = "";
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                switch (grid[x, y])
                {
                    case CellType.Road:
                        result += " R ";
                        break;
                    case CellType.BuildingZone:
                        result += " B ";
                        break;
                    default:
                        result += " X ";
                        break;
                }
            }
            result += "\n";
        }
        Debug.Log(result);
    }
}