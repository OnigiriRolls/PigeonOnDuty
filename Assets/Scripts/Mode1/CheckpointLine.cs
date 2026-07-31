using UnityEngine;

public class CheckpointLine : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private CheckpointsManager endlessRunManager;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (endlessRunManager.CurrentObjective == null)
            return;

        lineRenderer.SetPosition(0, player.position);
        lineRenderer.SetPosition(1, endlessRunManager.CurrentObjective.position);
    }
}
