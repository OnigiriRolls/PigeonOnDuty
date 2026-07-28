using System.Collections;
using UnityEngine;

public class FeatherPickupSpawner : MonoBehaviour
{
    [SerializeField] private ThrowablePickup pickupPrefab;
    [SerializeField] private float respawnTime = 10f;

    private ThrowablePickup currentPickup;
    private Coroutine respawnRoutine;

    private void OnEnable()
    {
        if(respawnRoutine  != null)
        {
            StopCoroutine(respawnRoutine);
            respawnRoutine = null;
        }
        SpawnPickup();
    }

    private void SpawnPickup()
    {
        if (currentPickup != null)
            return;
        currentPickup = Instantiate(pickupPrefab, transform.position, Quaternion.identity);
        currentPickup.OnCollected += HandlePickupCollected;
    }

    private void HandlePickupCollected()
    {
        currentPickup.OnCollected -= HandlePickupCollected;
        currentPickup = null;
        if (respawnRoutine != null)
        {
            StopCoroutine(respawnRoutine);
            respawnRoutine = null;
        }
        respawnRoutine = StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnTime);
        SpawnPickup();
    }

    private void OnDisable()
    {
        if (respawnRoutine != null)
        {
            StopCoroutine(respawnRoutine);
            respawnRoutine = null;
        }
    }
}
