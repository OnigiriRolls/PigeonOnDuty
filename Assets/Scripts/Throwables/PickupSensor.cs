using UnityEngine;

public class PickupSensor : MonoBehaviour
{
    [SerializeField] private ThrowablePickup pickup;
    [SerializeField] private SpriteRenderer colliderZone;

    private Color lastColor;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PickupSensorReceiver receiver))
            return;
        lastColor = colliderZone.color;
        colliderZone.color = Color.green;
        receiver.NotifyPickup(pickup);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out PickupSensorReceiver _))
            return;
        colliderZone.color = lastColor;
    }
}
