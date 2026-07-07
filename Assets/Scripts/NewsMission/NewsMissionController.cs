using System.Collections.Generic;
using UnityEngine;

public class NewsMissionController : MonoBehaviour
{
    public IReadOnlyList<ClientController> ActiveClients => activeClients;

    [SerializeField] private ClientAssigner clientAssigner;
    [SerializeField] private PedestrianSpawner pedestrianSpawner;

    private List<ClientController> activeClients = new();

    public void StartMission(int minClients, int maxClients)
    {
        pedestrianSpawner.SpawnPedestrians();
        int clientCount = Random.Range(minClients, maxClients + 1);
        activeClients = clientAssigner.AssignRandomClients(clientCount);
    }

    public void EndMission()
    {
        clientAssigner.ClearClients(activeClients);
        activeClients.Clear();
    }
}
