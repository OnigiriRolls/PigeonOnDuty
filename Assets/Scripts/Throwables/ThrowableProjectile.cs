using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class ThrowableProjectile : MonoBehaviour
{
    [SerializeField] protected float lifetime = 10f;

    protected Rigidbody rb;

    protected PickupManager pickupManager;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
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

    protected abstract void HandleImpact(Collision collision);

    public void Initialize(PickupManager manager)
    {
        pickupManager = manager;
    }
}
