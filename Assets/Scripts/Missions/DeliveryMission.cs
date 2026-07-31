using UnityEngine;

[CreateAssetMenu(fileName = "DeliveryMission", menuName = "Game/Missions/Delivery Mission")]
public class DeliveryMission : MissionData
{
    public DeliveryModifier modifier;
    public float coinMultiplier = 1f;
    public float timerMultiplier = 1f;
    public float throttleMultiplier = 1f;
    public bool oneHitFail;

    public override void StartMission(MissionManager missionManager)
    {
        missionManager.NormalMissionController.StartMission(missionManager.MissionTimer, timeBuffer, timerMultiplier);
    }

    public override void CompleteMission(MissionManager missionManager)
    {
        missionManager.NormalMissionController.ClearMission();
    }

    public override void FailMission(MissionManager missionManager)
    {
        missionManager.NormalMissionController.ClearMission();
    }
}
