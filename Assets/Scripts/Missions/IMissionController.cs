using System;
using UnityEngine;

public interface IMissionController
{
    event Action OnMissionCompleted;
    bool CanHandle(MissionData mission);
    void StartMission(MissionData mission);
    void CompleteMission();
    void FailMission();
}
