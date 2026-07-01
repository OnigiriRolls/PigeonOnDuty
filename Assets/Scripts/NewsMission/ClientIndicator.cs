using UnityEngine;

[RequireComponent(typeof(ClientController))]
public class ClientIndicator : MonoBehaviour
{
    [SerializeField] private GameObject indicatorObject;

    private ClientController client;

    private void Awake()
    {
        client = GetComponent<ClientController>();
        indicatorObject.SetActive(false);
    }

    private void OnEnable()
    {
        client.OnBecameClient += ShowIndicator;
        client.OnStoppedBeingClient += HideIndicator;
    }

    private void OnDisable()
    {
        client.OnBecameClient -= ShowIndicator;
        client.OnStoppedBeingClient -= HideIndicator;
    }

    private void ShowIndicator(ClientController client)
    {
        indicatorObject.SetActive(true);
    }

    private void HideIndicator(ClientController client)
    {
        indicatorObject.SetActive(false);
    }
}
