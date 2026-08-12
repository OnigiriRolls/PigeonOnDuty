using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private GameObject destroyEffectPrefab;
    [SerializeField] private AudioClip checkpointSound;

    private CheckpointsManager checkpointsManager;

    private void Start()
    {
        checkpointsManager = FindAnyObjectByType<CheckpointsManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        AudioManager.Instance.PlaySFX(checkpointSound);
        if (destroyEffectPrefab != null)
        {
            Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
        }
        checkpointsManager.NotifyCheckpointReached(transform);
        MissionManager.Instance.CompleteActiveMission();
        Destroy(gameObject);
    }
}
