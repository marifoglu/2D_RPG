using System;
using UnityEngine;

public class UI_Storage : MonoBehaviour
{
    private Inventory_Player inventory;
    private Inventory_Storage storage;

    [SerializeField] private UI_ItemSlotParent inventoryParent;
    [SerializeField] private UI_ItemSlotParent storageParent;
    [SerializeField] private UI_ItemSlotParent materialStashParent;

    public void SetupStorageUI(Inventory_Storage storage)
    {
        this.storage = storage;
        inventory = storage.playerInventory;

        storage.OnInventoryChange -= UpdateUI;
        inventory.OnInventoryChange -= UpdateUI;

        storage.OnInventoryChange += UpdateUI;
        inventory.OnInventoryChange += UpdateUI;

        UpdateUI();

        UI_StorageSlot[] storageSlots = GetComponentsInChildren<UI_StorageSlot>(true); // Include inactive

        foreach (var slot in storageSlots)
        {
            if (slot != null)
                slot.SetStorage(storage);
        }
    }
    private void OnEnable()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (inventory == null || storage == null)
        {
            Debug.LogWarning("UI_Storage: Cannot update UI - inventory or storage is null");
            return;
        }

        // Null check each parent before updating
        if (inventoryParent != null)
            inventoryParent.UpdateSlots(inventory.itemList);
        else
            Debug.LogWarning("UI_Storage: inventoryParent is not assigned in Inspector!");

        if (storageParent != null)
            storageParent.UpdateSlots(storage.itemList);
        else
            Debug.LogWarning("UI_Storage: storageParent is not assigned in Inspector!");

        if (materialStashParent != null)
            materialStashParent.UpdateSlots(storage.materialStash);
        else
            Debug.LogWarning("UI_Storage: materialStashParent is not assigned in Inspector!");
    }



    private void OnDisable()
    {
        if (storage != null)
            storage.OnInventoryChange -= UpdateUI;

        if (inventory != null)
            inventory.OnInventoryChange -= UpdateUI;
    }
}