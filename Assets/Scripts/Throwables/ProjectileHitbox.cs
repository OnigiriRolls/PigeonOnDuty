using UnityEngine;

public class ProjectileHitbox : MonoBehaviour
{
    [SerializeField] private ThrowableProjectile projectile;

    private void OnTriggerEnter(Collider other)
    {
        projectile.HandleHitboxTrigger(other);
    }
}
