using UnityEngine;

public class FogPainter : MonoBehaviour
{
    [SerializeField] private FogOfWarController fogController;
    [SerializeField] private Vector2 worldMin;
    [SerializeField] private Vector2 worldMax;
    [SerializeField] private int revealRadius = 15;

    public void Reveal(Vector3 worldPosition)
    {
        Texture2D texture = fogController.FogTexture;
        Color32[] pixels = fogController.Pixels;
        int textureSize = fogController.TextureSize;
        Vector2Int center = WorldToTexture(worldPosition, texture);
        bool modified = false;
        for (int x = -revealRadius; x <= revealRadius; x++)
        {
            for (int y = -revealRadius; y <= revealRadius; y++)
            {
                if (x * x + y * y > revealRadius * revealRadius)
                    continue;
                int px = center.x + x;
                int py = center.y + y;
                if (px < 0 || py < 0 || px >= texture.width || py >= texture.height)
                    continue;
                int index = py * textureSize + px;
                float distance = Mathf.Sqrt(x * x + y * y);
                float centerOffset = distance / revealRadius;
                float fogValue = Mathf.SmoothStep(0f, 1f, centerOffset);
                byte value = (byte)(fogValue * 255);
                Color32 revealColor = new Color32(value, value, value, 255);
                if (value < pixels[index].r)
                {
                    pixels[index] = revealColor;
                    modified = true;
                }
            }
        }
        if (modified)
            fogController.ApplyChanges();
    }

    private Vector2Int WorldToTexture(Vector3 worldPos, Texture2D texture)
    {
        float x = Mathf.InverseLerp(worldMin.x, worldMax.x, worldPos.x);
        float y = Mathf.InverseLerp(worldMin.y, worldMax.y, worldPos.z);
        return new Vector2Int(Mathf.RoundToInt(x * texture.width), Mathf.RoundToInt(y * texture.height));
    }
}
