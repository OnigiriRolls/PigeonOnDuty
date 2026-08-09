using System.Collections.Generic;
using UnityEngine;

public class PigeonShopManager : MonoBehaviour
{
    public PigeonSkinData EquippedSkin { get; private set; }
    public IReadOnlyList<PigeonSkinData> Skins => skins;

    [SerializeField] private List<PigeonSkinData> skins;

    private void Start()
    {
        InitializeSkins();
        LoadEquippedSkin();
    }

    private void InitializeSkins()
    {
        if (skins.Count == 0)
            return;

        PigeonSkinData defaultSkin = skins[0];
        if (!SaveManager.Instance.IsSkinUnlocked(defaultSkin.id))
        {
            SaveManager.Instance.UnlockSkin(defaultSkin.id);
        }
        if (string.IsNullOrEmpty(SaveManager.Instance.SelectedSkin))
        {
            SaveManager.Instance.SelectSkin(defaultSkin.id);
        }
    }

    public bool IsOwned(PigeonSkinData skin)
    {
        if (skin == null)
            return false;
        return SaveManager.Instance.IsSkinUnlocked(skin.id);
    }

    public bool IsEquipped(PigeonSkinData skin)
    {
        if (skin == null || EquippedSkin == null)
            return false;
        return EquippedSkin.id == skin.id;
    }

    public bool Buy(PigeonSkinData skin)
    {
        if (skin == null)
            return false;
        if (IsOwned(skin))
            return false;
        if (!SaveManager.Instance.SpendCoins(skin.price))
            return false;
        SaveManager.Instance.UnlockSkin(skin.id);
        return true;
    }

    public bool Equip(PigeonSkinData skin)
    {
        if (skin == null)
            return false;
        if (!IsOwned(skin))
            return false;
        SaveManager.Instance.SelectSkin(skin.id);
        EquippedSkin = skin;
        return true;
    }

    private void LoadEquippedSkin()
    {
        string selectedSkinId = SaveManager.Instance.SelectedSkin;
        PigeonSkinData skin = skins.Find(s => s.id == selectedSkinId);
        if (skin == null)
            skin = skins[0];
        EquippedSkin = skin;
    }
}
