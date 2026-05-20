using UnityEngine;

public class HelicopterBullet : MonoBehaviour
{
    [SerializeField] private float speed = 80f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private GameObject hitEffectPrefab;

    private Transform player;
    private Vector3 moveDirection;
    private HelicopterManager helicopterManager;
    private float homingTimer;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector3 direction, HelicopterManager _helicopterManager, Transform targetPlayer)
    {
        helicopterManager = _helicopterManager;
        homingTimer = helicopterManager.HomingDuration;
        moveDirection = direction;
        player = targetPlayer;
    }

    private void Update()
    {
        if (player != null && homingTimer > 0f)
        {
            homingTimer -= Time.deltaTime;
            Vector3 targetDirection = (player.position - transform.position).normalized;
            moveDirection = Vector3.Lerp(moveDirection, targetDirection, helicopterManager.HomingStrength * Time.deltaTime);
            moveDirection.Normalize();
        }
        transform.rotation = Quaternion.LookRotation(moveDirection);
        transform.position += helicopterManager.BulletSpeed * Time.deltaTime * moveDirection;
        Debug.DrawRay(
            transform.position,
            moveDirection * 10f,
            Color.red
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnHitEffect(other);
            Destroy(gameObject);
        }
    }

    private void SpawnHitEffect(Collider playerCollider)
    {
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
        PlayerAudioController audioController = playerCollider.gameObject.GetComponent<PlayerAudioController>();
        if (audioController != null)
        {
            audioController.PlayHitClip();
        }
    }
}
