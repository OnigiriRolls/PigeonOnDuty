using UnityEngine;

public class BalloonSpawner : EnemySpawner
{
    [SerializeField] private float minAltitude = 51f;
    [SerializeField] private float maxAltitude = 150f;

    public override bool CanSpawn(float altitude)
    {
        return altitude >= minAltitude && altitude <= maxAltitude;
    }

    protected override void InitializeEnemy(GameObject enemy)
    {
        BalloonController controller = enemy.GetComponent<BalloonController>();
        if (!EnemyAggroManager.Instance.TryAcquire(controller))
        {
            Destroy(enemy);
            FinishEnemy();
            return;
        }
        controller.Initialize(player.transform, this);
    }
}
