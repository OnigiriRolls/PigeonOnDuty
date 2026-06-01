using UnityEngine;

public class HelicopterBullet : MonoBehaviour
{
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private AudioClip[] shootClips;

    private HelicopterConfig config;

    private void Start()
    {
        AudioManager.Instance.PlayRandomSFX(shootClips);
        Destroy(gameObject, config.lifetime);
    }

    public void Initialize(HelicopterConfig helicopterConfig)
    {
        config = helicopterConfig;
    }

    private void Update()
    {
        transform.position += config.bulletSpeed * Time.deltaTime * transform.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(1);
            SpawnHitEffect(other);
            Destroy(gameObject);
            return;
        }

        bool isOtherCloud = other.gameObject.layer == LayerMask.NameToLayer("Cloud");
        if (isOtherCloud)
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
    }
}
