using UnityEngine;

public class ThrowablePickup : MonoBehaviour
{
    public ThrowableData Item => item;
    public bool IsCollected { get; private set; }

    [SerializeField] private ThrowableData item;

    private PickupManager pickupManager;

    public void Collect()
    {
        if (IsCollected)
            return;
        IsCollected = true;
        pickupManager.Unregister(this);
        Destroy(gameObject);
    }

    public void Init(PickupManager manager)
    {
        pickupManager = manager;
        manager.Register(this);
    }

    private void OnDisable()
    {
        if (pickupManager != null)
            pickupManager.Unregister(this);
    }
}
