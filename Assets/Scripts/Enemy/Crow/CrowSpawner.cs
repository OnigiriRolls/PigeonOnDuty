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
        CrowController controller = enemy.GetComponent<CrowController>();
        controller.Initialize(player.transform, this, spawnPosition);
    }
}
