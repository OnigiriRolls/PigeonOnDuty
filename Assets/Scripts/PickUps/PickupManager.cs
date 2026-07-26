using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    public static PickupManager Instance { get; private set; }

    private readonly List<ThrowablePickup> pickups = new();

    private void Awake()
    {
        Instance = this;
    }

    public void Register(ThrowablePickup pickup)
    {
        if (!pickups.Contains(pickup))
            pickups.Add(pickup);
    }

    public void Unregister(ThrowablePickup pickup)
    {
        pickups.Remove(pickup);
    }

    public ThrowablePickup GetClosestPickup(PatrolZone zone, ThrowableData data)
    {
        ThrowablePickup closest = null;
        float bestDistance = float.MaxValue;
        pickups.RemoveAll(p => p == null);
        foreach (ThrowablePickup pickup in pickups)
        {
            if (pickup.Item != data)
                continue;
            if (!pickup.CanBeCollected)
                continue;
            if (!zone.Contains(pickup.transform.position))
                continue;

            float distance = (pickup.transform.position - zone.transform.position).sqrMagnitude;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                closest = pickup;
            }
        }
        return closest;
    }
}
