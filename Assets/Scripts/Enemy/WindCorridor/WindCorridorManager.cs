using System.Collections;
using UnityEngine;

public class WindCorridorManager : MonoBehaviour
{
    [SerializeField] private WindCorridor prefab;
    [SerializeField] private WindCorridorConfig config;
    [SerializeField] private PlayerController player;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float wait = Random.Range(config.minSpawnTime, config.maxSpawnTime);
            yield return new WaitForSeconds(wait);
            SpawnCorridor();
        }
    }

    private void SpawnCorridor()
    {
        Vector3 offset = player.transform.right * Random.Range(-config.horizontalOffset, config.horizontalOffset);
        offset += player.transform.up * Random.Range(-config.verticalOffset, config.verticalOffset);
        Vector3 direction = player.Velocity.sqrMagnitude > 1f ? player.Velocity.normalized : player.transform.forward;
        Vector3 position = player.transform.position + direction * config.spawnDistance + offset;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Instantiate(prefab, position, rotation);
    }
}
