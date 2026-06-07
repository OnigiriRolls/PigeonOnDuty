using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    [Header("Collectibles")]
    [SerializeField] private CollectibleSpawnData[] specialCollectibles;
    [SerializeField] private CollectibleSpawnData coinCollectible;

    [Header("Spawn")]
    [SerializeField] private float spawnRadiusSpecials = 15f;
    [SerializeField] private int maxSpecialCollectibles = 2;
    [SerializeField] private int minCoinCount = 2;
    [SerializeField] private int maxCoinCount = 6;
    [SerializeField] private Transform collectibleParent;
    [SerializeField] private LayerMask cloudLayer;
    [SerializeField] private LayerMask buildingLayer;
    [SerializeField] private LayerMask roadLayer;

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
        float spacing = 10f;
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = startPos + i * spacing * direction;
            Vector3 offset = Random.insideUnitSphere * 1.5f;
            offset.y *= 0.3f;
            spawnPos += offset;
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
