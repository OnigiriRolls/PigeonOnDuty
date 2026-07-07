using UnityEngine;

[CreateAssetMenu(fileName = "NewsMission", menuName = "Game/Missions/News Mission")]
public class NewsMission : MissionData
{
    [Header("NEWS")]
    public int minClients = 3;
    public int maxClients = 4;

    public override void StartMission(MissionManager missionManager)
    {
        missionManager.NewsMissionController.StartMission(minClients, maxClients);
    }

    public override void CompleteMission(MissionManager missionManager)
    {
    }
}
