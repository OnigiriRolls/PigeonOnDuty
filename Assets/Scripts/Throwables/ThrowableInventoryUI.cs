using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThrowableInventoryUI : MonoBehaviour
{
    [SerializeField] private ThrowableInventory inventory;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;

    private void OnEnable()
    {
        inventory.OnInventoryChanged += Refresh;
        ThrowableData item = inventory.EquippedItem;
        if (item != null)
            Refresh(item, inventory.GetAmount(item));
        else
            Hide();
    }

    private void OnDisable()
    {
        inventory.OnInventoryChanged -= Refresh;
    }

    private void Refresh(ThrowableData item, int amount)
    {
        if (item == null)
        {
            Hide();
            return;
        }
        icon.enabled = true;
        icon.sprite = item.icon;
        amountText.text = amount.ToString();
    }

    private void Hide()
    {
        icon.enabled = false;
        amountText.text = "";
    }
}
