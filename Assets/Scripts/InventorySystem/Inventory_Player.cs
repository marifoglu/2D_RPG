using System.Collections.Generic;
using UnityEngine;

public class Inventory_Player : Inventory_Base
{
    private Player player;
    public List<Inventory_EquipmentSlot> equipList;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
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
        float savedHealthPercent = player.health.GetHealthPercentage();

        slot.equipedItem = itemToEquip;
        slot.equipedItem.AddModifiers(player.stats);

        player.health.SetHealthToPercentage(savedHealthPercent);
        RemoveItem(itemToEquip);
    }

    public void UnEquipItem(Inventory_Item itemToUnequip)
    {
        if (CanAddItem() == false)
        {
            Debug.Log("Not enough space in inventory to unequip item.");
            return;
        }

        float savedHealthPercent = player.health.GetHealthPercentage();

        var slotToUnequip = equipList.Find(slot => slot.equipedItem == itemToUnequip);

        if(slotToUnequip != null)
            slotToUnequip.equipedItem = null; 

        itemToUnequip.RemoveModifiers(player.stats);

        player.health.SetHealthToPercentage(savedHealthPercent);

        AddItem(itemToUnequip);
    }



}
