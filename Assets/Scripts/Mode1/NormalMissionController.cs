using System;
using UnityEngine;

public class NormalMissionController : MonoBehaviour, IMissionController
{
    public event Action OnMissionCompleted;

    [SerializeField] private CheckpointsManager checkpointsManager;
    [SerializeField] private CollectibleManager collectibleManager;
    [SerializeField] private WindAttackManager windAttackManager;
    [SerializeField] private NormalEnemiesSpawner normalEnemiesSpawner;
    [SerializeField] private HelicopterPatrolManager helicopterManager;

    private void Start()
    {
        MissionManager.Instance.RegisterController(this);
    }

    public void StartMission(float timeBuffer, float timerMultiplier)
    {
        checkpointsManager.SpawnNextCheckpoint();
        collectibleManager.StartNormalMissionCollectibles();
        checkpointsManager.StartCheckpointTimer(timeBuffer, timerMultiplier);
        windAttackManager.StartAttack();
        normalEnemiesSpawner.StartSpawn();
        helicopterManager.SpawnHelicopters();
    }

    public void ClearMission()
    {
        MissionTimer.Instance.StopTimer();
        windAttackManager.StopAttack();
        normalEnemiesSpawner.StopSpawn();
        collectibleManager.StopNormalMissionCollectibles();
        helicopterManager.Clear();
    }

    public bool CanHandle(MissionData mission)
    {
        return mission is DeliveryMission;
    }

    public void StartMission(MissionData mission)
    {
        if (mission is not DeliveryMission deliveryMission)
            return;
        StartMission(deliveryMission.timeBuffer, deliveryMission.timerMultiplier);
    }

    private void OnDestroy()
    {
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
