using UnityEngine;

public class MinimapMissionController : MonoBehaviour
{
    [SerializeField] private MinimapIconManager iconManager;
    [SerializeField] private MinimapIcon clientIconPrefab;
    [SerializeField] private MinimapIcon checkpointIconPrefab;

    public void ShowClient(Transform client)
    {
        iconManager.CreateIcon(clientIconPrefab, client.transform);
    }

    public void HideClient(Transform client)
    {
        iconManager.RemoveIcon(client.transform);
    }

    public void ShowCheckpoint(Transform checkpoint)
    {
        iconManager.CreateIcon(checkpointIconPrefab, checkpoint.transform);
    }

    public void HideCheckpoint(Transform checkpoint)
    {
        iconManager.RemoveIcon(checkpoint.transform);
    }
}
