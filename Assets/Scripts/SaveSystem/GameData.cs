using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public int gold;

    // Player health persistence
    public float playerCurrentHealth;
    public float playerHealthPercentage;

    // Inventory system
    public List<Inventory_Item> itemList;
    public SerializableDictionary<string, int> inventory;  // item saveID -> stackSize
    public SerializableDictionary<string, int> storageItems;
    public SerializableDictionary<string, int> storageMaterials;

    public SerializableDictionary<string, ItemType> equipedItems; //  itemSaveId -> slot for item

    // Experience and skill point system
    public int skillPoints;
    public int currentExperience;

    public SerializableDictionary<string, bool> skillTreeUI; // skillID -> isUnlocked status
    public SerializableDictionary<SkillType, SkillUpgradeType> skillUpgrades; // SkillType -> SkillUpgradeType

    // Checkpoints
    public SerializableDictionary<string, bool> unlockedCheckpoints; // checkpointID -> unlocked status
    public SerializableDictionary<string, Vector3> inScenePortals; // scene name -> portal position

    // Shrine System  - Cross-scene teleport data
    [Header("Shrine System")]
    public SerializableDictionary<string, bool> activatedShrines;
    public string lastActivatedShrineID;
    public string lastActivatedShrineScene;  // Scene name
    public float lastActivatedShrinePosX;    // Position X
    public float lastActivatedShrinePosY;    // Position Y
    public float lastActivatedShrinePosZ;    // Position Z
    public string previousShrineID;

    // Quest System
    public SerializableDictionary<string, int> activeQuests; // questID -> simple progress
    public SerializableDictionary<string, bool> completedQuests; // questID -> completed status

    // Complex Quest Objective Progress
    // questSaveID -> (objectiveID -> progress)
    public SerializableDictionary<string, SerializableDictionary<string, int>> questObjectiveProgress;

    public string portalDestinationSceneName;
    public bool returningFromTown;

    public string lastScenePlayed;

    public Vector3 lastPlayerPosition;

    public GameData()
    {
        // Default health values
        playerCurrentHealth = -1; // -1 means not set yet
        playerHealthPercentage = 1.0f; // Default to 100%

        inventory = new SerializableDictionary<string, int>();
        storageItems = new SerializableDictionary<string, int>();
        storageMaterials = new SerializableDictionary<string, int>();

        equipedItems = new SerializableDictionary<string, ItemType>();

        skillTreeUI = new SerializableDictionary<string, bool>();
        skillUpgrades = new SerializableDictionary<SkillType, SkillUpgradeType>();

        unlockedCheckpoints = new SerializableDictionary<string, bool>();

        // Portals
        inScenePortals = new SerializableDictionary<string, Vector3>();

        // Shrine data
        activatedShrines = new SerializableDictionary<string, bool>();
        lastActivatedShrineID = "";
        lastActivatedShrineScene = "";
        lastActivatedShrinePosX = 0;
        lastActivatedShrinePosY = 0;
        lastActivatedShrinePosZ = 0;
        previousShrineID = "";

        activeQuests = new SerializableDictionary<string, int>();
        completedQuests = new SerializableDictionary<string, bool>();

        // complex quest progress storage
        questObjectiveProgress = new SerializableDictionary<string, SerializableDictionary<string, int>>();
    }
}