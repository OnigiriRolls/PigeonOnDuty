using UnityEngine;

public class LifePickUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();

        if (health != null)
        {
            health.Heal(1);
            Destroy(gameObject);
        }
    }
}
