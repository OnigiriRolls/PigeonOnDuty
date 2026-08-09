using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private GameObject destroyEffectPrefab;
    [SerializeField] private AudioClip checkpointSound;

    private MissionManager missionManager;
    private CheckpointsManager checkpointsManager;

    private void Start()
    {
        missionManager = FindAnyObjectByType<MissionManager>();
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
        missionManager.CompleteActiveMission();
        Destroy(gameObject);
    }
}
