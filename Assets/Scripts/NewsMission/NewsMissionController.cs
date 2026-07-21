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
    [SerializeField] private TimedEnemySpawner balloonSpawner;
    [SerializeField] private CrowPatrolManager crowSpawner;
    [SerializeField] private DogPatrolManager dogSpawner;
    [SerializeField] private ThrowableInventory playerInventory;
    [SerializeField] private ThrowableData newspaperData;
    [SerializeField] private ThrowableData featherData;
    [SerializeField] private MinimapIconManager minimapIconManager;
    [SerializeField] private MinimapIcon clientIconPrefab;
    [SerializeField] private TravelTimeCalculator travelTimeCalculator;
    [SerializeField] private MissionTimer missionTimer;
    [SerializeField] private Transform player;

    private List<ClientController> activeClients = new();
    private NewsMission currentMission;

    public void StartMission(NewsMission newsMission)
    {
        currentMission = newsMission;
        pedestrianSpawner.SpawnPedestrians();
        balloonSpawner.Begin();
        crowSpawner.SpawnCrowZones();
        dogSpawner.GenerateDogs();
        int clientCount = Random.Range(currentMission.minClients, currentMission.maxClients + 1);
        activeClients = clientAssigner.AssignRandomClients(clientCount);
        foreach (ClientController client in activeClients)
        {
            client.Initialize(newspaperData);
            minimapIconManager.CreateIcon(clientIconPrefab, client.transform);
            client.OnDeliveryCompleted += HandleClientDelivered;
        }
        Debug.Log(activeClients.Count);
        playerInventory.SetAmount(newspaperData, activeClients.Count + 1000);
        playerInventory.SetAmount(featherData, activeClients.Count + 1000);
        StartTimerForClosestClient();
    }

    private void HandleClientDelivered(ClientController client)
    {
        client.OnDeliveryCompleted -= HandleClientDelivered;
        activeClients.Remove(client);
        minimapIconManager.RemoveIcon(client.transform);
        if (activeClients.Count == 0)
            OnMissionCompleted?.Invoke();
        StartTimerForClosestClient();
    }

    public void ClearMission()
    {
        foreach (ClientController client in activeClients)
        {
            client.OnDeliveryCompleted -= HandleClientDelivered;
        }
        clientAssigner.ClearClients(activeClients);
        activeClients.Clear();
        balloonSpawner.Stop();
    }

    private ClientController GetClosestClient()
    {
        ClientController closest = null;
        float closestDistance = float.MaxValue;
        foreach (ClientController client in activeClients)
        {
            float distance = Vector3.Distance(player.position, client.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = client;
            }
        }

        return closest;
    }

    private void StartTimerForClosestClient()
    {
        ClientController closest = GetClosestClient();
        if (closest == null)
            return;
        float duration = travelTimeCalculator.CalculateTime(closest.transform, currentMission.timeBuffer);
        missionTimer.StartTimer(duration);
    }
}
