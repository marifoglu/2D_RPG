using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Player : Inventory_Base
{
    public event Action<int> OnQuickSlotUsed;

    public Inventory_Storage storage { get; private set; }
    public List<Inventory_EquipmentSlot> equipList;

    [Header("Quick Slots")]
    public Inventory_Item[] quickItems = new Inventory_Item[2];

    [Header("Gold Info")]
    public int gold = 10000;

    protected override void Awake()
    {
        base.Awake();
        if (player == null)
            player = FindAnyObjectByType<Player>();

        storage = FindFirstObjectByType<Inventory_Storage>();
    }

    public void SetQuickItemSlot(int slotNumber, Inventory_Item itemToSet)
    {
        quickItems[slotNumber - 1] = itemToSet;
        TriggerUpdateUI();
    }

    public void TryUseQuickItemInSlot(int passedSlotNumber)
    {
        int slotNumber = passedSlotNumber - 1;
        var itemToUse = quickItems[slotNumber];

        if (itemToUse == null)
            return;

        TryUseItem(itemToUse);

        if (FindItem(itemToUse) == null)
        {
            quickItems[slotNumber] = FindSameItem(itemToUse);
        }
        TriggerUpdateUI();

        OnQuickSlotUsed?.Invoke(slotNumber);
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

    public override void SaveData(ref GameData gameData)
    {
        gameData.gold = gold;
        gameData.itemDictionary.Clear();
        gameData.equipedItems.Clear();

        foreach (var item in itemList)
        {
            if (item != null && item.itemData != null)
            {
                string saveID = item.itemData.saveID;

                if (gameData.itemDictionary.ContainsKey(saveID) == false)
                    gameData.itemDictionary[saveID] = 0;

                gameData.itemDictionary[saveID] += item.stackSize;
            }
        }

        foreach (var slot in equipList)
        {
            if (slot.HasItem())
            {
                gameData.equipedItems[slot.equipedItem.itemData.saveID] = slot.slotType;
            }
        }
    }

    public override void LoadData(GameData gameData)
    {
        gold = gameData.gold;

        foreach (var entry in gameData.itemDictionary)
        {
            string saveId = entry.Key;
            int stackSize = entry.Value;

            ItemDataSO itemData = itemDataBase.GetItemData(saveId);

            if (itemData == null)
            {
                Debug.LogWarning("Item data with saveID " + saveId + " not found in item database.");
                continue;
            }

            for(int i = 0; i < stackSize; i++)
            {
                Inventory_Item itemToLoad = new Inventory_Item(itemData);
                AddItem(itemToLoad);
            }
        }

        foreach (var entry in gameData.equipedItems)
        {
            string saveId = entry.Key;
            ItemType loadSlotType = entry.Value;

            ItemDataSO itemDataSO = itemDataBase.GetItemData(saveId);
            Inventory_Item equipToLoad = new Inventory_Item(itemDataSO);

            var slot = equipList.Find(slot => slot.slotType == loadSlotType && slot.HasItem() == false);

            slot.equipedItem = equipToLoad;
            slot.equipedItem.AddModifiers(player.stats);
            slot.equipedItem.AddItemEffect(player);
        }
        TriggerUpdateUI();
    }
}