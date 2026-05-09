using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private EndlessRunManager endlessRunManager;

    public void Initialize(EndlessRunManager manager)
    {
        endlessRunManager = manager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            endlessRunManager.SpawnNextCheckpoint();
            Destroy(gameObject);
        }
    }
}
