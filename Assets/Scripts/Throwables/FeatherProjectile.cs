using UnityEngine;
using static UnityEditor.Progress;

public class FeatherProjectile : ThrowableProjectile
{
    [SerializeField] private LayerMask groundLayer;

    protected override void HandleImpact(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            return;
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
            BecomePickup();
    }
}
