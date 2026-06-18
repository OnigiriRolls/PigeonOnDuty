using UnityEngine;

[CreateAssetMenu(fileName = "GPSMission", menuName = "Game/Missions/GPS Mission")]
public class GPSMission : MissionData
{
    [Header("GPS")]
    public float maxDistance = 50f;
    public float lostTime = 5f;

    public override void StartMission(MissionManager missionManager)
    {
        missionManager.GPSMissionController.StartMission(this);
    }

    public override void CompleteMission(MissionManager missionManager)
    {
        missionManager.GPSMissionController.StopMission();
    }
}
