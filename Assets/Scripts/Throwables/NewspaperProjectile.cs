using UnityEngine;

[RequireComponent(typeof(ThrowablePickup))]
[RequireComponent(typeof(Collider))]
public class NewspaperProjectile : ThrowableProjectile
{
    protected override void HandleImpact(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            return;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        ThrowablePickup pickup = GetComponent<ThrowablePickup>();
        pickup.enabled = true;
        GetComponent<Collider>().isTrigger = true;
    }
}
