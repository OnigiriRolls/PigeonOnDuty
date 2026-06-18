using UnityEngine;

public class ShieldCollectible : CollectibleBase
{
    protected override void Collect(GameObject player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health == null)
            return;

        health.ActivateShield();
    }
}
