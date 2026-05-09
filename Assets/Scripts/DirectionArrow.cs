using UnityEngine;

public class DirectionArrow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private EndlessRunManager endlessRunManager;

    void Update()
    {
        if (endlessRunManager.CurrentCheckpoint == null)
            return;

        Vector3 direction = endlessRunManager.CurrentCheckpoint.transform.position - player.position;
        transform.forward = direction.normalized;
    }
}
