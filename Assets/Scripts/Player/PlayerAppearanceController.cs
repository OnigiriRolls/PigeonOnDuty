using System.Collections.Generic;
using UnityEngine;

public class PlayerAppearanceController : MonoBehaviour
{
    public Material PigeonMaterial { get; private set; }
    
    [SerializeField] private List<PigeonSkinData> skins;
    [SerializeField] private PigeonSkinData defaultSkin;
    [SerializeField] private Renderer pigeonRenderer;

    private Color originalEmissionColor = Color.black;

    private void Awake()
    {
        PigeonMaterial = new Material(pigeonRenderer.material);
        pigeonRenderer.material = PigeonMaterial;
        originalEmissionColor = PigeonMaterial.GetColor("_EmissionColor");
    }


    private void Start()
    {
        ApplySavedSkin();
    }

    public void ApplySavedSkin()
    {
        if (SaveManager.Instance == null)
        {
            ApplySkin(defaultSkin);
            return;
        }

        string selectedSkinId = SaveManager.Instance.SelectedSkin;
        if (string.IsNullOrEmpty(selectedSkinId))
        {
            ApplySkin(defaultSkin);
            return;
        }

        PigeonSkinData skin = skins.Find(s => s.id == selectedSkinId);
        if (skin == null)
        {
            Debug.LogWarning($"Skin '{selectedSkinId}' was not found. Using default skin.");
            skin = defaultSkin;
        }
        ApplySkin(skin);
    }

    public void ApplySkin(PigeonSkinData skin)
    {
        if (skin == null)
            return;
        if (PigeonMaterial == null)
            return;
        PigeonMaterial.color = skin.color;
    }

    public void SetEmission(Color color)
    {
        if (PigeonMaterial == null)
            return;
        PigeonMaterial.SetColor("_EmissionColor", color);
    }

    public void RestoreOriginalEmission()
    {
        if (PigeonMaterial == null)
            return;
        PigeonMaterial.SetColor("_EmissionColor", originalEmissionColor);
    }
}
