using UnityEngine;

public class PlayerPickupCollector : MonoBehaviour
{
    [SerializeField] private ThrowableInventory inventory;
    [SerializeField] private AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out ThrowablePickup pickup))
            return;
        if (pickup.enabled)
            Collect(pickup);
    }

    private void Collect(ThrowablePickup pickup)
    {
        inventory.Add(pickup.Item, 1);
        AudioManager.Instance.PlaySFX(pickupSound);
        pickup.ReleaseReservation();
        pickup.Collect();
    }
}
