using UnityEngine;

public class MobileInventorySlot : MonoBehaviour
{
    [SerializeField] private ThrowableInventory inventory;
    [SerializeField] private ThrowableData item;

    public void SelectItem()
    {
        if (inventory == null || item == null)
            return;
        inventory.Select(item);
    }
}
