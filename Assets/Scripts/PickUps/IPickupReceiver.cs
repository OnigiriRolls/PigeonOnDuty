using UnityEngine;

public interface IPickupReceiver
{
    void NotifyPickup(ThrowablePickup pickup);
}
