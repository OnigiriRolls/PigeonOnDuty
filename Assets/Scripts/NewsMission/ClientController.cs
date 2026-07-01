using System;
using UnityEngine;

public class ClientController : MonoBehaviour
{
    public bool IsActiveClient => isActiveClient;
    public event Action<ClientController> OnBecameClient;
    public event Action<ClientController> OnStoppedBeingClient;

    [SerializeField] private bool isActiveClient;

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
