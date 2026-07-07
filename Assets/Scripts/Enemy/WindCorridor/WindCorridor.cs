using UnityEngine;

public class WindCorridor : MonoBehaviour
{
    [SerializeField] private WindCorridorConfig config;

    private void Start()
    {
        Destroy(gameObject, config.lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController player))
            return;
        player.StartWindAssist(transform.forward, Mathf.Infinity, 55f, 2f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController player))
            return;
        player.StopWindCarry();
    }
}
