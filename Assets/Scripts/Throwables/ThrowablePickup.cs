using UnityEngine;

public class ThrowablePickup : MonoBehaviour
{
    public ThrowableData Item => item;
    public bool IsCollected { get; private set; }
    public bool IsReserved { get; private set; }

    [SerializeField] private ThrowableData item;

    public void Collect()
    {
        if (IsCollected)
            return;
        IsCollected = true;
        Destroy(gameObject);
    }

    public bool TryReserve()
    {
        if (IsReserved)
            return false;

        IsReserved = true;
        return true;
    }

    public void ReleaseReservation()
    {
        IsReserved = false;
    }
}
