using UnityEngine;
using UnityEngine.UI;

public class FogOfWarController : MonoBehaviour
{
    public Texture2D FogTexture { get; private set; }
    public Color32[] Pixels { get; private set; }
    public int TextureSize => textureSize;

    [SerializeField] private int textureSize = 512;
    [SerializeField] private RawImage debugImage;

    private void Awake()
    {
        FogTexture = new Texture2D(textureSize, textureSize, TextureFormat.R8, false);
        Pixels = new Color32[textureSize * textureSize];

        for (int i = 0; i < Pixels.Length; i++)
            Pixels[i] = Color.white;

        FogTexture.SetPixels32(Pixels);
        FogTexture.Apply();
        if (debugImage != null)
            debugImage.texture = FogTexture;
    }

    public void ApplyChanges()
    {
        FogTexture.SetPixels32(Pixels);
        FogTexture.Apply();
    }
}
