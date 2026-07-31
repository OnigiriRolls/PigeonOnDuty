using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    [Header("Collectibles")]
    [SerializeField] private CollectibleSpawnData[] specialCollectibles;
    [SerializeField] private CollectibleSpawnData coinCollectible;

    [Header("Spawn")]
    [SerializeField] private float spawnRadiusSpecials = 15f;
    [SerializeField] private int maxSpecialCollectibles = 2;
    [SerializeField] private Transform collectibleParent;
    [SerializeField] private LayerMask cloudLayer;
    [SerializeField] private LayerMask buildingLayer;
    [SerializeField] private LayerMask roadLayer;

    [Header("Coins")]
    [SerializeField] private int minCoinCount = 2;
    [SerializeField] private int maxCoinCount = 6;
    [SerializeField] private float coinsSpacing = 5f;
    [SerializeField] private float minCoinSpawnInterval = 20f;
    [SerializeField] private float maxCoinSpawnInterval = 30f;
    [SerializeField] private float coinSpawnDistance = 20f;

    private Coroutine coinRoutine;
    private Coroutine normalMissionRoutine;

    public void SpawnCollectibles(Transform checkpoint, Vector3 playerPosition)
    {
        Vector3 direction = (checkpoint.position - playerPosition).normalized;
        Vector3 startPos = Vector3.Lerp(playerPosition, checkpoint.position, 0.4f);
        int count = Random.Range(minCoinCount, maxCoinCount + 1);
        SpawnCoinLine(startPos, direction, count);
        SpawnSpecials(checkpoint.position, playerPosition);
    }

    private void SpawnCoinLine(Vector3 startPos, Vector3 direction, int count)
    {
        if (coinCollectible == null)
            return;
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = startPos + i * coinsSpacing * direction;
            if (IsObstructed(spawnPos, 3f))
                continue;
            Instantiate(coinCollectible.prefab, spawnPos, Quaternion.identity, collectibleParent);
        }
    }

    private void SpawnSpecials(Vector3 checkpointPos, Vector3 playerPos)
    {
        int spawnedCount = 0;
        foreach (CollectibleSpawnData collectible in specialCollectibles)
        {
            if (spawnedCount >= maxSpecialCollectibles)
                break;
            if (Random.value > collectible.spawnChance)
                continue;

            if (TryGetSpawnPosition(checkpointPos, playerPos, spawnRadiusSpecials, out Vector3 pos))
            {
                Instantiate(collectible.prefab, pos, Quaternion.identity, collectibleParent);
                spawnedCount++;
            }
        }
    }

    public void StartNormalMissionCollectibles()
    {
        StopNormalMissionCollectibles();
        normalMissionRoutine = StartCoroutine(NormalMissionRoutine());
    }

    public void StopNormalMissionCollectibles()
    {
        if (normalMissionRoutine != null)
        {
            StopCoroutine(normalMissionRoutine);
            normalMissionRoutine = null;
        }
    }

    private IEnumerator NormalMissionRoutine()
    {
        float wait = Random.Range(15f, 25f);
        yield return new WaitForSeconds(wait);
        SpawnCollectiblesInFrontOfPlayer();
        normalMissionRoutine = null;
    }

    private void SpawnCollectiblesInFrontOfPlayer()
    {
        Vector3 direction = playerTransform.forward;
        direction.y = 0f;
        direction.Normalize();
        Vector3 center = playerTransform.position + direction * coinSpawnDistance;
        int count = Random.Range(minCoinCount, maxCoinCount + 1);
        SpawnCoinLine(center, direction, count);
        SpawnSpecialsAroundPoint(center);
    }

    private void SpawnSpecialsAroundPoint(Vector3 center)
    {
        int spawnedCount = 0;
        foreach (CollectibleSpawnData collectible in specialCollectibles)
        {
            if (spawnedCount >= maxSpecialCollectibles)
                break;
            if (Random.value > collectible.spawnChance)
                continue;
            if (TryGetSpawnPositionAround(center, spawnRadiusSpecials, out Vector3 pos))
            {
                Instantiate(collectible.prefab, pos, Quaternion.identity, collectibleParent);
                spawnedCount++;
            }
        }
    }

    private bool TryGetSpawnPositionAround(Vector3 center, float radius,  out Vector3 spawnPos)
    {
        const int maxAttempts = 5;
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * radius;
            randomOffset.y *= 0.3f;
            spawnPos = center + randomOffset;
            if (!IsObstructed(spawnPos, 5f))
                return true;
        }
        spawnPos = Vector3.zero;
        return false;
    }

    public void StartContinuousCoinSpawning(Transform player)
    {
        StopContinuousCoinSpawning();
        coinRoutine = StartCoroutine(CoinSpawnRoutine(player));
    }

    private IEnumerator CoinSpawnRoutine(Transform player)
    {
        float wait = 5f;
        while (true)
        {
            yield return new WaitForSeconds(wait);
            SpawnCoinLineInFrontOfPlayer(player);
            wait = Random.Range(minCoinSpawnInterval, maxCoinSpawnInterval);
        }
    }

    public void StopContinuousCoinSpawning()
    {
        if (coinRoutine != null)
        {
            StopCoroutine(coinRoutine);
            coinRoutine = null;
        }
    }

    private void SpawnCoinLineInFrontOfPlayer(Transform player)
    {
        Vector3 direction = player.forward;
        direction.y = 0f;
        direction.Normalize();
        Vector3 startPos = player.position + direction * coinSpawnDistance;
        int count = Random.Range(minCoinCount, maxCoinCount + 1);
        SpawnCoinLine(startPos, direction, count);
    }

    private bool TryGetSpawnPosition(Vector3 checkpointPos, Vector3 playerPos, float spawnRadius, out Vector3 spawnPos)
    {
        const int maxAttempts = 5;
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 center = Vector3.Lerp(playerPos, checkpointPos, Random.Range(0.3f, 0.65f));
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            randomOffset.y *= 0.3f;
            spawnPos = center + randomOffset;
            if (!IsObstructed(spawnPos, 5f))
                return true;
        }
        spawnPos = Vector3.zero;
        return false;
    }

    private bool IsAboveRoad(Vector3 position)
    {
        return Physics.Raycast(position + Vector3.up * 5f, Vector3.down, 8f, roadLayer);
    }

    private bool IsObstructed(Vector3 position, float sphereRadius)
    {
        bool insideCloud = Physics.CheckSphere(position, sphereRadius, cloudLayer);
        bool insideBuilding = Physics.CheckSphere(position, sphereRadius, buildingLayer);
        bool onRoad = IsAboveRoad(position);
        return insideCloud || insideBuilding || onRoad;
    }
}
