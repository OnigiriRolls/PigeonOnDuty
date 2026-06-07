using System;
using UnityEngine;

[Serializable]
public class DeliveryMission
{
    public string missionName;
    [TextArea] public string description;
    public DeliveryModifier modifier;
    public float coinMultiplier = 1f;
    public float timerMultiplier = 1f;
    public float throttleMultiplier = 1f;
    public int checkpointReward;
    public bool oneHitFail;
}
