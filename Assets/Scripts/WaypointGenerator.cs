using UnityEngine;

public class WaypointGenerator : MonoBehaviour
{
    public float offset = 15f;

    [Header("References")]
    [SerializeField] private GameObject buildingWaypointPrefab;
    [SerializeField] private GameObject postWaypointPrefab;
    [SerializeField] private Transform buildingWaypointParent;
    [SerializeField] private Transform postWaypointParent;
    [SerializeField] private WaypointValidator validator;
    [SerializeField] private int midPostCount = 3;
    [SerializeField] private int highPostCount = 5;

    [Header("Altitude Layers")]
    [SerializeField] private string midHeight1 = "Sky_small";
    [SerializeField] private string midHeight2 = "Residential";
    [SerializeField] private string highHeight = "Sky_big";

    private Renderer buildingRenderer;
    private Vector3 areaSize;

    private void Awake()
    {
        buildingRenderer = GetComponent<Renderer>();
    }

    void Start()
    {
        GenerateWaypoints();
    }

    private void GenerateWaypoints()
    {
        GenerateSideWaypoints();
        GenerateRooftopWaypoint();
    }

    private void GenerateSideWaypoints()
    {
        Vector3[] directions = { transform.forward, -transform.forward, transform.right, -transform.right };
        foreach (Vector3 dir in directions)
        {
            Vector3 spawnPos = transform.position + dir * offset;
            TrySpawnBuildingWaypoint(spawnPos);
        }
    }

    private void GenerateRooftopWaypoint()
    {
        Bounds bounds = buildingRenderer.bounds;
        Vector3 rooftopPos = bounds.center + Vector3.up * (bounds.extents.y + offset);
        TrySpawnBuildingWaypoint(rooftopPos);
    }

    private void GeneratePostWaypoints(int count, float height, AltitudeLayer layer)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-areaSize.x, areaSize.x),
                height,
                Random.Range(-areaSize.z, areaSize.z)
            );

            if (!validator.IsValidPosition(randomPos))
                continue;

            GameObject obj = Instantiate(postWaypointPrefab, randomPos, Quaternion.identity, postWaypointParent);
            Waypoint waypoint = obj.GetComponent<Waypoint>();
            waypoint.AltitudeLayer = layer;
        }
    }

    private void TrySpawnBuildingWaypoint(Vector3 spawnPos)
    {
        if (!validator.IsValidPosition(spawnPos))
            return;

        GameObject waypointObject = Instantiate(buildingWaypointPrefab, spawnPos, Quaternion.identity, buildingWaypointParent);
        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
        waypoint.AltitudeLayer = GetAltitudeLayer(waypointObject.name);
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
