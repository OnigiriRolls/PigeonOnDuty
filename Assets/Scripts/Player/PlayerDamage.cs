using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        PlayerAudioController audioController = other.GetComponent<PlayerAudioController>();
        if (health != null)
        {
            health.TakeDamage(1);
            audioController.PlayHitClip();
            Instantiate(health.HitParticlesPrefab, other.transform.position, Quaternion.identity);
        }
    }
}
