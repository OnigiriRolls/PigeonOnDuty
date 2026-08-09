using UnityEngine;

public class PigeonShopUI : MonoBehaviour
{
    [SerializeField] private PigeonShopManager shopManager;
    [SerializeField] private PigeonSkinShopItem[] items;
    [SerializeField] private UIPanelController confirmationPanel;

    private PigeonSkinData pendingSkin;

    private void OnEnable()
    {
        SaveManager.Instance.OnCoinsChanged += HandleCoinsChanged;
        Refresh();
    }

    public void Refresh()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
                continue;
            if (i >= shopManager.Skins.Count)
            {
                items[i].gameObject.SetActive(false);
                continue;
            }
            items[i].gameObject.SetActive(true);
            items[i].Initialize(shopManager.Skins[i], this);
        }
    }

    public bool IsOwned(PigeonSkinData skin)
    {
        return shopManager.IsOwned(skin);
    }

    public bool IsEquipped(PigeonSkinData skin)
    {
        return shopManager.IsEquipped(skin);
    }

    public void RequestPurchase(PigeonSkinData skin)
    {
        pendingSkin = skin;
        confirmationPanel.Open();
    }

    public void ConfirmPurchase()
    {
        if (pendingSkin == null)
            return;

        bool success = shopManager.Buy(pendingSkin);
        if (success)
        {
            pendingSkin = null;
            confirmationPanel.Close();
            Refresh();
        }
    }

    public void CancelPurchase()
    {
        pendingSkin = null;
        confirmationPanel.Close();
    }

    public void Equip(PigeonSkinData skin)
    {
        if (shopManager.Equip(skin))
        {
            Refresh();
        }
    }

    private void HandleCoinsChanged(int amount)
    {
        Refresh();
    }

    private void OnDisable()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.OnCoinsChanged -= HandleCoinsChanged;
    }
}
