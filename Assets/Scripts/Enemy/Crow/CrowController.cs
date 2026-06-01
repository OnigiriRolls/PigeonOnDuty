using UnityEngine;

public class CrowController : MonoBehaviour
{
    public bool CanHitPlayer { get; set; }
    public bool CanTakeDamage { get; set; }
    public int CurrentAttacks { get; set; }
    public CrowConfig Config => config;
    public Transform Player => player;

    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private CrowConfig config;
    [SerializeField] private AudioClip[] crowSounds;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private float minSoundInterval = 2f;
    [SerializeField] private float maxSoundInterval = 5f;

    private Transform player;
    private CrowState currentState;
    private PlayerController playerController;
    private CrowSpawner spawner;
    private Transform spawnPosition;
    private float soundTimer;

    public void Initialize(Transform playerTransform, CrowSpawner crowManager, Transform spawnPosition)
    {
        CanTakeDamage = false;
        spawner = crowManager;
        player = playerTransform;
        playerController = player.GetComponent<PlayerController>();
        this.spawnPosition = spawnPosition;
        ResetSoundTimer();
        ChangeState(new CrowChaseState(this));
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

    public Vector3 GetPredictedPlayerPositionWithOffset1(float predictionTime)
    {
        Quaternion yawOnly = Quaternion.Euler(0f, player.eulerAngles.y, 0f);
        Vector3 flatRight = yawOnly * Vector3.right;
        return player.position + flatRight * 5f + playerController.Velocity * predictionTime;
    }


    public Vector3 GetPredictedPlayerPosition(float predictionTime)
    {
        return player.position + playerController.Velocity * predictionTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isOtherCloud = other.gameObject.layer == LayerMask.NameToLayer("Cloud");
        Debug.Log($"Crow {other.gameObject.layer} {CanTakeDamage}");
        if ((other.CompareTag("Building") || isOtherCloud) && CanTakeDamage)
        {
            DestroyCrow();
        }
        if (!CanHitPlayer) return;
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(1);
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    private void UpdateCrowSounds()
    {
        soundTimer -= Time.deltaTime;
        if (soundTimer > 0f)
            return;
        PlayCrowSound();
        ResetSoundTimer();
    }

    private void PlayCrowSound()
    {
        AudioManager.Instance.PlayRandomSFX(crowSounds);
    }

    private void ResetSoundTimer()
    {
        soundTimer = Random.Range(minSoundInterval, maxSoundInterval);
    }

    public void DestroyCrow()
    {
        spawner.FinishEnemy();
        AudioManager.Instance.PlaySFX(hitClip);
        Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
