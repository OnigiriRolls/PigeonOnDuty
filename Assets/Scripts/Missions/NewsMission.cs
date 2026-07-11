using UnityEngine;

[CreateAssetMenu(fileName = "NewsMission", menuName = "Game/Missions/News Mission")]
public class NewsMission : MissionData
{
    [Header("NEWS")]
    public int startingNewspapers = 10;
    public int minClients = 3;
    public int maxClients = 4;

    public override void StartMission(MissionManager missionManager)
    {
        missionManager.NewsMissionController.StartMission(this);
    }

    public override void CompleteMission(MissionManager missionManager)
    {
        missionManager.NewsMissionController.ClearMission();
    }

    public override void FailMission(MissionManager missionManager)
    {
        Debug.Log("News mission failed.");
        missionManager.NewsMissionController.ClearMission();
    }
}
