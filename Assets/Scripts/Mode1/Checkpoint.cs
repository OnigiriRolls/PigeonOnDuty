using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    [SerializeField] private GameObject destroyEffectPrefab;
    [SerializeField] private AudioClip checkpointSound;

    private EndlessRunManager endlessRunManager;
    private ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
    }

    public void Initialize(EndlessRunManager manager)
    {
        endlessRunManager = manager;
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
        scoreManager.AddCheckpointScore();
        endlessRunManager.SpawnNextCheckpointAndCollectibles();
        Destroy(gameObject);
    }
}
