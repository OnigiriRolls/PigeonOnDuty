using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PigeonSkinShopItem : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button equipButton;

    private PigeonSkinData skin;
    private PigeonShopUI shopUI;

    public void Initialize(PigeonSkinData skin, PigeonShopUI shopUI)
    {
        this.skin = skin;
        this.shopUI = shopUI;
        nameText.text = skin.skinName;
        priceText.text = skin.price.ToString();
        Refresh();
    }

    public void Refresh()
    {
        bool owned = shopUI.IsOwned(skin);
        bool equipped = shopUI.IsEquipped(skin);
        buyButton.gameObject.SetActive(!owned);
        equipButton.gameObject.SetActive(owned);
        equipButton.interactable = !equipped;
        if (!owned)
        {
            buyButton.interactable = SaveManager.Instance.TotalCoins >= skin.price;
        }
    }

    public void OnBuyClicked()
    {
        shopUI.RequestPurchase(skin);
    }

    public void OnEquipClicked()
    {
        shopUI.Equip(skin);
    }
}
