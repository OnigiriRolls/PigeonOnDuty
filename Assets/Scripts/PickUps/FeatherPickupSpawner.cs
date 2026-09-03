using System.Collections;
using UnityEngine;

public class FeatherPickupSpawner : MonoBehaviour
{
    [SerializeField] private ThrowablePickup pickupPrefab;
    [SerializeField] private GameObject sign;
    [SerializeField] private float respawnTime = 10f;
    [SerializeField] private MinimapMissionController minimapController;

    private ThrowablePickup currentPickup;
    private Coroutine respawnRoutine;

    public void StartSpawn()
    {
        sign.SetActive(true);
        minimapController.ShowFeathers(sign.transform);
        if (respawnRoutine != null)
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

    public void StopSpawn()
    {
        sign.SetActive(false);
        minimapController.HideFeathers(sign.transform);
        if (respawnRoutine != null)
        {
            StopCoroutine(respawnRoutine);
            respawnRoutine = null;
        }
        if (currentPickup != null)
            Destroy(currentPickup.gameObject);
    }
}
