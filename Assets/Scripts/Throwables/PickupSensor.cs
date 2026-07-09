using UnityEngine;

public class PickupSensor : MonoBehaviour
{
    [SerializeField] private ThrowablePickup pickup;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PickupSensorReceiver receiver))
            return;
        receiver.NotifyPickup(pickup);
    }
}
