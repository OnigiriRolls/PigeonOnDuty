using System.Collections.Generic;
using UnityEngine;

public class NewsMissionController : MonoBehaviour
{
    public IReadOnlyList<ClientController> ActiveClients => activeClients;

    [SerializeField] private ClientAssigner clientAssigner;
    [SerializeField] private PedestrianSpawner pedestrianSpawner;
    [SerializeField] private ThrowableInventory playerInventory;
    [SerializeField] private ThrowableData newspaperData;

    private List<ClientController> activeClients = new();

    public void StartMission(int minClients, int maxClients, int startingNewspapers)
    {
        pedestrianSpawner.SpawnPedestrians();
        playerInventory.Equip(newspaperData);
        playerInventory.SetAmount(newspaperData, startingNewspapers);
        int clientCount = Random.Range(minClients, maxClients + 1);
        activeClients = clientAssigner.AssignRandomClients(clientCount);
    }

    public void EndMission()
    {
        clientAssigner.ClearClients(activeClients);
        activeClients.Clear();
    }
}
