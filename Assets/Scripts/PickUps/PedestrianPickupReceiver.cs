using UnityEngine;

[RequireComponent(typeof(PedestrianController))]
public class PedestrianPickupReceiver : MonoBehaviour, IPickupReceiver
{
    private PedestrianController controller;

    private void Awake()
    {
        controller = GetComponent<PedestrianController>();
    }

    public void NotifyPickup(ThrowablePickup pickup)
    {
        if (controller.CarriedItem != null)
            return;
        if (pickup == null)
            return;
        controller.TryCollectPickup(pickup);
    }
}
