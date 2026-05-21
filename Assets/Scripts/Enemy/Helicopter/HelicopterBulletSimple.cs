using UnityEngine;

public class HelicopterBulletSimple : MonoBehaviour
{
    [SerializeField] private float speed = 80f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private AudioClip[] shootClips;

    private HelicopterManager helicopterManager;

    private void Start()
    {
        AudioManager.Instance.PlayRandomSFX(shootClips);
        Destroy(gameObject, lifetime);
    }

    public void Initialize(HelicopterManager _helicopterManager)
    {
        helicopterManager = _helicopterManager;
    }

    private void Update()
    {
        transform.position += helicopterManager.BulletSpeed * Time.deltaTime * transform.forward;
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
        if (playerCollider.gameObject.TryGetComponent<PlayerAudioController>(out var audioController))
        {
            audioController.PlayHitClip();
        }
    }
}
