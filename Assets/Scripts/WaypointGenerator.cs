using UnityEngine;

public class WaypointGenerator : MonoBehaviour
{
    public GameObject waypointPrefab;
    public float offset = 15f;

    [SerializeField] private WaypointValidator validator;

    void Start()
    {
        GenerateWaypoints();
    }

    void GenerateWaypoints()
    {
        Vector3[] directions =
        {
            transform.forward,
            -transform.forward,
            transform.right,
            -transform.right
        };

        foreach (Vector3 dir in directions)
        {
            Vector3 spawnPos =
                transform.position + dir * offset;

            if (validator.IsValidPosition(spawnPos))
            {
                Instantiate(
                    waypointPrefab,
                    spawnPos,
                    Quaternion.Euler(0f, 0f, 90f)
                );
            }
        }
    }
}
