using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ParadeManager : MonoBehaviour
{
    [SerializeField] private GameObject paradeZonePrefab;
    [SerializeField] private Transform paradeParent;
    [SerializeField] private int minParades = 3;
    [SerializeField] private int maxParades = 8;
    [SerializeField] private float maxDistanceFromPath = 50f;
    [SerializeField] private float minimumDistanceBetweenParades = 200f;

    private ParadeSpawnPoint[] spawnPoints;
    private readonly List<ParadeZoneController> activeParades = new();

    private void Start()
    {
        spawnPoints = FindObjectsByType<ParadeSpawnPoint>();
    }

    public void RegisterParade(ParadeZoneController parade)
    {
        if (activeParades.Contains(parade))
            return;
        activeParades.Add(parade);
    }

    public void UnregisterParade(ParadeZoneController parade)
    {
        activeParades.Remove(parade);
    }

    public void ClearAllParades()
    {
        foreach (ParadeZoneController parade in activeParades.ToArray())
        {
            if (parade != null)
                parade.DisperseParade();
        }
    }

    public void SpawnParades(Vector3 playerPosition, Vector3 destinationPosition)
    {
        NavMeshHit hit;
        bool foundDestination = NavMesh.SamplePosition(destinationPosition, out hit, 50f, NavMesh.AllAreas);
        Debug.Log("foundDestination = " + foundDestination);
        Vector3 navMeshDestination = hit.position;
        Debug.DrawRay(hit.position, Vector3.up * 20f, Color.green, 30f);
        NavMeshPath path = new NavMeshPath();
        bool foundPath = NavMesh.CalculatePath(playerPosition, navMeshDestination, NavMesh.AllAreas, path);
        Debug.Log("found path = " + foundPath);
        SpawnParades(path);
    }

    private void SpawnParades(NavMeshPath path)
    {
        int count = Random.Range(minParades, maxParades + 1);
        List<ParadeSpawnPoint> selectedPoints = SelectParadeSpawnPoints(path, count);
        foreach (ParadeSpawnPoint point in selectedPoints)
        {
            Instantiate(paradeZonePrefab, point.transform.position, point.transform.rotation, paradeParent);
        }
    }

    private float DistanceToPath(Vector3 point, NavMeshPath path)
    {
        float minDistance = float.MaxValue;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            float distance = DistancePointToSegment(point, path.corners[i], path.corners[i + 1]);
            if (distance < minDistance)
                minDistance = distance;
        }
        return minDistance;
    }

    private float DistancePointToSegment(Vector3 point, Vector3 segmentStart, Vector3 segmentEnd)
    {
        Vector3 segment = segmentEnd - segmentStart;
        float t = Vector3.Dot(point - segmentStart, segment) / segment.sqrMagnitude;
        t = Mathf.Clamp01(t);
        Vector3 projection = segmentStart + segment * t;
        return Vector3.Distance(point, projection);
    }

    private List<ParadeSpawnPoint> SelectParadeSpawnPoints(NavMeshPath path, int paradeCount)
    {
        List<ParadeSpawnPoint> candidates = new();
        foreach (ParadeSpawnPoint spawnPoint in spawnPoints)
        {
            float distanceToPath = DistanceToPath(spawnPoint.transform.position, path);
            if (distanceToPath <= maxDistanceFromPath)
                candidates.Add(spawnPoint);
        }
        candidates = candidates.OrderBy(_ => Random.value).ToList();

        List<ParadeSpawnPoint> selected = new();
        foreach (ParadeSpawnPoint candidate in candidates)
        {
            bool tooClose = false;
            foreach (ParadeSpawnPoint chosen in selected)
            {
                float distance = Vector3.Distance(candidate.transform.position, chosen.transform.position);
                if (distance < minimumDistanceBetweenParades)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose)
                continue;
            selected.Add(candidate);
            if (selected.Count >= paradeCount)
                break;
        }
        return selected;
    }
}
