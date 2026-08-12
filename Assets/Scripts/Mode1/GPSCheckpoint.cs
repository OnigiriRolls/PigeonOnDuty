using UnityEngine;

public class GPSCheckpoint : MonoBehaviour
{
    private bool playerInside;
    private bool humanInside;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;

        if (other.CompareTag("GPSHuman"))
            humanInside = true;

        TryCompleteMission();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;

        if (other.CompareTag("GPSHuman"))
            humanInside = false;
    }

    private void TryCompleteMission()
    {
        if (!playerInside)
            return;

        if (!humanInside)
            return;

        MissionManager.Instance.CompleteActiveMission();
        Destroy(gameObject);
    }
}
