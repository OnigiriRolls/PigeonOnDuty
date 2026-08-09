using UnityEngine;

[RequireComponent(typeof(IBasePedestrianController))]
public class PedestrianPickupReceiver : MonoBehaviour, IPickupReceiver
{
    private IBasePedestrianController controller;

    private void Awake()
    {
        controller = GetComponent<IBasePedestrianController>();
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
