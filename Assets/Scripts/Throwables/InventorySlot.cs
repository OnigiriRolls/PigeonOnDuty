using System;

[Serializable]
public class InventorySlot
{
    public ThrowableData Item;
    public int Amount;
    public bool Enabled = true;
}
