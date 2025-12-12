using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Merchant : Inventory_Base
{
    private Inventory_Player inventory;

    [SerializeField] private ItemListDataSO shopData;
    [SerializeField] private int minItemAmount = 4;

    protected override void Awake()
    {
        base.Awake();
        FillShopList();
    } 

    public void TryBuyItem(Inventory_Item itemToBuy, bool buyFullStack)
    {
        int amountToBuy = buyFullStack ? itemToBuy.stackSize : 1;

        for(int i = 0; i < amountToBuy; i++)
        {
            if(inventory.gold < itemToBuy.buyPrice)
            {
                Debug.Log("Not enough gold to buy " + itemToBuy.itemData.itemName);
                return;
            }

            if(itemToBuy.itemData.itemType == ItemType.Material)
            {
                inventory.storage.AddMaterialToStash(itemToBuy);
            }
            else
            {
                if(inventory.CanAddItem(itemToBuy))
                {
                   var itemToAdd = new Inventory_Item(itemToBuy.itemData);
                   inventory.AddItem(itemToAdd);
                }
            }
            inventory.gold -= itemToBuy.buyPrice;
            RemoveOneItem(itemToBuy);
        }
        TriggerUpdateUI();
    }

    public void TrySellItem(Inventory_Item itemToSell, bool sellFullStack)
    {
        int amountToSell = sellFullStack ? itemToSell.stackSize : 1;

        for(int i = 0; i < amountToSell; i++)
        {
            int sellPrice = Mathf.FloorToInt(itemToSell.sellPrice);
            inventory.gold += sellPrice;
            inventory.RemoveOneItem(itemToSell);
        }
    }

    public void FillShopList()
    {
        itemList.Clear();
        List<Inventory_Item> possibleItems = new List<Inventory_Item>();

        foreach (var itemData in shopData.itemList)
        {
            int randomStackSize = Random.Range(itemData.minStackSizeAtShop, itemData.maxStackSizeAtShop + 1);
            int finalStackSize = Mathf.Clamp(randomStackSize, 1, itemData.maxStackSize);

            Inventory_Item itemToAdd = new Inventory_Item(itemData);
            itemToAdd.stackSize = finalStackSize;

            possibleItems.Add(itemToAdd);
        }

        int randomItemAmount = Random.Range(minItemAmount, maxInventorySize + 1);
        int finalAmount = Mathf.Clamp(randomItemAmount, 1, possibleItems.Count);

        for(int i = 0; i < finalAmount; i++)
        {
            var randomIndex = Random.Range(0, possibleItems.Count);
            var item = possibleItems[randomIndex];

            if(CanAddItem(item))
            {
                possibleItems.Remove(item);
                AddItem(item);
            }
        }
        TriggerUpdateUI();
    }

    public void SetInventory(Inventory_Player inventory) => this.inventory = inventory;
}
