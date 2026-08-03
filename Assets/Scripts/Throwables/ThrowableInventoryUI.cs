using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThrowableInventoryUI : MonoBehaviour
{
    [SerializeField] private ThrowableInventory inventory;
    [SerializeField] private InventorySlotUI[] slots;

    private void OnEnable()
    {
        inventory.OnInventoryChanged += Refresh;
        inventory.OnSelectionChanged += RefreshSelection;
        Refresh();
    }

    private void OnDisable()
    {
        inventory.OnInventoryChanged -= Refresh;
        inventory.OnSelectionChanged -= RefreshSelection;
    }

    private void Refresh(ThrowableData item, int amount)
    {
        Refresh();
    }

    private void RefreshSelection(ThrowableData item)
    {
        Refresh();
    }

    private void Refresh()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i >= inventory.Slots.Count)
            {
                slots[i].Hide();
                continue;
            }
            InventorySlot slot = inventory.Slots[i];
            slots[i].gameObject.SetActive(slot.Enabled);
            if (!slot.Enabled)
                continue;
            slots[i].Setup(slot.Item, slot.Amount, slot.Item == inventory.SelectedItem);
        }
    }
}
