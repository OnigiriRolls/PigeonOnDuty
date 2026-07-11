using UnityEngine;

[RequireComponent(typeof(ThrowablePickup))]
[RequireComponent(typeof(Collider))]
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

    private void BecomePickup()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        ThrowablePickup pickup = GetComponent<ThrowablePickup>();
        pickup.enabled = true;
        GetComponent<Collider>().isTrigger = true;
        enabled = false;
    }
}
