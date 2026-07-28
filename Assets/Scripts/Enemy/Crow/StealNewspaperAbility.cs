using UnityEngine;

[CreateAssetMenu(fileName = "StealNewspaperAbility", menuName = "Game/Enemies/Steal Newspaper Ability")]
public class StealNewspaperAbility : CrowAbility
{
    [SerializeField] private ThrowableData newspaper;

    public override ThrowableData Execute(PlayerController player)
    {
        ThrowableInventory inventory = player.GetComponentInChildren<ThrowableInventory>();
        if (inventory.TryConsume(newspaper))
            return newspaper;
        return null;
    }
}
