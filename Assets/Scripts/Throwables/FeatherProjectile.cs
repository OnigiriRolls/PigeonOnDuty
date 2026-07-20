using UnityEngine;
using static UnityEditor.Progress;

public class FeatherProjectile : ThrowableProjectile
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float homingStrength = 20f;

    private IThrowTarget target;

    protected override void HandleImpact(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            return;
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
            BecomePickup();
    }

    protected override void HandleTrigger(Collider other)
    {
        if (other.TryGetComponent(out IThrowTarget target))
        {
            target.OnHit(pickup.Item);
            Destroy(gameObject);
        }
    }

    public void SetTarget(IThrowTarget crow)
    {
        target = crow;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (target == null)
            return;

        //Vector3 aimPoint = target.AimPoint.position +  target.AimPoint.forward * 0.5f;
        Vector3 aimPoint = target.AimPoint.position +  target.AimPoint.forward;
        Vector3 desired = (aimPoint - transform.position).normalized;
        Vector3 current = rb.linearVelocity.normalized;
        Vector3 newDirection = Vector3.RotateTowards(current, desired, homingStrength * Time.fixedDeltaTime, 0f);
        rb.linearVelocity = newDirection * rb.linearVelocity.magnitude;
    }
}
