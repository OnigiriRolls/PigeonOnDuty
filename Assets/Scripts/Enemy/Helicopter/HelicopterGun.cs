using UnityEngine;

public class HelicopterGun : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    private Transform player;
    private HelicopterConfig config;
    private Vector3 playerVelocity;

    public void Initialize(Transform targetPlayer, HelicopterConfig helicopterConfig)
    {
        player = targetPlayer;
        config = helicopterConfig;
        playerVelocity = player.GetComponent<PlayerController>().Velocity;
    }

    private void Update()
    {
        if (player == null)
            return;
        AimAtPlayer();
    }

    private void AimAtPlayer()
    {
        Vector3 predictedPosition = player.position + playerVelocity * config.predictionPrecision;
        Vector3 direction = (predictedPosition - transform.position).normalized;
        direction += Random.insideUnitSphere * Random.Range(0f, 0.03f);
        direction.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = targetRotation;
    }

    public void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        HelicopterBullet bulletScript = bullet.GetComponent<HelicopterBullet>();
        bulletScript.Initialize(config);
    }
}
