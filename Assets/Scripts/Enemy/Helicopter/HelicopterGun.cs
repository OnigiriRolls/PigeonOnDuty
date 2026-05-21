using UnityEngine;

public class HelicopterGun : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;

    public void Shoot(Transform player, HelicopterManager manager)
    {
        Vector3 predictedPosition = player.position + player.GetComponent<PlayerController>().Velocity * manager.PredictionPrecision;
        Vector3 direction = (predictedPosition - transform.position).normalized;
        
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.LookRotation(direction));
        HelicopterBullet helicopterBullet = bullet.GetComponent<HelicopterBullet>();
        helicopterBullet.Initialize(direction, manager, player);
    }
}
