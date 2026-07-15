using UnityEngine;

public class CrowController : StopAudio, IEnemyPursuer
{
    public bool CanHitPlayer { get; set; }
    public bool CanTakeDamage { get; set; }
    public int CurrentAttacks { get; set; }
    public CrowConfig Config => config;
    public Transform Player => player;
    public CrowStartMode StartMode => startMode;
    public CrowChaseState ChaseState { get; private set; }
    public CrowAttackState AttackState { get; private set; }
    public CrowLeaveState LeaveState { get; private set; }
    public CrowPatrolState PatrolState { get; private set; }
    public CrowReturnState ReturnState { get; private set; }
    public CrowPatrolZone PatrolZone { get; private set; }
    public ThrowableData CarriedItem { get; private set; }

    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private CrowConfig config;
    [SerializeField] private AudioClip[] crowSounds;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float minSoundInterval = 2f;
    [SerializeField] private float maxSoundInterval = 5f;
    [SerializeField] private CarryVisual carryVisual;

    private Transform player;
    private CrowState currentState;
    private PlayerController playerController;
    private CrowSpawner spawner;
    private Transform spawnPosition;
    private float soundTimer;
    private CrowStartMode startMode;
    private bool isDestroyed;

    private void Awake()
    {
        //ChaseState = new CrowChaseState(this);
        //AttackState = new CrowAttackState(this);
        //LeaveState = new CrowLeaveState(this);
        //PatrolState = new CrowPatrolState(this);
        //ReturnState = new CrowReturnState(this);
        isDestroyed = false;
    }

    public void Initialize(Transform playerTransform, CrowSpawner crowSpawner, Transform spawnPosition, CrowStartMode mode, CrowPatrolZone patrolZone = null)
    {
        CanTakeDamage = false;
        spawner = crowSpawner;
        player = playerTransform;
        playerController = player.GetComponent<PlayerController>();
        this.spawnPosition = spawnPosition;
        startMode = mode;
        PatrolZone = patrolZone;
        ResetSoundTimer();
        switch (mode)
        {
            case CrowStartMode.AttackPlayer:
                ChangeState(ChaseState);
                break;

            case CrowStartMode.Patrol:
                ChangeState(PatrolState);
                break;
        }
    }

    private void Update()
    {
        UpdateCrowSounds();
        currentState?.UpdateState();
    }

    public void ChangeState(CrowState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void MoveTowards(Vector3 target, float speed)
    {
        Vector3 dir = (target - transform.position).normalized;
        if (dir == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.SetPositionAndRotation(
            Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime),
            Quaternion.Slerp(transform.rotation, targetRotation, config.rotationSpeed * Time.deltaTime)
        );
    }

    public void MoveTowardsSpawnPositionAndDestroyCrow(float speed)
    {
        MoveTowards(spawnPosition.position, speed);
        float distance = Vector3.Distance(transform.position, spawnPosition.position);
        if (distance < 5f)
        {
            spawner.FinishEnemy();
            Destroy(gameObject);
        }
    }

    public Vector3 GetPredictedPlayerPositionWithOffset(float predictionTime)
    {
        Quaternion yawOnly = Quaternion.Euler(0f, player.eulerAngles.y, 0f);
        Vector3 flatRight = yawOnly * Vector3.right;
        return player.position - flatRight * config.leftOffset + playerController.Velocity * predictionTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isOtherCloud = other.gameObject.layer == LayerMask.NameToLayer("Cloud");
        if ((other.CompareTag("Building") || isOtherCloud) && CanTakeDamage)
        {
            DestroyCrow();
        }
        if (!CanHitPlayer) return;
        if (other.CompareTag("Player"))
        {
            ThrowableData throwable = config.ability.Execute(playerController);
            if (throwable != null)
                PickUp(throwable);
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    private void UpdateCrowSounds()
    {
        if (isDestroyed)
            return;
        soundTimer -= Time.deltaTime;
        if (soundTimer > 0f)
            return;
        PlayCrowSound();
        ResetSoundTimer();
    }

    private void PlayCrowSound()
    {
        AudioManager.Instance.PlayRandomSFX(crowSounds, audioSource);
    }

    private void ResetSoundTimer()
    {
        soundTimer = Random.Range(minSoundInterval, maxSoundInterval);
    }

    public void DestroyCrow()
    {
        isDestroyed = true;
        EnemyAggroManager.Instance.Release(this);
        if (startMode == CrowStartMode.AttackPlayer)
            spawner.FinishEnemy();
        AudioManager.Instance.PlayRandomSFX(hitClip, audioSource);
        Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    protected override void HandleGameOver()
    {
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        if (PatrolZone == null)
            return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(PatrolZone.Center, PatrolZone.Radius);
    }

    public void PickUp(ThrowableData item)
    {
        CarriedItem = item;
        carryVisual.Show(item);
    }

    public void DropItem()
    {
        if (CarriedItem == null)
            return;

        Instantiate(CarriedItem.pickupPrefab, transform.position, Quaternion.identity);
        carryVisual.Hide();
        CarriedItem = null;
    }
}
