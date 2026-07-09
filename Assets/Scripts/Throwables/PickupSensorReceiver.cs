using UnityEngine;

[RequireComponent(typeof(PedestrianController))]
public class PickupSensorReceiver : MonoBehaviour
{
    private PedestrianController controller;

    private void Awake()
    {
        controller = GetComponent<PedestrianController>();
    }

    public void NotifyPickup(ThrowablePickup pickup)
    {
        if (pickup == null)
            return;
        controller.TryCollectPickup(pickup);
    }
}
