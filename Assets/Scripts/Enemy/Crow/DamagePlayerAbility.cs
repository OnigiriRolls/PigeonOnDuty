using UnityEngine;

[CreateAssetMenu(fileName = "DamagePlayerAbility", menuName = "Game/Enemies/Damage Player Ability")]
public class DamagePlayerAbility : CrowAbility
{
    public override ThrowableData Execute(PlayerController player)
    {
        player.GetComponent<PlayerHealth>().TakeDamage(1, DeathReason.Crow);
        return null;
    }
}
