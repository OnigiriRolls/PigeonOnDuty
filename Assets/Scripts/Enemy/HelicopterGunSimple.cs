using UnityEngine;

public class HelicopterGunSimple : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float rotationSpeed = 25f;

    private Transform player;
    private HelicopterManager manager;
    private Vector3 playerVelocity;

    public void Initialize(Transform targetPlayer, HelicopterManager helicopterManager)
    {
        player = targetPlayer;
        manager = helicopterManager;
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
        Vector3 predictedPosition = player.position + playerVelocity * manager.PredictionPrecision;
        Vector3 direction = (predictedPosition - transform.position).normalized;
        direction += Random.insideUnitSphere * Random.Range(0f, 0.03f);
        direction.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = targetRotation;
    }

    public void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        HelicopterBulletSimple bulletScript = bullet.GetComponent<HelicopterBulletSimple>();
        bulletScript.Initialize(manager);
    }
}
