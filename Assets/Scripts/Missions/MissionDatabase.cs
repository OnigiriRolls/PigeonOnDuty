using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Missions/Mission Database")]
public class MissionDatabase : ScriptableObject
{
    public IReadOnlyList<MissionData> Missions => missions;

    [SerializeField] private List<MissionData> missions;

    public MissionData GetByUnlockId(string unlockId)
    {
        foreach (MissionData mission in missions)
        {
            if (mission.unlockId == unlockId)
                return mission;
        }

        return null;
    }
}
