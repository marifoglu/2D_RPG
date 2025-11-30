using System.Collections.Generic;
using UnityEngine;


public class UI_Inventory : MonoBehaviour
{
    private UI_ItemSlot[] itemSlots;
    private Inventory_Base inventory;

    private void Awake()
    {
        itemSlots = GetComponentsInChildren<UI_ItemSlot>();

        inventory = FindFirstObjectByType<Inventory_Base>();
        inventory.OnInventoryChange += UpdateInventorySlots;
        UpdateInventorySlots();

    }

    private void UpdateInventorySlots()
    {
        List<Inventory_Item> itemList = inventory.itemList;

        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < itemList.Count)
            {
                itemSlots[i].UpdateSlot(itemList[i]);
            }
            else
            {
                itemSlots[i].UpdateSlot(null);
            }
        }
    }
}
