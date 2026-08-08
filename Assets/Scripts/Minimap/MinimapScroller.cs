using UnityEngine;
using UnityEngine.UI;

public class MinimapScroller : MonoBehaviour
{
    public Camera MinimapCamera => minimapCamera;
    public RectTransform MinimapRect => minimapRect;
    public Rect UVRect => minimap.uvRect;
    public Transform Player => player;

    [SerializeField] private RawImage minimap;
    [SerializeField] private RectTransform minimapRect;
    [SerializeField] private Camera minimapCamera;
    [SerializeField] private Transform player;
    [SerializeField, Range(0.05f, 1f)] private float visibleSize = 0.4f;

    private void LateUpdate()
    {
        Vector3 viewport = minimapCamera.WorldToViewportPoint(player.position);
        Rect rect = minimap.uvRect;
        rect.width = visibleSize;
        rect.height = visibleSize;
        rect.x = Mathf.Clamp(viewport.x - visibleSize * 0.5f, 0f, 1f - visibleSize);
        rect.y = Mathf.Clamp(viewport.y - visibleSize * 0.5f, 0f, 1f - visibleSize);
        minimap.uvRect = rect;
    }
}
