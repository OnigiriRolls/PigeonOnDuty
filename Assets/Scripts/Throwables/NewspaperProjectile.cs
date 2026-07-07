using UnityEngine;

[RequireComponent(typeof(ThrowablePickup))]
[RequireComponent(typeof(Collider))]
public class NewspaperProjectile : ThrowableProjectile
{
    protected override void HandleImpact(Collision collision)
    {
        Debug.Log($"Newspaper hit {collision.gameObject.name}");
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        ThrowablePickup pickup = GetComponent<ThrowablePickup>();
        pickup.enabled = true;
        pickup.Init(pickupManager);
        GetComponent<Collider>().isTrigger = true;
    }
}
