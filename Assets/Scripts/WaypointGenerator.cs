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
    [SerializeField] private float lowHeight = 20f;
    [SerializeField] private float midHeight = 60f;
    [SerializeField] private float highHeight = 80f;

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
            TrySpawnWaypoint(spawnPos);
        }
    }

    private void GenerateRooftopWaypoint()
    {
        Bounds bounds = buildingRenderer.bounds;
        Vector3 rooftopPos = bounds.center + Vector3.up * (bounds.extents.y + offset);
        TrySpawnWaypoint(rooftopPos);
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

            if (!validator.IsValidPositionForPost(randomPos))
                continue;

            GameObject obj = Instantiate(postWaypointPrefab, randomPos, Quaternion.identity, postWaypointParent);
            Waypoint waypoint = obj.GetComponent<Waypoint>();
            waypoint.AltitudeLayer = layer;
        }
    }

    private void TrySpawnWaypoint(Vector3 spawnPos)
    {
        if (!validator.IsValidPositionForBuilding(spawnPos))
            return;

        GameObject waypointObject = Instantiate(buildingWaypointPrefab, spawnPos, Quaternion.identity, buildingWaypointParent);
        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
        waypoint.AltitudeLayer = GetAltitudeLayer(spawnPos.y);
    }


    private AltitudeLayer GetAltitudeLayer(float height)
    {
        if (height < lowHeight)
            return AltitudeLayer.Low;
        if (height < midHeight)
            return AltitudeLayer.Mid;
        return AltitudeLayer.High;
    }
}
