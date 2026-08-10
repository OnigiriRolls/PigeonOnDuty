using UnityEngine;

public class NormalMissionController : MonoBehaviour
{
    [SerializeField] private CheckpointsManager checkpointsManager;
    [SerializeField] private CollectibleManager collectibleManager;
    [SerializeField] private WindAttackManager windAttackManager;
    [SerializeField] private NormalEnemiesSpawner normalEnemiesSpawner;
    [SerializeField] private HelicopterPatrolManager helicopterManager;

    public void StartMission(MissionTimer missionTimer, float timeBuffer, float timerMultiplier)
    {
        checkpointsManager.SpawnNextCheckpoint();
        collectibleManager.StartNormalMissionCollectibles();
        checkpointsManager.StartCheckpointTimer(missionTimer, timeBuffer, timerMultiplier);
        windAttackManager.StartAttack();
        normalEnemiesSpawner.StartSpawn();
        helicopterManager.SpawnHelicopters();
    }

    public void ClearMission()
    {
        windAttackManager.StopAttack();
        normalEnemiesSpawner.StopSpawn();
        collectibleManager.StopNormalMissionCollectibles();
        helicopterManager.Clear();
    }
}
