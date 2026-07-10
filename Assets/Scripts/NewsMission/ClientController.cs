using System;
using UnityEngine;

public class ClientController : MonoBehaviour
{
    public bool IsActiveClient => isActiveClient;
    public event Action<ClientController> OnBecameClient;
    public event Action<ClientController> OnStoppedBeingClient;

    [SerializeField] private bool isActiveClient;
    [SerializeField] private ThrowableData requestedItem;
    [SerializeField] private PedestrianPickupFeedback pickupFeedback;

    public void Initialize(ThrowableData item)
    {
        requestedItem = item;
    }

    public void HandlePickup(ThrowableData item)
    {
        if (item == requestedItem && IsActiveClient)
        {
            pickupFeedback.PlayCorrect();
            return;
        }
        pickupFeedback.PlayWrong();
    }

    public void BecomeClient()
    {
        if (isActiveClient)
            return;
        isActiveClient = true;
        OnBecameClient?.Invoke(this);
    }

    public void ClearClient()
    {
        if (!isActiveClient)
            return;
        isActiveClient = false;
        OnStoppedBeingClient?.Invoke(this);
    }
}
