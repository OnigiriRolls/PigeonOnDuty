using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "Game/Missions/Mission Data")]
public abstract class MissionData : ScriptableObject
{
    [Header("General")]
    public string missionName;
    public string description;
    public float timeBuffer = 20f;
    public string[] flavorTexts;
    public MissionCity city;

    [Header("Rewards")]
    public int reputationReward;
    public int coinReward;

    [Header("Unlock")]
    public string unlockId;

    public string GetRandomFlavorText()
    {
        if (flavorTexts == null || flavorTexts.Length == 0)
            return "";
        return flavorTexts[Random.Range(0, flavorTexts.Length)];
    }
}
