using UnityEngine;

public class CrowSpawner : EnemySpawner
{
    [SerializeField] private float maxAltitude = 50f;

    public override bool CanSpawn(float altitude)
    {
        return altitude <= maxAltitude;
    }

    protected override void InitializeEnemy(GameObject enemy)
    {
        AttackCrowController controller = enemy.GetComponent<AttackCrowController>();
        controller.Initialize(player.transform, this, spawnPosition);
    }
}
