using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ThrowablePickup))]
[RequireComponent(typeof(Collider))]
public abstract class ThrowableProjectile : MonoBehaviour
{
    [SerializeField] protected float lifetime = 10f;
    [SerializeField] protected GameObject pickupSensor;

    protected Rigidbody rb;
    protected Collider itemCollider;
    protected ThrowablePickup pickup;
    protected bool alreadyPickup;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();
        pickup = GetComponent<ThrowablePickup>();
    }

    protected virtual void FixedUpdate()
    {
        Debug.DrawRay(transform.position, rb.linearVelocity, Color.green);
    }

    public virtual void Launch(Vector3 initialVelocity)
    {
        rb.linearVelocity = initialVelocity;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        HandleImpact(collision);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        HandleTrigger(other);
    }

    protected virtual void HandleTrigger(Collider other)
    {
    }

    protected abstract void HandleImpact(Collision collision);

    protected void BecomePickup()
    {
        if (alreadyPickup)
            return;
        alreadyPickup = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        itemCollider.isTrigger = true;
        pickup.enabled = true;
        if (pickupSensor != null)
            pickupSensor.SetActive(true);
        enabled = false;
    }
}
