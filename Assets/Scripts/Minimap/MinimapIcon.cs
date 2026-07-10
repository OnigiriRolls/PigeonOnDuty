using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class MinimapIcon : MonoBehaviour
{
    private Transform target;
    private RectTransform rectTransform;
    private MinimapScroller minimap;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(Transform target, MinimapScroller minimap)
    {
        this.target = target;
        this.minimap = minimap;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;
        Vector3 viewport = minimap.MinimapCamera.WorldToViewportPoint(target.position);
        Rect visible = minimap.UVRect;
        float x = (viewport.x - visible.x) / visible.width;
        float y = (viewport.y - visible.y) / visible.height;
        Vector2 halfSize = minimap.MinimapRect.rect.size * 0.5f;
        Vector2 position = new((x - 0.5f) * minimap.MinimapRect.rect.width, (y - 0.5f) * minimap.MinimapRect.rect.height);
        position.x = Mathf.Clamp(position.x, -halfSize.x, halfSize.x);
        position.y = Mathf.Clamp(position.y, -halfSize.y, halfSize.y);
        rectTransform.anchoredPosition = position;
    }
}
