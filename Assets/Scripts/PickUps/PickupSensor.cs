using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PickupSensor : MonoBehaviour
{
    [SerializeField] private ThrowablePickup pickup;
    [SerializeField] private SpriteRenderer colliderZone;
    [SerializeField] private float pickupDelay = 0.5f;

    private Color lastColor;
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
        lastColor = colliderZone.color;
        colliderZone.color = Color.green;
        receiver.NotifyPickup(pickup);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out IPickupReceiver _))
            return;
        colliderZone.color = lastColor;
    }

    private IEnumerator DelayPickup()
    {
        yield return new WaitForSeconds(pickupDelay);
        sphereCollider.enabled = true;
    }
}
