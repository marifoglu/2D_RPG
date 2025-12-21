using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public int gold;

    public List<Inventory_Item> itemList;
    public SerializableDictionary<string, int> itemDictionary;  // item saveID -> stackSize
    public SerializableDictionary<string, int> storageItems; 
    public SerializableDictionary<string, int> storageMaterials; 

    public SerializableDictionary<string, ItemType> equipedItems;  // itemSaveID -> slotType

    public int skillPoints;
    public SerializableDictionary<string, bool> skillTreeUI; // skillID -> isUnlocked status
    public SerializableDictionary<SkillType, SkillUpgradeType> skillUpgrades; // SkillType -> SkillUpgradeType


    public GameData()
    {
        itemDictionary = new SerializableDictionary<string, int>();
        storageItems = new SerializableDictionary<string, int>();
        storageMaterials = new SerializableDictionary<string, int>();

        equipedItems = new SerializableDictionary<string, ItemType>();

        skillTreeUI = new SerializableDictionary<string, bool>();
        skillUpgrades = new SerializableDictionary<SkillType, SkillUpgradeType>();
    }
}   
