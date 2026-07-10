using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NewsMissionController : MonoBehaviour
{
    public IReadOnlyList<ClientController> ActiveClients => activeClients;
    public event Action OnMissionCompleted;

    [SerializeField] private ClientAssigner clientAssigner;
    [SerializeField] private PedestrianSpawner pedestrianSpawner;
    [SerializeField] private ThrowableInventory playerInventory;
    [SerializeField] private ThrowableData newspaperData;
    [SerializeField] private MinimapIconManager minimapIconManager;
    [SerializeField] private MinimapIcon clientIconPrefab;

    private List<ClientController> activeClients = new();

    public void StartMission(int minClients, int maxClients, int startingNewspapers)
    {
        pedestrianSpawner.SpawnPedestrians();
        playerInventory.Equip(newspaperData);
        playerInventory.SetAmount(newspaperData, startingNewspapers);
        int clientCount = Random.Range(minClients, maxClients + 1);
        activeClients = clientAssigner.AssignRandomClients(clientCount);
        foreach (ClientController client in activeClients)
        {
            client.Initialize(newspaperData);
            minimapIconManager.CreateIcon(clientIconPrefab, client.transform);
            client.OnStoppedBeingClient += HandleClientDelivered;
        }
    }

    private void HandleClientDelivered(ClientController client)
    {
        client.OnStoppedBeingClient -= HandleClientDelivered;
        activeClients.Remove(client);
        minimapIconManager.RemoveIcon(client.transform);
        if (activeClients.Count == 0)
            OnMissionCompleted?.Invoke();
    }

    public void EndMission()
    {
        clientAssigner.ClearClients(activeClients);
        activeClients.Clear();
    }
}
