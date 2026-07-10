using UnityEngine;

public class HumanInteractionZone : MonoBehaviour
{
    public bool PlayerInside { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        PlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        PlayerInside = false;
    }
}
