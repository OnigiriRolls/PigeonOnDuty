using System.Collections.Generic;
using UnityEngine;

public class PlayerAppearanceController : MonoBehaviour
{
    [SerializeField] private List<PigeonSkinData> skins;
    [SerializeField] private PigeonSkinData defaultSkin;
    [SerializeField] private Renderer pigeonRenderer;

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
        if (pigeonRenderer == null)
            return;
        pigeonRenderer.material.color = skin.color;
    }
}
