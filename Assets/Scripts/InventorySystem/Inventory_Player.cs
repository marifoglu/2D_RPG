using System.Collections.Generic;
using UnityEngine;

public class Inventory_Player : Inventory_Base
{
    private Entity_Stats playerStats;
    public List<Inventory_EquipmentSlot> equipList;

    protected override void Awake()
    {
        base.Awake();
        playerStats = GetComponent<Entity_Stats>();
    }

    public void TryEquipItem(Inventory_Item item)
    {
        // Check if the item is actually equipment
        if (item.itemData is not EquipmentDataSO)
        {
            Debug.Log($"Cannot equip {item.itemData.itemName}. This item is not equipment.");
            return;
        }

        var inventoryItem = FindItem(item.itemData);
        var matchingSlots = equipList.FindAll(slot => slot.slotType == item.itemData.itemType);

        if (matchingSlots.Count == 0)
        {
            Debug.LogWarning($"No equipment slot available for item type: {item.itemData.itemType}");
            return;
        }

        foreach (var slot in matchingSlots)
        {
            if (slot.HasItem() == false)
            {
                EquipItem(inventoryItem, slot);
                return;
            }
        }

        var slotReplace = matchingSlots[0];
        var itemToUnequip = slotReplace.equipedItem;

        EquipItem(inventoryItem, slotReplace);
        UnEquipItem(itemToUnequip);
    }

    private void EquipItem(Inventory_Item itemToEquip, Inventory_EquipmentSlot slot)
    {
        slot.equipedItem = itemToEquip;
        slot.equipedItem.AddModifiers(playerStats);
        RemoveItem(itemToEquip);
    }

    public void UnEquipItem(Inventory_Item itemToUnequip)
    {
        if (CanAddItem() == false)
        {
            Debug.Log("Not enough space in inventory to unequip item.");
            return;
        }

        foreach (var slot in equipList)
        {
            if (slot.equipedItem == itemToUnequip)
            {
                slot.equipedItem = null;
                break;
            }
        }

        itemToUnequip.RemoveModifiers(playerStats);
        AddItem(itemToUnequip);
    }



}
