using System;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableInventory : MonoBehaviour
{
    public event Action<ThrowableData, int> OnItemChanged;

    [SerializeField] private List<InventorySlot> items = new();

    public bool TryConsume(ThrowableData item)
    {
        InventorySlot slot = items.Find(s => s.item == item);
        if (slot == null)
            return false;
        if (slot.amount <= 0)
            return false;
        slot.amount--;
        return true;
    }

    public void Add(ThrowableData item, int amount)
    {
        InventorySlot slot = items.Find(s => s.item == item);
        if (slot == null)
        {
            slot = new InventorySlot { item = item, amount = 0 };
            items.Add(slot);
        }
        slot.amount += amount;
    }

    public int GetAmount(ThrowableData item)
    {
        InventorySlot slot = items.Find(s => s.item == item);
        return slot == null ? 0 : slot.amount;
    }
}
