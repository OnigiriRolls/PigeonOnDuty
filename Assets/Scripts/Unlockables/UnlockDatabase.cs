using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockDatabase", menuName = "Game/Unlocks/UnlockDatabase")]
public class UnlockDatabase : ScriptableObject
{
    [SerializeField] private List<UnlockableData> unlockables;
    public List<UnlockableData> Unlockables => unlockables;
}
