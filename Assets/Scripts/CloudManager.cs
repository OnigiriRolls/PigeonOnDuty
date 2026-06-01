using UnityEngine;

public class CloudManager : MonoBehaviour
{
    [Header("Clouds")]
    [SerializeField] private GameObject[] cloudPrefabs;
    [SerializeField] private Transform cloudParent;

    [Header("Spawn Counts")]
    [SerializeField] private int midCloudCount = 50;
    [SerializeField] private int highCloudCount = 80;

    [Header("Ranges")]
    [SerializeField] private Vector2 xRange;
    [SerializeField] private Vector2 zRange;
    [SerializeField] private Vector2 midHeightRange;
    [SerializeField] private Vector2 highHeightRange;

    private void Start()
    {
        SpawnClouds(midCloudCount, midHeightRange);
        SpawnClouds(highCloudCount, highHeightRange);
    }

    private void SpawnClouds(int count, Vector2 heightRange)
    {
        for (int i = 0; i < count; i++)
            SpawnRandomCloud(heightRange);
    }

    private void SpawnRandomCloud(Vector2 heightRange)
    {
        Vector3 pos = new Vector3(
                Random.Range(xRange.x, xRange.y),
                Random.Range(heightRange.x, heightRange.y),
                Random.Range(zRange.x, zRange.y));
        GameObject prefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];
        Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        float scale = Random.Range(0.8f, 1.5f);
        GameObject cloud = Instantiate(prefab, pos, rot, cloudParent);
        cloud.transform.localScale *= scale;
    }
}
