using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ThrowablePickup))]
[RequireComponent(typeof(Collider))]
public abstract class ThrowableProjectile : MonoBehaviour
{
    public bool IsPickup => isPickup;

    [SerializeField] protected GameObject pickupSensor;
    [SerializeField] private float homingStrength = 20f;
    [SerializeField] private GameObject projectileHitbox;

    protected Rigidbody rb;
    protected Collider itemCollider;
    protected ThrowablePickup pickup;
    private bool isPickup;
    private IThrowTarget target;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();
        pickup = GetComponent<ThrowablePickup>();
    }

    protected virtual void FixedUpdate()
    {
        if (target == null || target.AimPoint == null)
            return;
        
        Vector3 aimPoint = target.AimPoint.position + target.AimPoint.forward;
        Vector3 desired = (aimPoint - transform.position).normalized;
        Vector3 current = rb.linearVelocity.normalized;
        Vector3 newDirection = Vector3.RotateTowards(current, desired, homingStrength * Time.fixedDeltaTime, 0f);
        rb.linearVelocity = newDirection * rb.linearVelocity.magnitude;
    }

    public void SetTarget(IThrowTarget crow)
    {
        target = crow;
    }

    public virtual void Launch(Vector3 initialVelocity)
    {
        rb.linearVelocity = initialVelocity;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        HandleImpact(collision);
    }

    public virtual void HandleHitboxTrigger(Collider other)
    {
        if (!other.TryGetComponent(out IProjectileTarget target))
            return;
        if (!target.CanBeHitBy(pickup.Item))
            return;

        if (target.OnHit(pickup.Item))
            Destroy(gameObject);
    }

    protected abstract void HandleImpact(Collision collision);

    protected void BecomePickup()
    {
        if (isPickup)
            return;
        isPickup = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        itemCollider.isTrigger = true;
        pickup.enabled = true;
        if (pickupSensor != null)
            pickupSensor.SetActive(true);
        projectileHitbox.SetActive(false);
        PickupManager.Instance.Register(pickup);
        enabled = false;
    }
}
