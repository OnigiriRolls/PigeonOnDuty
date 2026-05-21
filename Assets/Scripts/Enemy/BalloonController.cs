using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BalloonController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float steeringStrengthToPlayer = 2f;
    [SerializeField] private AudioClip movementClip;

    [Header("Drift")]
    [SerializeField] private float driftStrength = 5f;
    [SerializeField] private float driftFrequency = 1f;

    [Header("Explosion")]
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private AudioClip[] explosionClips;

    private Vector3 moveDirection;
    private BalloonManager manager;
    private bool directChase;
    private float driftTimer;
    private AudioSource audioSource;
    private Collider balloonCollider;

    public void Initialize(Transform targetPlayer, BalloonManager balloonManager)
    {
        player = targetPlayer;
        manager = balloonManager;
        driftTimer = Random.Range(manager.minDriftDuration, manager.maxDriftDuration);
    }

    private void Start()
    {
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
            Debug.Log(directChase);
            moveDirection = (player.position - transform.position).normalized;
        }
        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        if (directChase)
        {
            Vector3 chaseDirection = (player.position - transform.position).normalized;
            moveDirection = Vector3.Lerp(moveDirection, chaseDirection, manager.steeringStrengthToPlayer * Time.deltaTime);
        }
        else
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer < manager.minSafeDistance)
            {
                Vector3 pushAway = (transform.position - player.position).normalized;
                moveDirection = Vector3.Lerp(moveDirection, pushAway, manager.steeringStrengthToPlayer * Time.deltaTime);
            }
            else
            {
                float angle = Time.time * manager.orbitSpeed;
                Vector3 localOrbitOffset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * manager.orbitRadius;
                Vector3 worldOrbitOffset = Quaternion.Euler(0f, player.eulerAngles.y, 0f) * localOrbitOffset;
                Vector3 forwardBias = player.forward * manager.forwardOffset;
                Vector3 orbitPoint = player.position + worldOrbitOffset + forwardBias + Vector3.up * manager.driftOffset;
                Vector3 toOrbit = (orbitPoint - transform.position).normalized;
                Vector3 targetDirection = toOrbit.normalized;
                moveDirection = Vector3.Lerp(moveDirection, targetDirection, manager.steeringStrengthToPlayerDrift * Time.deltaTime);
            }
        }

        moveDirection.Normalize();
        transform.position += manager.moveSpeed * Time.deltaTime * moveDirection;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), 3f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"La final: {directChase}");
        }
        if (other.CompareTag("Player") && directChase)
        {
            Explode();
        }
        else if (other.CompareTag("Building"))
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
        manager.BalloonFinished();
        Destroy(gameObject);
    }
}
