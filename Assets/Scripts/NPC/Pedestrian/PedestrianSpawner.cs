using System.Collections.Generic;
using UnityEngine;

public class PedestrianSpawner : MonoBehaviour
{
    public IReadOnlyList<Pedestrian> ActivePedestrians => pedestrians;

    [SerializeField] private Pedestrian pedestrianPrefab;
    [SerializeField] private List<PedestrianSpawnpoint> spawnPoints = new();
    [SerializeField] private Transform spawnParent;
    [SerializeField] private Transform pedestriansParent;
    [SerializeField] private int pedestrianCount = 20;
    [SerializeField] private WaypointNetwork waypointNetwork;

    private readonly List<Pedestrian> pedestrians = new();

#if UNITY_EDITOR
    private void OnValidate()
    {
        spawnPoints.Clear();
        spawnPoints.AddRange(spawnParent.GetComponentsInChildren<PedestrianSpawnpoint>());
    }
#endif

    public void SpawnPedestrians()
    {
        pedestrians.Clear();
        for (int i = 0; i < pedestrianCount; i++)
        {
            PedestrianSpawnpoint spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            Pedestrian pedestrian = Instantiate(pedestrianPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation, pedestriansParent);
            pedestrian.Controller.Initialize(waypointNetwork);
            pedestrians.Add(pedestrian);
        }
    }

    public Pedestrian GetRandomPedestrian()
    {
        if (pedestrians.Count == 0)
            return null;
        return pedestrians[Random.Range(0, pedestrians.Count)];
    }

    public ClientController GetRandomAvailableClient()
    {
        List<ClientController> available = new();
        foreach (Pedestrian pedestrian in pedestrians)
        {
            if (!pedestrian.Client.IsActiveClient)
                available.Add(pedestrian.Client);
        }
        if (available.Count == 0)
            return null;
        return available[Random.Range(0, available.Count)];
    }

    public List<ClientController> GetRandomAvailableClients(int count)
    {
        List<ClientController> available = new();
        foreach (Pedestrian pedestrian in pedestrians)
        {
            if (!pedestrian.Client.IsActiveClient)
                available.Add(pedestrian.Client);
        }
        List<ClientController> result = new();
        count = Mathf.Min(count, available.Count);
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, available.Count);
            result.Add(available[index]);
            available.RemoveAt(index);
        }
        return result;
    }

    public void Clear()
    {
        foreach (Pedestrian pedestrian in pedestrians)
        {
            if (pedestrian != null)
                Destroy(pedestrian.gameObject);
        }
        pedestrians.Clear();
    }
}
