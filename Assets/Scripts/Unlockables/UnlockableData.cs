using UnityEngine;

[CreateAssetMenu(fileName = "UnlockableData", menuName = "Game/Unlocks/UnlockableData")]
public class UnlockableData : ScriptableObject
{
    public string displayName;
    public string id;
    public UnlockableType type;
    public int requiredReputation;
    public bool showUnlockNotification = true;
}
