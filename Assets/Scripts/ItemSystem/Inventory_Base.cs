using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Base : MonoBehaviour
{
    public event Action OnInventoryChange;

    public int maxInventorySize = 10;
    public List<Inventory_Item> itemList = new List<Inventory_Item>();

    protected virtual void Awake()
    {

    }

    public bool CanAddItem() => itemList.Count < maxInventorySize;

    public void AddItem(Inventory_Item itemToAdd)
    {
        if (itemToAdd == null)
        {
            Debug.LogWarning("Attempted to AddItem with a null Inventory_Item.");
            return;
        }

        if (itemToAdd.itemData == null)
        {
            Debug.LogWarning("Attempted to AddItem with Inventory_Item that has null itemData.");
            return;
        }

        Inventory_Item itemInInventory = FindItem(itemToAdd.itemData);

        if (itemInInventory != null && itemInInventory.CanAttackStack())
            itemInInventory.AddStack();
        else
            itemList.Add(itemToAdd);

        OnInventoryChange?.Invoke();
    }

    public void RemoveItem(Inventory_Item itemToRemove)
    {
        itemList.Remove(FindItem(itemToRemove.itemData));
        OnInventoryChange?.Invoke();
    }

    public Inventory_Item FindItem(ItemDataSO itemData)
    {
        return itemList.Find(item => item.itemData == itemData);
    }
}