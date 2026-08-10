using UnityEngine;

[RequireComponent(typeof(HelicopterAudioController))]
public class PatrolHelicopterController : StopAudio
{
    [SerializeField] private HelicopterConfig config;
    [SerializeField] private HelicopterGun[] guns;
    [SerializeField] private float patrolSpeed = 20f;
    [SerializeField] private float patrolReachDistance = 5f;
    [SerializeField] private float attackRadius = 80f;
    [SerializeField] private float chaseDuration = 2f;
    [SerializeField] private float returnDistance = 5f;
    [SerializeField] private float minHeight = 151f;
    [SerializeField] private float maxHeight = 250f;

    private float patrolHeight;
    private Transform player;
    private Transform currentPatrolPoint;
    private Transform previousPatrolPoint;
    private HelicopterPatrolManager manager;
    private float fireTimer;
    private int bulletsShot;
    private float sideOffset;
    private float chaseTimer;
    private HelicopterState currentState;
    private HelicopterAudioController audioController;

    private enum HelicopterState
    {
        Patrol,
        Chase,
        Shooting,
        Return
    }

    protected override void Start()
    {
        base.Start();
        audioController = GetComponent<HelicopterAudioController>();
    }

    public void Initialize(Transform targetPlayer, Transform startingPoint, HelicopterPatrolManager helicopterManager)
    {
        player = targetPlayer;
        currentPatrolPoint = startingPoint;
        manager = helicopterManager;
        sideOffset = Random.Range(-config.sideOffset, config.sideOffset);
        patrolHeight = Random.Range(minHeight, maxHeight);
        foreach (HelicopterGun gun in guns)
        {
            gun.Initialize(player, config);
        }
        ChangeState(HelicopterState.Patrol);
    }

    private void Update()
    {
        switch (currentState)
        {
            case HelicopterState.Patrol:
                HandlePatrol();
                break;
            case HelicopterState.Chase:
                HandleChase();
                break;
            case HelicopterState.Shooting:
                HandleShooting();
                break;
            case HelicopterState.Return:
                HandleReturn();
                break;
        }
    }

    private void ChangeState(HelicopterState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case HelicopterState.Chase:
                TutorialManager.Instance.TryShow("tutorial_enemy_helicopter", "Helicopter Enemy", "Avoid the bullets and survive the attack! Try hiding in clouds...");
                chaseTimer = 0f;
                sideOffset = Random.Range(-config.sideOffset, config.sideOffset);
                break;
            case HelicopterState.Shooting:
                fireTimer = 0f;
                bulletsShot = 0;
                break;
        }
    }

    private void HandlePatrol()
    {
        audioController.PlayFlying();
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= attackRadius)
            {
                ChangeState(HelicopterState.Chase);
                return;
            }
        }

        var currentPosition = GetPatrolPosition(currentPatrolPoint);
        MoveTowards(currentPosition, patrolSpeed);
        float patrolDistance = Vector3.Distance(transform.position, currentPosition);
        if (patrolDistance <= patrolReachDistance)
        {
            PickNextPatrolPoint();
        }
    }

    private void PickNextPatrolPoint()
    {
        previousPatrolPoint = currentPatrolPoint;
        currentPatrolPoint = manager.GetNextPatrolPoint(currentPatrolPoint, previousPatrolPoint);
    }

    private void HandleChase()
    {
        audioController.PlayFlying();
        if (player == null)
        {
            ChangeState(HelicopterState.Return);
            return;
        }
        chaseTimer += Time.deltaTime;
        Vector3 targetPosition = GetAttackPosition();
        MoveTowards(targetPosition, config.moveSpeed);
        if (chaseTimer >= chaseDuration)
        {
            ChangeState(HelicopterState.Shooting);
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
            ChangeState(HelicopterState.Return);
        }
    }

    private void HandleReturn()
    {
        audioController.PlayFlying();
        var currentPosition = GetPatrolPosition(currentPatrolPoint);
        MoveTowards(currentPosition, patrolSpeed);
        float distance = Vector3.Distance(transform.position, currentPosition);
        if (distance <= returnDistance)
        {
            PickNextPatrolPoint();
            ChangeState(HelicopterState.Patrol);
        }
    }

    private Vector3 GetPatrolPosition(Transform point)
    {
        return new Vector3(point.position.x, patrolHeight, point.position.z);
    }

    private Vector3 GetAttackPosition()
    {
        return player.position + player.forward * config.forwardDistance + player.right * sideOffset + Vector3.up * config.upDistance;
    }

    private void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += speed * Time.deltaTime * direction;
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
#endif
}
