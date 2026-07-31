using System.Collections.Generic;
using UnityEngine;

public class NormalEnemiesSpawner : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private EnemySpawner[] spawners;
    [SerializeField] private float minTimerCooldown = 5f;
    [SerializeField] private float maxTimerCooldown = 10f;
    [SerializeField] private bool enemyActive;
    [SerializeField] private float timer;

    private readonly List<EnemySpawner> availableSpawners = new();
    private bool isActive;

    void Start()
    {
        isActive = false;
    }

    public void StartSpawn()
    {
        if (isActive)
            return;
        isActive = true;
        foreach (EnemySpawner spawner in spawners)
        {
            spawner.OnEnemyFinished += HandleEnemyFinished;
        }
        ResetTimer();
    }

    void Update()
    {
        if (!isActive)
            return;
        if (enemyActive)
            return;

        timer -= Time.deltaTime;
        if (timer > 0f)
            return;
        TrySpawnEnemy();
    }

    private void TrySpawnEnemy()
    {
        availableSpawners.Clear();
        float altitude = player.transform.position.y;
        foreach (EnemySpawner spawner in spawners)
        {
            if (spawner.CanSpawn(altitude))
                availableSpawners.Add(spawner);
        }

        if (availableSpawners.Count == 0)
        {
            ResetTimer();
            return;
        }

        EnemySpawner selected = availableSpawners[Random.Range(0, availableSpawners.Count)];
        selected.SpawnEnemy();
        enemyActive = true;
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

    public void StopSpawn()
    {
        if (!isActive)
            return;
        isActive = false;
        enemyActive = false;
        foreach (EnemySpawner spawner in spawners)
        {
            spawner.OnEnemyFinished -= HandleEnemyFinished;
        }
    }
}
