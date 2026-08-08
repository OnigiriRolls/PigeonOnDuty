using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class PlayerMinimapIcon : MonoBehaviour
{
    [SerializeField] private MinimapScroller minimapScroller;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (minimapScroller == null)
            return;

        Vector3 viewport = minimapScroller.MinimapCamera.WorldToViewportPoint(minimapScroller.Player.position);
        Rect visible = minimapScroller.UVRect;
        float x = (viewport.x - visible.x) / visible.width;
        float y = (viewport.y - visible.y) / visible.height;
        Vector2 size = minimapScroller.MinimapRect.rect.size;
        Vector2 position = new((x - 0.5f) * size.x, (y - 0.5f) * size.y);
        Vector2 halfSize = size * 0.5f;
        position.x = Mathf.Clamp(position.x, -halfSize.x, halfSize.x);
        position.y = Mathf.Clamp(position.y, -halfSize.y, halfSize.y);
        rectTransform.anchoredPosition = position;
    }
}
