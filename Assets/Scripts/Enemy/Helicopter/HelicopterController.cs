using UnityEngine;

[RequireComponent(typeof(HelicopterAudioController))]
public class HelicopterController : StopAudio
{
    [SerializeField] private HelicopterConfig config;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private HelicopterGun[] guns;

    private HelicopterSpawner spawner;
    private Transform player;
    private Transform spawnPosition;
    private float fireTimer;
    private int bulletsShot;
    private float sideOffset;
    private HelicopterState currentState;
    private HelicopterAudioController audioController;

    private enum HelicopterState
    {
        Entering,
        Shooting,
        Leaving
    }

    protected override void Start()
    {
        base.Start();
        audioController = GetComponent<HelicopterAudioController>();
    }

    public void Initialize(Transform targetPlayer, Transform originalSpawn, HelicopterSpawner helicopterManager)
    {
        player = targetPlayer;
        spawnPosition = originalSpawn;
        spawner = helicopterManager;
        sideOffset = Random.Range(-config.sideOffset, config.sideOffset);
        currentState = HelicopterState.Entering;
        foreach (HelicopterGun gun in guns)
        {
            gun.Initialize(player, config);
        }
        TutorialManager.Instance.TryShow("tutorial_enemy_helicopter", "Helicopter Enemy", "Avoid the bullets and survive the attack! Try hiding in clouds...");
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
        if (distance < config.shootingDistance)
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
        fireTimer = config.fireRate;

        Shoot();
        bulletsShot++;
        if (bulletsShot >= config.bulletsToShoot)
        {
            currentState = HelicopterState.Leaving;
        }
    }

    private void HandleLeaving()
    {
        audioController.PlayFlying();
        MoveTowards(spawnPosition.position);
        float distance = Vector3.Distance(transform.position, spawnPosition.position);
        if (distance < 5f)
        {
            spawner.FinishEnemy();
            Destroy(gameObject);
        }
    }

    private void MoveTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        transform.position += config.moveSpeed * Time.deltaTime * dir;
    }

    private Vector3 GetAttackPosition()
    {
        return player.position + player.forward * config.forwardDistance + player.right * sideOffset + Vector3.up * config.upDistance;
    }

    private void Shoot()
    {
        foreach (HelicopterGun gun in guns)
        {
            gun.Shoot();
        }
    }

    protected override void HandleGameOver()
    {
        gameObject.SetActive(false);
    }
}
