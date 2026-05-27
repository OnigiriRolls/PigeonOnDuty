using UnityEngine;

public class WaypointGenerator : MonoBehaviour
{
    public float offset = 15f;
    public int lowWaypoints = 0;
    public int midWaypoints = 0;
    public int highWaypoints = 0;

    [Header("References")]
    [SerializeField] private GameObject buildingsParent;
    [SerializeField] private GameObject postWaypointPrefab;
    [SerializeField] private Transform postWaypointParent;
    [SerializeField] private WaypointValidator validator;
    [SerializeField] private int lowPostCount = 200;
    [SerializeField] private int midPostCount = 200;
    [SerializeField] private int highPostCount = 200;

    [Header("Altitude Layers")]
    [SerializeField] private Vector2 lowHeightRange = new(0f, 0f);
    [SerializeField] private string midHeight1 = "Sky_small";
    [SerializeField] private string midHeight2 = "Residential";
    [SerializeField] private Vector2 midHeightRange = new(0f, 0f);
    [SerializeField] private string highHeight = "Sky_big";
    [SerializeField] private Vector2 highHeightRange = new(0f, 0f);
    [SerializeField] private Vector2 xRange;
    [SerializeField] private Vector2 zRange;

    void Start()
    {
        GenerateWaypoints();
    }

    private void GenerateWaypoints()
    {
        foreach (Transform building in buildingsParent.transform)
        {
            GenerateSideWaypoints(building);
            GenerateRooftopWaypoint(building);
        }

        GeneratePostWaypoints(lowPostCount, lowHeightRange, AltitudeLayer.Low);
        GeneratePostWaypoints(midPostCount, midHeightRange, AltitudeLayer.Mid);
        GeneratePostWaypoints(highPostCount, highHeightRange, AltitudeLayer.High);
    }

    private void GenerateSideWaypoints(Transform building)
    {
        Vector3[] directions = { building.forward, -building.forward, building.right, -building.right };
        foreach (Vector3 dir in directions)
        {
            Vector3 spawnPos = building.position + dir * offset;
            TrySpawnBuildingWaypoint(spawnPos, building.name);
        }
    }

    private void GenerateRooftopWaypoint(Transform building)
    {
        Renderer renderer = building.GetComponent<Renderer>();
        if (renderer == null)
            return;
        Bounds bounds = renderer.bounds;
        Vector3 rooftopPos = bounds.center + Vector3.up * (bounds.extents.y + offset);
        TrySpawnBuildingWaypoint(rooftopPos, building.name);
    }

    private void GeneratePostWaypoints(int count, Vector2 height, AltitudeLayer layer)
    {
        int spawned = 0;
        int attempts = 0;
        while (spawned < count && attempts < count * 2)
        {
            attempts++;
            Vector3 randomPos = new Vector3(
                    Random.Range(xRange.x, xRange.y),
                    Random.Range(height.x, height.y),
                    Random.Range(zRange.x, zRange.y)
                );
            if (!validator.IsValidPosition(randomPos))
                continue;

            GameObject obj = Instantiate(postWaypointPrefab, randomPos, Quaternion.identity, postWaypointParent);
            Waypoint waypoint = obj.GetComponent<Waypoint>();
            waypoint.AltitudeLayer = layer;
            spawned++;
            if (layer == AltitudeLayer.High)
                highWaypoints++;
            else if (layer == AltitudeLayer.Mid) midWaypoints++;
            else lowWaypoints++;
        }
    }

    private void TrySpawnBuildingWaypoint(Vector3 spawnPos, string buildingName)
    {
        if (!validator.IsValidPosition(spawnPos))
            return;

        GameObject waypointObject = Instantiate(postWaypointPrefab, spawnPos, Quaternion.identity, postWaypointParent);
        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
        waypoint.AltitudeLayer = GetAltitudeLayer(buildingName);
    }


    private AltitudeLayer GetAltitudeLayer(string name)
    {
        if (name.Contains(highHeight))
        {
            highWaypoints++;
            highPostCount--;
            return AltitudeLayer.High;
        }
        if (name.Contains(midHeight1) || name.Contains(midHeight2))
        {
            midWaypoints++;
            midPostCount--;
            return AltitudeLayer.Mid;
        }
        lowPostCount--;
        lowWaypoints++;
        return AltitudeLayer.Low;
    }
}
