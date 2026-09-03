using UnityEngine;

public class MinimapMissionController : MonoBehaviour
{
    [SerializeField] private MinimapIconManager iconManager;
    [SerializeField] private MinimapIcon clientIconPrefab;
    [SerializeField] private MinimapIcon checkpointIconPrefab;
    [SerializeField] private MinimapIcon wellIconPrefab;
    [SerializeField] private MinimapIcon feathersIconPrefab;

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

    public void ShowWell(Transform well)
    {
        iconManager.CreateIcon(wellIconPrefab, well.transform);
    }

    public void HideWell(Transform well)
    {
        iconManager.RemoveIcon(well.transform);
    }

    public void ShowFeathers(Transform spawnPoint)
    {
        iconManager.CreateIcon(feathersIconPrefab, spawnPoint);
    }

    public void HideFeathers(Transform spawnPoint)
    {
        iconManager.RemoveIcon(spawnPoint);
    }
}
