using UnityEngine;

[RequireComponent(typeof(ThrowablePickup))]
public class NewspaperProjectile : ThrowableProjectile
{
    [SerializeField] private LayerMask groundLayer;

    protected override void HandleImpact(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            return;
        if (((1 << collision.gameObject.layer) & groundLayer) == 0)
            return;
        BecomePickup();
    }
}
