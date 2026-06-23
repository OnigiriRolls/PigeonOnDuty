using UnityEngine;

[RequireComponent(typeof(ParadeZoneController))]
public class ParadeZone : MonoBehaviour
{
    private ParadeZoneController parade;
    private bool wasSpawned = false;

    private void Awake()
    {
        parade = GetComponent<ParadeZoneController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || wasSpawned)
            return;
        wasSpawned = true;
        parade.SpawnParade();
    }
}
