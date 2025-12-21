using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory_Storage : Inventory_Base
{
    public Inventory_Player playerInventory { get; private set; }
    public List<Inventory_Item> materialStash = new List<Inventory_Item>();

    public bool CanCraftItem(Inventory_Item itemToCraft)
    {
        return HasEnoughMaterials(itemToCraft) && playerInventory.CanAddItem(itemToCraft);
    }

    public void CraftItem(Inventory_Item itemToCraft)
    {
        ConsumeMaterials(itemToCraft);
        playerInventory.AddItem(itemToCraft);
    }

    public void AddMaterialToStash(Inventory_Item itemToAdd)
    {
        var stackableItem = StackableInStash(itemToAdd);

        if (stackableItem != null)
            stackableItem.AddStack();
        else
        {
            var newItemToAdd = new Inventory_Item(itemToAdd.itemData);
            materialStash.Add(newItemToAdd);
        }

        TriggerUpdateUI();
        materialStash = materialStash.OrderBy(item => item.itemData.name).ToList();
    }

    public Inventory_Item StackableInStash(Inventory_Item itemToAdd)
    {
        return materialStash.Find(item => item.itemData == itemToAdd.itemData && item.CanAddStack());
    }


    public void SetInventory(Inventory_Player inventory) => this.playerInventory = inventory;
    public void FromPlayerToStorage(Inventory_Item item, bool transferFullStack)
    {
        int transferAmount = transferFullStack ? item.stackSize : 1;

        for (int i = 0; i < transferAmount; i++)
        {
            if (CanAddItem(item))
            {
                var itemToAdd = new Inventory_Item(item.itemData);

                playerInventory.RemoveOneItem(item);
                AddItem(itemToAdd);
            }
        }

        TriggerUpdateUI();
    }

    public void FromStorageToPlayer(Inventory_Item item, bool transferFullStack)
    {
        int transferAmount = transferFullStack ? item.stackSize : 1;

        for (int i = 0; i < transferAmount; i++)
        {
            if (playerInventory.CanAddItem(item))
            {
                var itemToAdd = new Inventory_Item(item.itemData);

                RemoveOneItem(item);
                playerInventory.AddItem(itemToAdd);
            }
        }
        TriggerUpdateUI();
    }


public int GetAvailableAmountOf(ItemDataSO requiredItem)
    {
        int amount = 0;

        foreach (var item in playerInventory.itemList)
        {
            if (item.itemData == requiredItem)
                amount += item.stackSize;
        }

        foreach(var item in itemList)
        {
            if (item.itemData == requiredItem)
                amount += item.stackSize;
        }

        foreach(var item in materialStash)
        {
            if (item.itemData == requiredItem)
                amount += item.stackSize;
        }
        return amount;
    }

    private bool HasEnoughMaterials(Inventory_Item itemToCraft)
    {
        foreach (var requiredMaterial in itemToCraft.itemData.craftReceipe)
        {
            if (GetAvailableAmountOf(requiredMaterial.itemData) < requiredMaterial.stackSize)
                return false;
        }
        return true;
    }

    private int ConsumedMaterialsAmount(List<Inventory_Item> itemList, Inventory_Item neededItem)
    {
        int amountNeeded = neededItem.stackSize;
        int consumedAmount = 0;

        for (int i = itemList.Count - 1; i >= 0; i--)
        {
            var item = itemList[i];

            if (item.itemData != neededItem.itemData)
                continue;

            int removeAmount = Mathf.Min(item.stackSize, amountNeeded - consumedAmount);
            item.stackSize -= removeAmount;
            consumedAmount += removeAmount;

            if (item.stackSize <= 0)
                itemList.RemoveAt(i);

            if (consumedAmount >= amountNeeded)
                break;
        }
        return consumedAmount;
    }

    private void ConsumeMaterials(Inventory_Item itemToCraft)
    {
        foreach(var requiredItem in itemToCraft.itemData.craftReceipe)
        {
            int amountToConsume = requiredItem.stackSize;

            amountToConsume = amountToConsume - ConsumedMaterialsAmount(playerInventory.itemList, requiredItem);

            if(amountToConsume > 0)
                amountToConsume = amountToConsume - ConsumedMaterialsAmount(itemList, requiredItem);

            if(amountToConsume > 0)
                amountToConsume = amountToConsume - ConsumedMaterialsAmount(materialStash, requiredItem);
            
        }
    }

    public override void SaveData(ref GameData gameData)
    {
        base.SaveData(ref gameData);

        gameData.storageItems.Clear();

        foreach (var item in itemList)
        {
            if (item != null && item.itemData != null)
            {
                string saveID = item.itemData.saveID;

                if (gameData.storageItems.ContainsKey(saveID) == false)
                    gameData.storageItems[saveID] = 0;

                gameData.storageItems[saveID] += item.stackSize;

            }
        }
        gameData.storageMaterials.Clear();

        foreach (var item in materialStash)
        {
            if (item != null && item.itemData != null)
            {
                string saveID = item.itemData.saveID;

                if (gameData.storageMaterials.ContainsKey(saveID) == false)
                    gameData.storageMaterials[saveID] = 0;

                gameData.storageMaterials[saveID] += item.stackSize;

            }
        }
    }

    public override void LoadData(GameData gameData)
    {
        itemList.Clear();
        materialStash.Clear();

        foreach (var item in gameData.storageItems)
        {
            string saveId = item.Key;
            int stackSize = item.Value;

            ItemDataSO itemData = itemDataBase.GetItemData(saveId);

            if (itemData == null)
            {
                Debug.LogWarning("Item data with saveID " + saveId + " not found in item database.");
                continue;
            }

            for (int i = 0; i < stackSize; i++)
            {
                Inventory_Item itemToLoad = new Inventory_Item(itemData);
                AddItem(itemToLoad);
            }
        }

        foreach (var item in gameData.storageMaterials)
        {
            string saveId = item.Key;
            int stackSize = item.Value;

            ItemDataSO itemData = itemDataBase.GetItemData(saveId);

            if (itemData == null)
            {
                Debug.LogWarning("Item data with saveID " + saveId + " not found in item database.");
                continue;
            }

            for (int i = 0; i < stackSize; i++)
            {
                Inventory_Item itemToLoad = new Inventory_Item(itemData);
                AddMaterialToStash(itemToLoad);
            }
        }
    }
}