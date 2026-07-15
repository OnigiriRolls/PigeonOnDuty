using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Image selectionFrame;

    public void Setup(ThrowableData item, int amount, bool selected)
    {
        icon.sprite = item.icon;
        icon.enabled = true;
        amountText.text = amount.ToString();
        selectionFrame.enabled = selected;
    }

    public void Hide()
    {
        icon.enabled = false;
        amountText.text = "";
        selectionFrame.enabled = false;
    }
}
