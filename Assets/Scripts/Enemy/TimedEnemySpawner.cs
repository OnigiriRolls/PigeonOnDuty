using UnityEngine;

public class TimedEnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private float minSpawnDelay = 8f;
    [SerializeField] private float maxSpawnDelay = 15f;

    private bool active;
    private bool enemyAlive;
    private float timer;
    private GameObject currentEnemy;

    private void OnEnable()
    {
        spawner.OnEnemyFinished += HandleEnemyFinished;
    }

    private void OnDisable()
    {
        spawner.OnEnemyFinished -= HandleEnemyFinished;
    }

    private void Update()
    {
        if (!active || enemyAlive)
            return;
        timer -= Time.deltaTime;
        if (timer <= 0f)
            SpawnEnemy();
    }

    public void Begin()
    {
        active = true;
        ResetTimer();
    }

    public void StopAndClear()
    {
        active = false;
        Destroy(currentEnemy);
    }

    private void SpawnEnemy()
    {
        enemyAlive = true;
        currentEnemy = spawner.SpawnEnemy();
    }

    private void HandleEnemyFinished()
    {
        enemyAlive = false;
        ResetTimer();
    }

    private void ResetTimer()
    {
        timer = Random.Range(minSpawnDelay, maxSpawnDelay);
    }
}
