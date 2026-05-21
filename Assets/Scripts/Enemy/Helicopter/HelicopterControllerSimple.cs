using UnityEngine;

[RequireComponent(typeof(HelicopterAudioController))]
public class HelicopterControllerSimple : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float rotationSpeed = 3f;

    [Header("Combat")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private HelicopterGunSimple[] guns;
    [SerializeField] private float fireRate = 0.3f;
    [SerializeField] private int bulletsToShoot = 20;

    [SerializeField] private Transform player;
    private Vector3 spawnPosition;
    [SerializeField] private HelicopterManager manager;
    private int bulletsShot;
    private float fireTimer;
    private float sideOffset;
    private HelicopterState currentState;
    private HelicopterAudioController audioController;

    private enum HelicopterState
    {
        Entering,
        Shooting,
        Leaving
    }

    private void Start()
    {
        audioController = GetComponent<HelicopterAudioController>();
    }

    public void Initialize(Transform targetPlayer, Vector3 originalSpawn, HelicopterManager helicopterManager)
    {
        player = targetPlayer;
        spawnPosition = originalSpawn;
        manager = helicopterManager;
        sideOffset = Random.Range(-15f, 15f);
        currentState = HelicopterState.Entering;
        foreach (HelicopterGunSimple gun in guns)
        {
            gun.Initialize(player, manager);
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case HelicopterState.Entering:
                HandleEntering();
                break;
            case HelicopterState.Shooting:
                HandleShooting();
                break;
            case HelicopterState.Leaving:
                HandleLeaving();
                break;
        }
    }

    private void HandleEntering()
    {
        audioController.PlayFlying();
        Vector3 attackPosition = GetAttackPosition();
        MoveTowards(attackPosition);
        float distance = Vector3.Distance(transform.position, attackPosition);
        if (distance < 5f)
        {
            currentState = HelicopterState.Shooting;
        }
    }

    private void HandleShooting()
    {
        audioController.PlayShooting();
        fireTimer -= Time.deltaTime;
        if (fireTimer > 0f)
            return;
        fireTimer = fireRate;

        Shoot();
        bulletsShot++;
        if (bulletsShot >= bulletsToShoot)
        {
            currentState = HelicopterState.Leaving;
        }
    }

    private void HandleLeaving()
    {
        audioController.PlayFlying();
        MoveTowards(spawnPosition);
        float distance = Vector3.Distance(transform.position, spawnPosition);
        if (distance < 10f)
        {
            manager.HelicopterFinished();
            Destroy(gameObject);
        }
    }

    private void MoveTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        transform.position += moveSpeed * Time.deltaTime * dir;
    }

    private Vector3 GetAttackPosition()
    {
        return player.position + player.forward * manager.ForwardDistance + player.right * sideOffset + Vector3.up * manager.UpDistance;
    }

    private void Shoot()
    {
        foreach (HelicopterGunSimple gun in guns)
        {
            gun.Shoot();
        }
    }
}
