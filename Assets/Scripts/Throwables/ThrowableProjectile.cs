using UnityEngine;
using static UnityEngine.Rendering.STP;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ThrowablePickup))]
[RequireComponent(typeof(Collider))]
public abstract class ThrowableProjectile : MonoBehaviour
{
    public bool IsPickup => isPickup;

    [SerializeField] protected float lifetime = 10f;
    [SerializeField] protected GameObject pickupSensor;

    protected Rigidbody rb;
    protected Collider itemCollider;
    protected ThrowablePickup pickup;
    private bool isPickup;

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
        enabled = false;
    }
}
