using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    [Header("Collectibles")]
    [SerializeField] private CollectibleSpawnData[] specialCollectibles;
    [SerializeField] private CollectibleSpawnData coinCollectible;

    [Header("Spawn")]
    [SerializeField] private float spawnRadiusCoins = 25f;
    [SerializeField] private float spawnRadiusSpecials = 15f;
    [SerializeField] private int maxSpecialCollectibles = 2;
    [SerializeField] private int minCoinCount = 3;
    [SerializeField] private int maxCoinCount = 8;
    [SerializeField] private Transform collectibleParent;
    [SerializeField] private LayerMask cloudLayer;
    [SerializeField] private LayerMask buildingLayer;

    public void SpawnCollectibles(Transform checkpoint, Vector3 playerPosition)
    {
        SpawnCoins(checkpoint.position, playerPosition);
        SpawnSpecials(checkpoint.position, playerPosition);
    }

    private void SpawnCoins(Vector3 checkpointPos, Vector3 playerPos)
    {
        int count = Random.Range(minCoinCount, maxCoinCount + 1);
        if (coinCollectible == null)
            return;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = GetSpawnPosition(checkpointPos, playerPos, spawnRadiusCoins);
            Instantiate(coinCollectible.prefab, pos, Quaternion.identity, collectibleParent);
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

            Vector3 pos = GetSpawnPosition(checkpointPos, playerPos, spawnRadiusSpecials);
            Instantiate(collectible.prefab, pos, Quaternion.identity, collectibleParent);
            spawnedCount++;
        }
    }

    private Vector3 GetSpawnPosition(Vector3 checkpointPos, Vector3 playerPos, float spawnRadius)
    {
        const int maxAttempts = 2;
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 center = Vector3.Lerp(playerPos, checkpointPos, Random.Range(0.3f, 0.65f));
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            randomOffset.y *= 0.3f;
            Vector3 spawnPos = center + randomOffset;
            bool insideCloud = Physics.CheckSphere(spawnPos, 5f, cloudLayer);
            bool insideBuilding = Physics.CheckSphere(spawnPos, 5f, buildingLayer);
            if (!insideCloud && !insideBuilding)
                return spawnPos;
        }

        return checkpointPos;
    }
}
