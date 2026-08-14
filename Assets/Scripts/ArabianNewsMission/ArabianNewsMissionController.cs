using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArabianNewsMissionController : MonoBehaviour, IMissionController
{
    public event Action OnMissionCompleted;

    [SerializeField] private ClientAssigner clientAssigner;
    [SerializeField] private PedestrianSpawner pedestrianSpawner;
    [SerializeField] private TimedEnemySpawner balloonSpawner;
    [SerializeField] private CrowPatrolManager crowSpawner;
    [SerializeField] private DogPatrolManager dogSpawner;
    [SerializeField] private FeatherPickupSpawner featherSpawner;
    [SerializeField] private WindAttackManager windAttackManager;
    [SerializeField] private ThrowableInventory playerInventory;
    [SerializeField] private ThrowableData newspaperData;
    [SerializeField] private ThrowableData featherData;
    [SerializeField] private MinimapMissionController minimapController;
    [SerializeField] private TravelTimeCalculator travelTimeCalculator;
    [SerializeField] private Transform player;
    [SerializeField] private CollectibleManager collectibleManager;

    private List<ClientController> activeClients = new();
    private ArabianNewsMission currentMission;
    private PlayerHydration playerHydration;

    private void Start()
    {
        playerHydration = player.GetComponent<PlayerHydration>();
        if (MissionManager.Instance.StartMissionAfterLoad)
        {
            MissionManager.Instance.RegisterController(this);
        }
        else
        {
            MissionManager.Instance.RegisterController(this);
            MissionManager.Instance.RequestMissionSelection();
        }
    }

    public void StartMission(ArabianNewsMission newsMission)
    {
        currentMission = newsMission;
        pedestrianSpawner.SpawnPedestrians();
        balloonSpawner.Begin();
        crowSpawner.SpawnCrowZones();
        dogSpawner.GenerateDogs();
        featherSpawner.StartSpawn();
        int clientCount = Random.Range(currentMission.minClients, currentMission.maxClients + 1);
        activeClients = clientAssigner.AssignRandomClients(clientCount);
        foreach (ClientController client in activeClients)
        {
            client.Initialize(newspaperData);
            minimapController.ShowClient(client.transform);
            client.OnDeliveryCompleted += HandleClientDelivered;
        }
        playerInventory.SetAmount(newspaperData, activeClients.Count + 1);
        playerInventory.SetAmount(featherData, activeClients.Count);
        playerInventory.SetEnabled(newspaperData, true);
        playerInventory.SetEnabled(featherData, true);
        playerHydration.Activate();
        TutorialManager.Instance.TryShow("tutorial_fountain_hydration", "Stay Hydrated", "The sun is scorching... Use the wells to stay hydrated!");
        MissionTimer.Instance.StartTimer(activeClients.Count * currentMission.timeBuffer);
        collectibleManager.StartContinuousCoinSpawning(player);
        windAttackManager.StartAttack();
    }

    private void HandleClientDelivered(ClientController client)
    {
        client.OnDeliveryCompleted -= HandleClientDelivered;
        activeClients.Remove(client);
        minimapController.HideClient(client.transform);
        if (activeClients.Count == 0)
            OnMissionCompleted?.Invoke();
    }

    public void ClearMission()
    {
        foreach (ClientController client in activeClients)
        {
            client.OnDeliveryCompleted -= HandleClientDelivered;
        }
        clientAssigner.ClearClients(activeClients);
        activeClients.Clear();
        balloonSpawner.StopAndClear();
        pedestrianSpawner.Clear();
        crowSpawner.Clear();
        dogSpawner.ClearDogs();
        dogSpawner.ClearPatrolZones();
        featherSpawner.StopSpawn();
        windAttackManager.StopAttack();
        collectibleManager.StopContinuousCoinSpawning();
        playerInventory.SetEnabled(newspaperData, false);
        playerInventory.SetEnabled(featherData, false);
        playerHydration.Deactivate();
    }

    public bool CanHandle(MissionData mission)
    {
        return mission is ArabianNewsMission;
    }

    public void StartMission(MissionData mission)
    {
        if (mission is not ArabianNewsMission newsMission)
            return;
        StartMission(newsMission);
    }

    private void OnDestroy()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.UnregisterController(this);
    }

    public void CompleteMission()
    {
        ClearMission();
    }

    public void FailMission()
    {
        ClearMission();
    }
}
