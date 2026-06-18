using UnityEngine;

public class GameManagerSpeedMode : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private EnemySpawner[] spawners;
    [SerializeField] private float minTimerCooldown = 5f;
    [SerializeField] private float maxTimerCooldown = 10f;

    private bool enemyActive;
    private float timer;

    void Start()
    {
        foreach (EnemySpawner spawner in spawners)
        {
            spawner.OnEnemyFinished += HandleEnemyFinished;
        }
        ResetTimer();
    }

    void Update()
    {
        if (enemyActive)
            return;

        timer -= Time.deltaTime;

        if (timer > 0f)
            return;

        TrySpawnEnemy();
    }

    private void TrySpawnEnemy()
    {
        float altitude = player.transform.position.y;
        foreach (EnemySpawner spawner in spawners)
        {
            if (!spawner.CanSpawn(altitude))
                continue;
            spawner.SpawnEnemy();
            enemyActive = true;
            return;
        }

        ResetTimer();
    }

    public void HandleEnemyFinished()
    {
        enemyActive = false;
        ResetTimer();
    }

    private void ResetTimer()
    {
        timer = Random.Range(minTimerCooldown, maxTimerCooldown);
    }
}
