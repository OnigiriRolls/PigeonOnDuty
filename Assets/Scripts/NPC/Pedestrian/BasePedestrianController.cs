using UnityEngine;

public interface IBasePedestrianController
{
    ThrowableData CarriedItem { get; }
    void TryCollectPickup(ThrowablePickup pickup);
}
