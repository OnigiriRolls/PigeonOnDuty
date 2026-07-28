using UnityEngine;
using UnityEngine.Rendering;

public abstract class BaseCrowController : StopAudio, IEnemyPursuer
{
    public bool CanHitPlayer { get; set; }
    public bool CanTakeDamage { get; set; }
    public int CurrentAttacks { get; set; }
    public CrowConfig Config => config;
    public Transform Player => player;
    public CrowChaseState ChaseState { get; protected set; }
    public CrowAttackState AttackState { get; protected set; }

    [SerializeField] protected GameObject hitEffectPrefab;
    [SerializeField] protected CrowConfig config;
    [SerializeField] private AudioClip[] crowSounds;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float minSoundInterval = 2f;
    [SerializeField] private float maxSoundInterval = 5f;

    protected Transform player;
    protected PlayerController playerController;
    protected CrowState currentState;

    private float soundTimer;
    private bool isDestroyed;
    private bool isActive = true;

    protected virtual void Awake()
    {
        ChaseState = new CrowChaseState(this);
        AttackState = new CrowAttackState(this);
    }

    protected virtual void Update()
    {
        if (!isActive)
            return;
        UpdateCrowSounds();
        currentState?.UpdateState();
    }

    public void ChangeState(CrowState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void InitializePlayer(Transform player)
    {
        this.player = player;
        playerController = player.GetComponent<PlayerController>();
        ResetSoundTimer();
    }

    public virtual void MoveTowards(Vector3 target, float speed)
    {
        Vector3 dir = (target - transform.position).normalized;
        if (dir == Vector3.zero)
            return;

        Quaternion rotation = Quaternion.LookRotation(dir);
        transform.SetPositionAndRotation(
            Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime),
            Quaternion.Slerp(transform.rotation, rotation, config.rotationSpeed * Time.deltaTime));
    }

    public Vector3 GetPredictedPlayerPositionWithOffset(float predictionTime)
    {
        Quaternion yawOnly = Quaternion.Euler(0f, player.eulerAngles.y, 0f);
        Vector3 flatRight = yawOnly * Vector3.right;
        return player.position - flatRight * config.leftOffset + playerController.Velocity * predictionTime;
    }

    public abstract void OnPlayerHit();

    public abstract void OnAttackFinished();

    protected virtual void OnTriggerEnter(Collider other)
    {
        bool isCloud = other.gameObject.layer == LayerMask.NameToLayer("Cloud");
        if ((other.CompareTag("Building") || isCloud) && CanTakeDamage)
        {
            DestroyCrow();
            return;
        }
        if (!CanHitPlayer)
            return;
        if (!other.CompareTag("Player"))
            return;
        OnPlayerHit();
        Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
    }

    public virtual void DestroyCrow()
    {
        isDestroyed = true;
        EnemyAggroManager.Instance.Release(this);
        AudioManager.Instance.PlayRandomSFX(hitClip, audioSource);
        Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void UpdateCrowSounds()
    {
        if (isDestroyed)
            return;
        soundTimer -= Time.deltaTime;
        if (soundTimer > 0f)
            return;
        AudioManager.Instance.PlayRandomSFX(crowSounds, audioSource);
        ResetSoundTimer();
    }

    private void ResetSoundTimer()
    {
        soundTimer = Random.Range(minSoundInterval, maxSoundInterval);
    }

    protected override void HandleGameOver()
    {
        gameObject.SetActive(false);
    }

    internal void Deactivate()
    {
        isActive = false;
    }
}
