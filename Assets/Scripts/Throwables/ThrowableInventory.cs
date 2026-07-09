using System;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableInventory : MonoBehaviour
{
    public ThrowableData EquippedItem => equippedItem;
    public event Action<ThrowableData, int> OnInventoryChanged;

    [SerializeField] private List<InventorySlot> slots = new();
    [SerializeField] private ThrowableData equippedItem;

    public void Equip(ThrowableData item)
    {
        equippedItem = item;
        OnInventoryChanged?.Invoke(item, GetAmount(item));
    }

    public bool TryConsumeEquipped()
    {
        return TryConsume(equippedItem);
    }

    public bool TryConsume(ThrowableData item)
    {
        InventorySlot slot = GetSlot(item);
        if (slot == null)
            return false;
        if (slot.Amount <= 0)
            return false;
        slot.Amount--;
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
}
