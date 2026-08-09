using System.Collections;
using UnityEngine;

public class WindCorridorManager : MonoBehaviour
{
    [SerializeField] private WindCorridor prefab;
    [SerializeField] private WindCorridorConfig config;
    [SerializeField] private PlayerController player;
    [SerializeField] private LayerMask obstacleMask;

    private Vector3 corridorSize;

    private void Start()
    {
        BoxCollider box = prefab.GetComponent<BoxCollider>();
        corridorSize = Vector3.Scale(box.size, prefab.transform.localScale) * 0.5f;
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
        const int maxAttempts = 10;
        Vector3 direction = player.Velocity.sqrMagnitude > 1f ? player.Velocity.normalized : player.transform.forward;
        Quaternion rotation = Quaternion.LookRotation(direction);
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 offset = player.transform.right * Random.Range(-config.horizontalOffset, config.horizontalOffset);
            offset += player.transform.up * Random.Range(-config.verticalOffset, config.verticalOffset);
            Vector3 position = player.transform.position + direction * config.spawnDistance + offset;

            bool blocked = Physics.CheckBox(position, corridorSize, rotation, obstacleMask);
            if (!blocked)
            {
                Instantiate(prefab, position, rotation);
                TutorialManager.Instance.TryShow("tutorial_wind_gust", "Wind Corridor", "Try the helpful wind corridor!");
                return;
            }
        }
    }
}
