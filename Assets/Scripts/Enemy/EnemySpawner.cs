using System;
using UnityEngine;

public abstract class EnemySpawner : MonoBehaviour
{
    public event Action OnEnemyFinished;
    [SerializeField] protected PlayerController player;
    [SerializeField] protected Transform spawnPosition;
    [SerializeField] private GameObject enemyPrefab;

    public GameObject SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition.position, Quaternion.identity);
        InitializeEnemy(enemy);
        return enemy;
    }

    protected abstract void InitializeEnemy(GameObject enemy);

    public abstract bool CanSpawn(float altitude);

    public void FinishEnemy()
    {
        OnEnemyFinished?.Invoke();
    }
}
