using System.Collections.Generic;
using UnityEngine;

public class Inventory_Storage : Inventory_Base
{
    public Inventory_Player playerInventory { get; private set; }
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
            return;

        Inventory_Item playerItem = playerInventory.FindItem(item.itemData);
        if (playerItem == null)
            return;

        int transferAmount = transferFullStack ? playerItem.stackSize : 1;

        if (item.itemData.itemType == ItemType.Material)
        {
            for (int i = 0; i < transferAmount; i++)
            {
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
            for (int i = 0; i < transferAmount; i++)
            {
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
            return;

        int transferAmount = transferFullStack ? item.stackSize : 1;

        if (item.itemData.itemType == ItemType.Material)
        {
            for (int i = 0; i < transferAmount; i++)
            {
                Inventory_Item materialItem = materialStash.Find(m => m.itemData == item.itemData);
                if (materialItem == null || materialItem.stackSize <= 0)
                    break;

                if (!playerInventory.CanAddItem(materialItem))
                    break;

                var itemToAdd = new Inventory_Item(item.itemData);

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
            for (int i = 0; i < transferAmount; i++)
            {
                Inventory_Item storageItem = FindItem(item.itemData);
                if (storageItem == null || storageItem.stackSize <= 0)
                    break;

                if (!playerInventory.CanAddItem(storageItem))
                    break;

                var itemToAdd = new Inventory_Item(item.itemData);
                RemoveOneItem(storageItem);
                playerInventory.AddItem(itemToAdd);
            }
        }

        playerInventory.TriggerUpdateUI();
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

    public bool HasEnoughMaterials(Inventory_Item itemToCraft)
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

    public void ConsumeMaterials(Inventory_Item itemToCraft)
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
}