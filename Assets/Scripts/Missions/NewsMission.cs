using UnityEngine;

[CreateAssetMenu(fileName = "NewsMission", menuName = "Game/Missions/News Mission")]
public class NewsMission : MissionData
{
    public override void StartMission(MissionManager missionManager)
    {
        missionManager.NewsMissionController.StartMission();
    }

    public override void CompleteMission(MissionManager missionManager)
    {
    }
}
