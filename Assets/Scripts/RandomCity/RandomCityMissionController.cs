using System;
using UnityEngine;

public class RandomCityMissionController : MonoBehaviour, IMissionController
{
    public event Action OnMissionCompleted;

    [SerializeField] private CheckpointsManager checkpointsManager;
    [SerializeField] private CollectibleManager collectibleManager;
    [SerializeField] private WindAttackManager windAttackManager;
    [SerializeField] private NormalEnemiesSpawner normalEnemiesSpawner;
    [SerializeField] private HelicopterPatrolManager helicopterManager;

    private void Start()
    {
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
