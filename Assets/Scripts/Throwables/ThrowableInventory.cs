using System;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableInventory : MonoBehaviour
{
    public ThrowableData SelectedItem => selectedIndex >= 0 && selectedIndex < slots.Count ? slots[selectedIndex].Item : null;
    public event Action<ThrowableData, int> OnInventoryChanged;
    public event Action<ThrowableData> OnSelectionChanged;
    public IReadOnlyList<InventorySlot> Slots => slots;

    [SerializeField] private List<InventorySlot> slots = new();
    [SerializeField] private int selectedIndex;

    public void Select(ThrowableData item)
    {
        int index = slots.FindIndex(s => s.Item == item);
        if (index < 0)
            return;
        selectedIndex = index;
        OnSelectionChanged?.Invoke(item);
    }

    public void SelectNext()
    {
        if (slots.Count == 0)
            return;
        int start = selectedIndex;
        do
        {
            selectedIndex++;
            if (selectedIndex >= slots.Count)
                selectedIndex = 0;
            if (IsSelectable(selectedIndex))
            {
                OnSelectionChanged?.Invoke(SelectedItem);
                return;
            }
        } while (selectedIndex != start);
    }

    public void SelectPrevious()
    {
        if (slots.Count == 0)
            return;
        selectedIndex--;
        if (selectedIndex < 0)
            selectedIndex = slots.Count - 1;
        OnSelectionChanged?.Invoke(SelectedItem);
    }

    private bool IsSelectable(int index)
    {
        return slots[index].Amount > 0;
    }

    public bool TryConsumeEquipped()
    {
        return TryConsume(SelectedItem);
    }

    public bool TryConsume(ThrowableData item)
    {
        InventorySlot slot = GetSlot(item);
        if (slot == null)
            return false;
        if (slot.Amount <= 0)
            return false;
        slot.Amount--;
        if (item == SelectedItem && slot.Amount == 0)
            SelectNext();
        OnInventoryChanged?.Invoke(item, slot.Amount);
        return true;
    }

    public void Add(ThrowableData item, int amount)
    {
        InventorySlot slot = GetSlot(item);
        if (slot == null)
        {
            slot = new InventorySlot { Item = item, Amount = 0 };
            slots.Add(slot);
        }
        slot.Amount += amount;
        OnInventoryChanged?.Invoke(item, slot.Amount);
    }

    public int GetAmount(ThrowableData item)
    {
        InventorySlot slot = GetSlot(item);
        return slot == null ? 0 : slot.Amount;
    }

    private InventorySlot GetSlot(ThrowableData item)
    {
        return slots.Find(s => s.Item == item);
    }

    public void SetAmount(ThrowableData item, int amount)
    {
        InventorySlot slot = GetSlot(item);
        if (slot == null)
        {
            slot = new InventorySlot { Item = item };
            slots.Add(slot);
        }
        slot.Amount = amount;
        OnInventoryChanged?.Invoke(item, amount);
    }

    public void Initialize(List<InventorySlot> startingItems)
    {
        foreach (InventorySlot slot in startingItems)
            SetAmount(slot.Item, slot.Amount);
        Select(startingItems[0].Item);
    }
}
