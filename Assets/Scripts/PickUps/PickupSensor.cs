using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PickupSensor : MonoBehaviour
{
    [SerializeField] private ThrowablePickup pickup;
    [SerializeField] private float pickupDelay = 0.5f;

    private Collider sphereCollider;

    private void OnEnable()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.enabled = false;
        StartCoroutine(DelayPickup());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IPickupReceiver receiver))
            return;
        receiver.NotifyPickup(pickup);
    }

    private IEnumerator DelayPickup()
    {
        yield return new WaitForSeconds(pickupDelay);
        sphereCollider.enabled = true;
    }
}
