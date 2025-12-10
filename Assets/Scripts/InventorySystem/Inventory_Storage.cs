using System.Collections.Generic;
using UnityEngine;

public class Inventory_Storage : Inventory_Base
{
    private Inventory_Player playerInventory;
    public List<Inventory_Item> materialStash = new List<Inventory_Item>();

    public void AddMaterialToStash(Inventory_Item itemToAdd)
    {
        var stackableItem = StackableInStash(itemToAdd);

        if (stackableItem != null)
        {
            stackableItem.AddStack();
        }
        else
        {
            materialStash.Add(itemToAdd);
        }

        TriggerUpdateUI();
    }

    public Inventory_Item StackableInStash(Inventory_Item itemToAdd)
    {
        List<Inventory_Item> stackableItems = materialStash.FindAll(item => item.itemData == itemToAdd.itemData);

        foreach (var stackable in stackableItems)
        {
            if (stackable.CanAddStack())
                return stackable;
        }
        return null;
    }

    public void SetInventory(Inventory_Player inventory) => this.playerInventory = inventory;

    public void FromPlayerToStorage(Inventory_Item item, bool transferFullStack)
    {
        if (item == null || item.itemData == null || playerInventory == null)
        {
            Debug.LogWarning("FromPlayerToStorage: Invalid parameters");
            return;
        }

        // Find the actual item in player inventory
        Inventory_Item playerItem = playerInventory.FindItem(item.itemData);
        if (playerItem == null)
        {
            Debug.LogWarning("FromPlayerToStorage: Item not found in player inventory");
            return;
        }

        int transferAmount = transferFullStack ? playerItem.stackSize : 1;

        if (item.itemData.itemType == ItemType.Material)
        {
            // Transfer materials to material stash
            for (int i = 0; i < transferAmount; i++)
            {
                // Re-find in case stack was depleted
                playerItem = playerInventory.FindItem(item.itemData);
                if (playerItem == null || playerItem.stackSize <= 0)
                    break;

                var itemToAdd = new Inventory_Item(item.itemData);
                playerInventory.RemoveOneItem(playerItem);
                AddMaterialToStash(itemToAdd);
            }
        }
        else
        {
            // Transfer regular items (equipment/consumables) to storage
            for (int i = 0; i < transferAmount; i++)
            {
                // Re-find in case stack was depleted
                playerItem = playerInventory.FindItem(item.itemData);
                if (playerItem == null || playerItem.stackSize <= 0)
                    break;
                    
                if (!CanAddItem(playerItem))
                {
                    Debug.LogWarning("Storage is full");
                    break;
                }

                var itemToAdd = new Inventory_Item(item.itemData);
                playerInventory.RemoveOneItem(playerItem);
                AddItem(itemToAdd);
            }
        }

        playerInventory.TriggerUpdateUI();
        TriggerUpdateUI();
    }

    public void FromStorageToPlayer(Inventory_Item item, bool transferFullStack)
    {
        if (item == null || item.itemData == null || playerInventory == null)
        {
            Debug.LogWarning("FromStorageToPlayer: Invalid parameters");
            return;
        }

        // Determine transfer amount based on current item stack
        int transferAmount = transferFullStack ? item.stackSize : 1;

        if (item.itemData.itemType == ItemType.Material)
        {
            // Transfer materials from material stash to player inventory
            for (int i = 0; i < transferAmount; i++)
            {
                // Re-find the material item each iteration
                Inventory_Item materialItem = materialStash.Find(m => m.itemData == item.itemData);
                if (materialItem == null || materialItem.stackSize <= 0)
                    break;

                if (!playerInventory.CanAddItem(materialItem))
                {
                    Debug.LogWarning("Player inventory is full");
                    break;
                }

                var itemToAdd = new Inventory_Item(item.itemData);

                // Remove from material stash
                if (materialItem.stackSize > 1)
                {
                    materialItem.RemoveStack();
                }
                else
                {
                    materialStash.Remove(materialItem);
                }

                playerInventory.AddItem(itemToAdd);
            }
        }
        else
        {
            // Transfer regular items (equipment/consumables) from storage to player inventory
            for (int i = 0; i < transferAmount; i++)
            {
                // Re-find the storage item each iteration
                Inventory_Item storageItem = FindItem(item.itemData);
                if (storageItem == null || storageItem.stackSize <= 0)
                    break;

                if (!playerInventory.CanAddItem(storageItem))
                {
                    Debug.LogWarning("Player inventory is full");
                    break;
                }

                var itemToAdd = new Inventory_Item(item.itemData);
                RemoveOneItem(storageItem);
                playerInventory.AddItem(itemToAdd);
            }
        }

        playerInventory.TriggerUpdateUI();
        TriggerUpdateUI();
    }
}