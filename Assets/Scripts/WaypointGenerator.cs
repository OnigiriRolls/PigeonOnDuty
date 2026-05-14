using UnityEngine;

public class WaypointGenerator : MonoBehaviour
{
    public GameObject waypointPrefab;
    public float offset = 15f;

    [SerializeField] private Transform waypointParent;
    [SerializeField] private WaypointValidator validator;
    private Renderer buildingRenderer;

    private void Awake()
    {
        buildingRenderer = GetComponent<Renderer>();
    }

    void Start()
    {
        GenerateWaypoints();
    }

    void GenerateWaypoints()
    {
        Vector3[] directions = { transform.forward, -transform.forward, transform.right, -transform.right };
        foreach (Vector3 dir in directions)
        {
            Vector3 spawnPos = transform.position + dir * offset;
            if (validator.IsValidPosition(spawnPos))
            {
                Instantiate(waypointPrefab, spawnPos, Quaternion.identity, waypointParent);
            }
        }

        Bounds bounds = buildingRenderer.bounds;
        Vector3 center = bounds.center;
        Vector3 rooftopPos = center + Vector3.up * (bounds.extents.y + offset);
        if (validator.IsValidPosition(rooftopPos))
        {
            Instantiate(waypointPrefab, rooftopPos, Quaternion.identity, waypointParent);
        }
    }
}
