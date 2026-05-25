using UnityEngine;

public class HelicopterSpawner : EnemySpawner
{
    [SerializeField] private float minAltitude = 151f;

    public override bool CanSpawn(float altitude)
    {
        return altitude >= minAltitude;
    }

    protected override void InitializeEnemy(GameObject enemy)
    {
        HelicopterController controller = enemy.GetComponent<HelicopterController>();
        controller.Initialize(player.transform, spawnPosition, this);
    }
}
