using System;
using UnityEngine;

public class UI_Storage : MonoBehaviour
{
    private Inventory_Player inventory;
    private Inventory_Storage storage;

    [SerializeField] private UI_ItemSlotParent inventyParent;
    [SerializeField] private UI_ItemSlotParent storageParent;
    [SerializeField] private UI_ItemSlotParent materialStashParent;

    public void SetupStorage(Inventory_Player player, Inventory_Storage storage)
    {
        this.inventory = player;
        this.storage = storage;
        storage.OnInventoryChange += UpdateUI;
        UpdateUI();

        UI_StorageSlot[] storegeSlots = GetComponentsInChildren<UI_StorageSlot>();

        foreach (var slot in storegeSlots)
        {
            slot.SetStorage(storage);
        }
    }

    private void UpdateUI()
    {
        inventyParent.UpdateSlots(inventory.itemList);
        storageParent.UpdateSlots(storage.itemList);
        materialStashParent.UpdateSlots(storage.materialStash);
    }
}
