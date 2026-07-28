using UnityEngine;

public class PlayerPickupCollector : MonoBehaviour
{
    [SerializeField] private ThrowableInventory inventory;
    [SerializeField] private AudioClip pickupSound;

    public void Collect(ThrowablePickup pickup)
    {
        inventory.Add(pickup.Item, pickup.Amount);
        AudioManager.Instance.PlaySFX(pickupSound);
        pickup.ReleaseReservation();
        pickup.Collect();
    }
}
