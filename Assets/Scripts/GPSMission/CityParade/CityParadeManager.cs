using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CityParadeManager : MonoBehaviour
{
    [SerializeField] private GameObject paradePrefab;
    [SerializeField] private Transform paradePointsParent;
    [SerializeField] private int paradeCount = 3;
    [SerializeField] private float[] followPlayerProbabilities = { 0.8f, 0.5f, 0.3f };
    [SerializeField] private int[] spawnPriorityIndices = { 0, 2, 4 };

    private readonly List<ParadeController> paradeControllers = new();
    private readonly List<GameObject> parades = new();
    private readonly List<PedestrianSpawnpoint> paradePoints = new();
    private Transform player;

    private void Awake()
    {
        paradePoints.AddRange(paradePointsParent.GetComponentsInChildren<PedestrianSpawnpoint>());
    }

    public void Initialize(Transform player, Vector3 destination)
    {
        this.player = player;
        SpawnParades(destination);
    }

    private void SpawnParades(Vector3 destination)
    {
        List<PedestrianSpawnpoint> available = new(paradePoints);
        for (int i = 0; i < paradeCount; i++)
        {
            if (available.Count == 0)
                break;
            PedestrianSpawnpoint spawnPoint = ChooseSpawnPoint(available, spawnPriorityIndices[Mathf.Min(i, spawnPriorityIndices.Length - 1)], destination);
            if (spawnPoint == null)
                break;

            available.Remove(spawnPoint);
            GameObject parade = Instantiate(paradePrefab, spawnPoint.transform.position, Quaternion.identity);
            ParadeController paradeController = parade.GetComponentInChildren<ParadeController>();
            float probability = i < followPlayerProbabilities.Length ? followPlayerProbabilities[i] : 0f;
            paradeController.Initialize(player, paradePoints, spawnPoint, probability);
            paradeControllers.Add(paradeController);
            parades.Add(parade);
        }
    }

    private PedestrianSpawnpoint ChooseSpawnPoint(List<PedestrianSpawnpoint> available, int preferredIndex, Vector3 destination)
    {
        List<PedestrianSpawnpoint> sorted = available
            .OrderBy(p => Vector3.Distance(p.transform.position, destination))
            .ToList();

        preferredIndex = Mathf.Clamp(preferredIndex, 0, sorted.Count - 1);
        return sorted[preferredIndex];
    }

    public ParadeController GetRandomParade()
    {
        if (paradeControllers.Count == 0)
        {
            Debug.Log("count = 0");
            return null;
        }

        int randomIndex = Random.Range(0, paradeControllers.Count);
        return paradeControllers[randomIndex];
    }

    public void Clear()
    {
        paradeControllers.Clear();
        foreach (GameObject parade in parades)
        {
            if (parade != null)
            {
                Destroy(parade);
            }
        }
        parades.Clear();
    }
}
