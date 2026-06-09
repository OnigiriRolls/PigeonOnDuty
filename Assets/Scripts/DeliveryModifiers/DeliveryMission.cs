using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class DeliveryMission
{
    public string missionName;
    [TextArea] public string description;
    public string[] flavorTexts;
    public DeliveryModifier modifier;
    public float coinMultiplier = 1f;
    public float timerMultiplier = 1f;
    public float throttleMultiplier = 1f;
    public int checkpointReward;
    public bool oneHitFail;
    public bool showThrottleLimitUI;

    public string GetRandomFlavorText()
    {
        if (flavorTexts == null || flavorTexts.Length == 0)
            return "";
        return flavorTexts[Random.Range(0, flavorTexts.Length)];
    }
}
