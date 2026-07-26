using UnityEngine;

public class PlayerPickupReceiver : MonoBehaviour, IPickupReceiver
{
    private PlayerPickupCollector collector;

    private void Awake()
    {
        collector = GetComponentInChildren<PlayerPickupCollector>();
    }

    public void NotifyPickup(ThrowablePickup pickup)
    {
        if (pickup == null)
            return;
        collector.Collect(pickup);
    }
}
