using System.Collections.Generic;
using UnityEngine;

public class ClientAssigner : MonoBehaviour
{
    [SerializeField] private PedestrianSpawner pedestrianSpawner;

    public List<ClientController> AssignRandomClients(int clientCount)
    {
        List<ClientController> clients = pedestrianSpawner.GetRandomAvailableClients(clientCount);
        foreach (ClientController client in clients)
            client.BecomeClient();
        return clients;
    }

    public void ClearClients(IEnumerable<ClientController> clients)
    {
        foreach (ClientController client in clients)
            client.ClearClient();
    }
}
