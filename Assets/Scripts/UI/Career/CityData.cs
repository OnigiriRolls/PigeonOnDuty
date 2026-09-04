using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CityData", menuName = "Game/Cities/City Data")]
public class CityData : ScriptableObject
{
    public string cityId;
    public string displayName;
    public Sprite icon;
    public int unlockReputationRequired;
    public bool released;
    public List<MissionData> missions;
}
