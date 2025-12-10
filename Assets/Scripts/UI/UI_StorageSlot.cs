using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StorageSlot : UI_ItemSlot
{
    private Inventory_Storage storage;

    public enum StorageSlotType
    {
        StorageSlot,
        PlayerInventorySlot,
        MaterialStashSlot
    }
    public StorageSlotType slotType;

    public void SetStorage(Inventory_Storage storage) => this.storage = storage;

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null || storage == null)
            return;

        bool transferFullStack = Input.GetKey(KeyCode.LeftControl);

        if (slotType == StorageSlotType.StorageSlot || slotType == StorageSlotType.MaterialStashSlot)
        {
            storage.FromStorageToPlayer(itemInSlot, transferFullStack);
        }
        else if (slotType == StorageSlotType.PlayerInventorySlot)
        {
            storage.FromPlayerToStorage(itemInSlot, transferFullStack);
        }

        if (ui != null && ui.itemToolTip != null)
            ui.itemToolTip.ShowToolTip(false, null);
    }
}