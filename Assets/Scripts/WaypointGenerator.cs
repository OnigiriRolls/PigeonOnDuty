using UnityEngine;

public class WaypointGenerator : MonoBehaviour
{
    public float offset = 15f;

    [Header("References")]
    [SerializeField] private GameObject buildingsParent;
    [SerializeField] private GameObject postWaypointPrefab;
    [SerializeField] private Transform postWaypointParent;
    [SerializeField] private WaypointValidator validator;
    [SerializeField] private int midPostCount = 100;
    [SerializeField] private int highPostCount = 200;

    [Header("Altitude Layers")]
    [SerializeField] private string midHeight1 = "Sky_small";
    [SerializeField] private string midHeight2 = "Residential";
    [SerializeField] private Vector2 midHeightRange = new(0f, 0f);
    [SerializeField] private string highHeight = "Sky_big";
    [SerializeField] private Vector2 highHeightRange = new (0f, 0f);
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
        for (int i = 0; i < count; i++)
        {
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
            return AltitudeLayer.High;
        if (name.Contains(midHeight1) || name.Contains(midHeight2))
            return AltitudeLayer.Mid;
        return AltitudeLayer.Low;
    }
}
