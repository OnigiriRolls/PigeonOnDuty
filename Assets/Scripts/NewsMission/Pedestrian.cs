using UnityEngine;

[RequireComponent(typeof(PedestrianController))]
[RequireComponent(typeof(ClientController))]
public class Pedestrian : MonoBehaviour
{
    public PedestrianController Controller { get; private set; }
    public ClientController Client { get; private set; }

    private void Awake()
    {
        Controller = GetComponent<PedestrianController>();
        Client = GetComponent<ClientController>();
    }
}
