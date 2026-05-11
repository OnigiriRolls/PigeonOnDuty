using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    [SerializeField] private GameObject destroyEffectPrefab;
    [SerializeField] private AudioClip checkpointSound;

    private EndlessRunManager endlessRunManager;

    public void Initialize(EndlessRunManager manager)
    {
        endlessRunManager = manager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        AudioSource.PlayClipAtPoint(checkpointSound, transform.position);
        if (destroyEffectPrefab != null)
        {
            Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
        }
        endlessRunManager.SpawnNextCheckpoint();
        Destroy(gameObject);
    }
}
