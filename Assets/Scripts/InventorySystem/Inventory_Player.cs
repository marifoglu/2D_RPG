using System.Collections.Generic;
using UnityEngine;

public class Inventory_Player : Inventory_Base
{
    public int gold = 1000;

    private Player player;
    public List<Inventory_EquipmentSlot> equipList;
    public Inventory_Storage storage { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
        if (player == null)
            player = FindAnyObjectByType<Player>();

        storage = FindFirstObjectByType<Inventory_Storage>();
    }

    public void TryEquipItem(Inventory_Item item)
    {
        if (item.itemData is not EquipmentDataSO)
            return;

        var inventoryItem = FindItem(item);
        var matchingSlots = equipList.FindAll(slot => slot.slotType == item.itemData.itemType);

        if (matchingSlots.Count == 0)
            return;

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

        UnEquipItem(itemToUnequip, slotReplace != null);
        EquipItem(inventoryItem, slotReplace);
    }

    private void EquipItem(Inventory_Item itemToEquip, Inventory_EquipmentSlot slot)
    {
        if (player == null || itemToEquip == null || slot == null)
            return;

        float savedHealthPercent = player.health.GetHealthPercentage();

        slot.equipedItem = itemToEquip;
        slot.equipedItem.AddModifiers(player.stats);
        slot.equipedItem.AddItemEffect(player);

        player.health.SetHealthToPercentage(savedHealthPercent);
        RemoveOneItem(itemToEquip);
    }

    public void UnEquipItem(Inventory_Item itemToUnequip, bool replacingItem = false)
    {
        if (CanAddItem(itemToUnequip) == false && replacingItem == false)
            return;

        if (player == null || itemToUnequip == null)
            return;

        float savedHealthPercent = player.health.GetHealthPercentage();

        var slotToUnequip = equipList.Find(slot => slot.equipedItem == itemToUnequip);

        if (slotToUnequip != null)
            slotToUnequip.equipedItem = null;

        itemToUnequip.RemoveModifiers(player.stats);
        itemToUnequip.RemoveItemEffect();

        player.health.SetHealthToPercentage(savedHealthPercent);

        AddItem(itemToUnequip);
    }
}