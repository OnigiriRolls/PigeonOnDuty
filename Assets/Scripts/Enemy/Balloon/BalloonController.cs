using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BalloonController : StopAudio
{
    [SerializeField] private BalloonConfig config;
    [SerializeField] private AudioClip movementClip;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private AudioClip[] explosionClips;

    private Transform player;
    private Vector3 moveDirection;
    private BalloonSpawner spawner;
    private bool directChase;
    private float driftTimer;
    private AudioSource audioSource;

    public void Initialize(Transform targetPlayer, BalloonSpawner balloonManager)
    {
        player = targetPlayer;
        spawner = balloonManager;
        driftTimer = Random.Range(config.minDriftDuration, config.maxDriftDuration);
    }

    protected override void Start()
    {
        base.Start();
        moveDirection = transform.forward;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = movementClip;
        audioSource.Play();
    }

    private void Update()
    {
        if (player == null)
            return;

        driftTimer -= Time.deltaTime;
        if (driftTimer <= 0f && !directChase)
        {
            directChase = true;
            moveDirection = (player.position - transform.position).normalized;
        }
        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        if (directChase)
        {
            Vector3 chaseDirection = (player.position - transform.position).normalized;
            moveDirection = Vector3.Lerp(moveDirection, chaseDirection, config.steeringStrengthToPlayer * Time.deltaTime);
        }
        else
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer < config.minSafeDistance)
            {
                Vector3 pushAway = (transform.position - player.position).normalized;
                moveDirection = Vector3.Lerp(moveDirection, pushAway, config.steeringStrengthToPlayer * Time.deltaTime);
            }
            else
            {
                float angle = Time.time * config.orbitSpeed;
                Vector3 localOrbitOffset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * config.orbitRadius;
                Vector3 worldOrbitOffset = Quaternion.Euler(0f, player.eulerAngles.y, 0f) * localOrbitOffset;
                Vector3 forwardBias = player.forward * config.forwardOffset;
                Vector3 orbitPoint = player.position + worldOrbitOffset + forwardBias + Vector3.up * config.driftOffset;
                Vector3 toOrbit = (orbitPoint - transform.position).normalized;
                Vector3 targetDirection = toOrbit.normalized;
                moveDirection = Vector3.Lerp(moveDirection, targetDirection, config.steeringStrengthToPlayerDrift * Time.deltaTime);
            }
        }

        moveDirection.Normalize();
        transform.position += config.moveSpeed * Time.deltaTime * moveDirection;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), 3f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isOtherCloud = other.gameObject.layer == LayerMask.NameToLayer("Cloud");
        if (other.CompareTag("Player"))
        {
            Debug.Log($"La final: {directChase}");
        }
        if (other.CompareTag("Player") && directChase)
        {
            other.GetComponent<PlayerHealth>().TakeDamage(1, DeathReason.Balloon);
            Explode();
        }
        else if (other.CompareTag("Building") || isOtherCloud)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        AudioManager.Instance.PlayRandomSFX(explosionClips);
        spawner.FinishEnemy();
        Destroy(gameObject);
    }

    protected override void HandleGameOver()
    {
        Debug.Log("Balloon Game Over");
        gameObject.SetActive(false);
    }
}
