using UnityEngine;

public class PlayerPickupCollector : MonoBehaviour
{
    [SerializeField] private ThrowableInventory inventory;
    [SerializeField] private AudioClip pickupSound;

    public void Collect(ThrowablePickup pickup)
    {
        Debug.Log("Player pickup " + pickup.name);
        inventory.Add(pickup.Item, 1);
        AudioManager.Instance.PlaySFX(pickupSound);
        pickup.ReleaseReservation();
        pickup.Collect();
    }
}
