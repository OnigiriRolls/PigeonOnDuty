using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    public IReadOnlyList<ThrowablePickup> Pickups => pickups;

    private readonly List<ThrowablePickup> pickups = new();

    public void Register(ThrowablePickup pickup)
    {
        if (!pickups.Contains(pickup))
            pickups.Add(pickup);
    }

    public void Unregister(ThrowablePickup pickup)
    {
        pickups.Remove(pickup);
    }
}
