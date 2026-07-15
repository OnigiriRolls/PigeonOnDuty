using UnityEngine;

public abstract class CrowAbility : ScriptableObject
{
    public abstract ThrowableData Execute(PlayerController player);
}
