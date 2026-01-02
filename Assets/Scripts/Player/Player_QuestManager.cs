//using System.Collections.Generic;
//using UnityEngine;

//public class Player_QuestManager : MonoBehaviour, ISaveable
//{
//    public List<QuestData> activeQuests;
//    public List<QuestData> complateQuests;

//    private Entity_DropManager dropManager;
//    private Inventory_Player inventory;

//    [Header("Quest Database")]
//    [SerializeField] private QuestDatabaseSO questDatabase;

//    private void Awake()
//    {
//        dropManager = GetComponent<Entity_DropManager>();
//        inventory = GetComponent<Inventory_Player>();
//    }

//    public  void TryGetRewardFrom(RewardType npcType)
//    {
//        List<QuestData> getRewardQuest = new List<QuestData>();

//        foreach (var quest in activeQuests)
//        {
//            // Delivery quest check
//            if (quest.questDataSo.questType != QuestType.Delivery)
//            {
//                var requiredItem = quest.questDataSo.itemToDelivery;
//                var requiredAmount = quest.questDataSo.requiredAmount;

//                if (inventory.HasItemAmount(requiredItem, requiredAmount))
//                {
//                    inventory.RemoveItemAmount(requiredItem, requiredAmount);
//                    quest.AddQuestProgress(requiredAmount);
//                }
//            }
//            if (quest.CanGetReward() && quest.questDataSo.rewardType == npcType)
//                getRewardQuest.Add(quest);
//        }

//        foreach (var quest in getRewardQuest)
//        {
//            GiveQuestReward(quest.questDataSo);  
//            CompleteQuest(quest);
//        }
//    }

//    private void GiveQuestReward(QuestDataSO questDataSo)
//    {
//        foreach (var item in questDataSo.rewardItems)
//        {
//            if(item == null || item.itemData == null) continue;

//            for(int i = 0; i < item.stackSize; i++)
//            {
//                dropManager.CreateItemDrop(item.itemData);
//            }
//        }
//    }

//    public bool HasComplatedQuest()
//    {
//        for(int i = 0; i < activeQuests.Count; i++)
//        {
//            // Delivery quest check

//            QuestData quest = activeQuests[i];

//            if (quest.questDataSo.questType != QuestType.Delivery)
//            {
//                var requiredItem = quest.questDataSo.itemToDelivery;
//                var requiredAmount = quest.questDataSo.requiredAmount;

//                if (inventory.HasItemAmount(requiredItem, requiredAmount))
//                    return true;

//            }

//            if (quest.CanGetReward())
//                return true;
//        }
//        return false;
//    }

//    public void AddProgress(string questTargetID, int amount = 1)
//    {
//        List<QuestData> getRewardQuests = new List<QuestData>();

//        foreach (var quest in activeQuests)
//        {
//            if (quest.questDataSo.questTargetID != questTargetID) 
//                continue;

//            if(quest.CanGetReward())
//                quest.AddQuestProgress(amount);

//            if (quest.questDataSo.rewardType == RewardType.None && quest.CanGetReward())
//                getRewardQuests.Add(quest);
//        }
//        foreach (var quest in getRewardQuests)
//        {
//            GiveQuestReward(quest.questDataSo);    
//            CompleteQuest(quest);
//        }
//    }

//    public void ActiveQuest(QuestDataSO questDataSo)
//    {
//        activeQuests.Add(new QuestData(questDataSo));
//    }

//    public bool QuestIsActive(QuestDataSO questToCheck)
//    {
//        if(questToCheck == null)
//            return false;

//        return activeQuests.Find(q => q.questDataSo == questToCheck) != null;
//    }

//    public int GetQuestProgress(QuestData questToCheck)
//    {
//        QuestData quest = activeQuests.Find(q => q == questToCheck);
//        return quest != null ? quest.currentAmount : 0;
//    }

//    public void CompleteQuest(QuestData questData)
//    {
//        complateQuests.Add(questData);
//        activeQuests.Remove(questData);
//    }

//    //public void LoadData(GameData gameData)
//    //{
//    //    gameData.activeQuests.Clear();

//    //    foreach (var entry in gameData.activeQuests)
//    //    {
//    //        string questSaveID = entry.Key;
//    //        int progress = entry.Value;

//    //        QuestDataSO questDataSo = questDatabase.GetQuestById(questSaveID);

//    //        if (questDataSo == null)
//    //        {
//    //            Debug.LogWarning($"Quest with ID {questSaveID} not found in the database.");
//    //            continue;
//    //        }

//    //        QuestData questToLoad = new QuestData(questDataSo);
//    //        questToLoad.currentAmount = progress;

//    //        activeQuests.Add(questToLoad);
//    //    }
//    //}

//    public void LoadData(GameData gameData)
//    {
//        activeQuests.Clear();
//        complateQuests.Clear();

//        foreach (var entry in gameData.activeQuests)
//        {
//            string questSaveID = entry.Key;
//            int progress = entry.Value;

//            QuestDataSO questDataSo = questDatabase.GetQuestById(questSaveID);

//            if (questDataSo == null)
//            {
//                Debug.LogWarning($"Quest with ID {questSaveID} not found in the database.");
//                continue;
//            }

//            QuestData questToLoad = new QuestData(questDataSo);
//            questToLoad.currentAmount = progress;

//            activeQuests.Add(questToLoad);
//        }

//        foreach (var entry in gameData.completedQuests)
//        {
//            string questSaveID = entry.Key;

//            QuestDataSO questDataSo = questDatabase.GetQuestById(questSaveID);

//            if (questDataSo == null)
//            {
//                Debug.LogWarning($"Completed quest with ID {questSaveID} not found in the database.");
//                continue;
//            }

//            QuestData questToLoad = new QuestData(questDataSo);
//            questToLoad.currentAmount = questDataSo.requiredAmount;
//            questToLoad.canGetReward = true;

//            complateQuests.Add(questToLoad);
//        }
//    }

//    //public void SaveData(ref GameData gameData)
//    //{
//    //    foreach(var quest in activeQuests)
//    //    {
//    //        gameData.activeQuests.Add(quest.questDataSo.questSaveID, quest.currentAmount);
//    //    }

//    //    foreach (var quest in complateQuests)
//    //    {
//    //        gameData.completedQuests.Add(quest.questDataSo.questSaveID, true);
//    //    }
//    //}
//    public void SaveData(ref GameData gameData)
//    {
//        gameData.activeQuests.Clear();
//        gameData.completedQuests.Clear();

//        foreach (var quest in activeQuests)
//        {
//            if (quest == null || quest.questDataSo == null)
//                continue;

//            gameData.activeQuests[quest.questDataSo.questSaveID] = quest.currentAmount;
//        }

//        foreach (var quest in complateQuests)
//        {
//            if (quest == null || quest.questDataSo == null)
//                continue;

//            gameData.completedQuests[quest.questDataSo.questSaveID] = true;
//        }

//        Debug.Log($"Saved {activeQuests.Count} active quests and {complateQuests.Count} completed quests");
//    }

//}























//using System.Collections.Generic;
//using UnityEngine;

//public class Player_QuestManager : MonoBehaviour, ISaveable
//{
//    public List<QuestData> activeQuests;
//    public List<QuestData> complateQuests;

//    private Entity_DropManager dropManager;
//    private Inventory_Player inventory;

//    [Header("Quest Database")]
//    [SerializeField] private QuestDatabaseSO questDatabase;

//    private void Awake()
//    {
//        dropManager = GetComponent<Entity_DropManager>();
//        inventory = GetComponent<Inventory_Player>();
//    }

//    public void TryGetRewardFrom(RewardType npcType)
//    {
//        List<QuestData> getRewardQuest = new List<QuestData>();

//        foreach (var quest in activeQuests)
//        {
//            // Delivery quest check
//            if (quest.questDataSo.questType == QuestType.Delivery)
//            {
//                var requiredItem = quest.questDataSo.itemToDelivery;
//                var requiredAmount = quest.questDataSo.requiredAmount;

//                if (inventory.HasItemAmount(requiredItem, requiredAmount))
//                {
//                    inventory.RemoveItemAmount(requiredItem, requiredAmount);
//                    quest.AddQuestProgress(requiredAmount);
//                }
//            }
//            if (quest.CanGetReward() && quest.questDataSo.rewardType == npcType)
//                getRewardQuest.Add(quest);
//        }

//        foreach (var quest in getRewardQuest)
//        {
//            GiveQuestReward(quest.questDataSo);
//            CompleteQuest(quest);
//        }
//    }

//    private void GiveQuestReward(QuestDataSO questDataSo)
//    {
//        foreach (var item in questDataSo.rewardItems)
//        {
//            if (item == null || item.itemData == null) continue;

//            for (int i = 0; i < item.stackSize; i++)
//            {
//                dropManager.CreateItemDrop(item.itemData);
//            }
//        }
//    }

//    public bool HasComplatedQuest()
//    {
//        for (int i = 0; i < activeQuests.Count; i++)
//        {
//            // Delivery quest check

//            QuestData quest = activeQuests[i];

//            if (quest.questDataSo.questType == QuestType.Delivery)
//            {
//                var requiredItem = quest.questDataSo.itemToDelivery;
//                var requiredAmount = quest.questDataSo.requiredAmount;

//                if (inventory.HasItemAmount(requiredItem, requiredAmount))
//                    return true;

//            }

//            if (quest.CanGetReward())
//                return true;
//        }
//        return false;
//    }

//    public void AddProgress(string questTargetID, int amount = 1)
//    {
//        List<QuestData> getRewardQuests = new List<QuestData>();

//        foreach (var quest in activeQuests)
//        {
//            if (quest.questDataSo.questTargetID != questTargetID)
//                continue;

//            if (quest.CanGetReward() == false)
//                quest.AddQuestProgress(amount);

//            if (quest.questDataSo.rewardType == RewardType.None && quest.CanGetReward())
//                getRewardQuests.Add(quest);
//        }
//        foreach (var quest in getRewardQuests)
//        {
//            GiveQuestReward(quest.questDataSo);
//            CompleteQuest(quest);
//        }
//    }

//    public void ActiveQuest(QuestDataSO questDataSo)
//    {
//        activeQuests.Add(new QuestData(questDataSo));
//    }

//    public bool QuestIsActive(QuestDataSO questToCheck)
//    {
//        if (questToCheck == null)
//            return false;

//        return activeQuests.Find(q => q.questDataSo == questToCheck) != null;
//    }

//    public int GetQuestProgress(QuestData questToCheck)
//    {
//        QuestData quest = activeQuests.Find(q => q == questToCheck);
//        return quest != null ? quest.currentAmount : 0;
//    }

//    public void CompleteQuest(QuestData questData)
//    {
//        complateQuests.Add(questData);
//        activeQuests.Remove(questData);
//    }

//    public void LoadData(GameData gameData)
//    {
//        activeQuests.Clear();
//        complateQuests.Clear();

//        foreach (var entry in gameData.activeQuests)
//        {
//            string questSaveID = entry.Key;
//            int progress = entry.Value;

//            QuestDataSO questDataSo = questDatabase.GetQuestById(questSaveID);

//            if (questDataSo == null)
//            {
//                Debug.LogWarning($"Quest with ID {questSaveID} not found in the database.");
//                continue;
//            }

//            QuestData questToLoad = new QuestData(questDataSo);
//            questToLoad.currentAmount = progress;

//            activeQuests.Add(questToLoad);
//        }

//        foreach (var entry in gameData.completedQuests)
//        {
//            string questSaveID = entry.Key;

//            QuestDataSO questDataSo = questDatabase.GetQuestById(questSaveID);

//            if (questDataSo == null)
//            {
//                Debug.LogWarning($"Completed quest with ID {questSaveID} not found in the database.");
//                continue;
//            }

//            QuestData questToLoad = new QuestData(questDataSo);
//            questToLoad.currentAmount = questDataSo.requiredAmount;
//            questToLoad.canGetReward = true;

//            complateQuests.Add(questToLoad);
//        }
//    }

//    public void SaveData(ref GameData gameData)
//    {
//        gameData.activeQuests.Clear();
//        gameData.completedQuests.Clear();

//        foreach (var quest in activeQuests)
//        {
//            if (quest == null || quest.questDataSo == null)
//                continue;

//            gameData.activeQuests[quest.questDataSo.questSaveID] = quest.currentAmount;
//        }

//        foreach (var quest in complateQuests)
//        {
//            if (quest == null || quest.questDataSo == null)
//                continue;

//            gameData.completedQuests[quest.questDataSo.questSaveID] = true;
//        }

//        Debug.Log($"Saved {activeQuests.Count} active quests and {complateQuests.Count} completed quests");
//    }

//}




using System.Collections.Generic;
using UnityEngine;

public class Player_QuestManager : MonoBehaviour, ISaveable
{
    public List<QuestData> activeQuests = new List<QuestData>();
    public List<QuestData> completedQuests = new List<QuestData>();

    private Entity_DropManager dropManager;
    private Inventory_Player inventory;

    [Header("Quest Database")]
    [SerializeField] private QuestDatabaseSO questDatabase;

    // Events for UI updates
    public System.Action<QuestData> OnQuestAccepted;
    public System.Action<QuestData> OnQuestCompleted;
    public System.Action<QuestData, QuestObjectiveData> OnObjectiveProgress;
    public System.Action<QuestData, QuestObjectiveData> OnObjectiveCompleted;

    private void Awake()
    {
        dropManager = GetComponent<Entity_DropManager>();
        inventory = GetComponent<Inventory_Player>();
    }

    #region Inventory Helpers

    /// <summary>
    /// Check if inventory has enough of an item
    /// </summary>
    private bool HasItemAmount(ItemDataSO itemData, int amount)
    {
        if (inventory == null || itemData == null) return false;

        int count = 0;
        foreach (var item in inventory.itemList)
        {
            if (item != null && item.itemData != null && item.itemData.saveID == itemData.saveID)
            {
                count += item.stackSize;
            }
        }

        return count >= amount;
    }

    /// <summary>
    /// Remove a specific amount of an item from inventory
    /// </summary>
    private void RemoveItemAmount(ItemDataSO itemData, int amount)
    {
        if (inventory == null || itemData == null) return;

        int remaining = amount;

        // Find and remove items
        for (int i = inventory.itemList.Count - 1; i >= 0 && remaining > 0; i--)
        {
            var item = inventory.itemList[i];

            if (item != null && item.itemData != null && item.itemData.saveID == itemData.saveID)
            {
                if (item.stackSize <= remaining)
                {
                    remaining -= item.stackSize;
                    inventory.itemList.RemoveAt(i);
                }
                else
                {
                    item.stackSize -= remaining;
                    remaining = 0;
                }
            }
        }

        inventory.TriggerUpdateUI();
        Debug.Log($"[Quest] Removed {amount - remaining}x {itemData.itemName} from inventory");
    }

    #endregion

    /// <summary>
    /// Activate a new quest
    /// </summary>
    public bool ActivateQuest(QuestDataSO questDataSo)
    {
        if (questDataSo == null) return false;

        // Check if already active or completed
        if (QuestIsActive(questDataSo) || QuestIsCompleted(questDataSo))
            return false;

        // Check prerequisite
        if (questDataSo.prerequisiteQuest != null && !QuestIsCompleted(questDataSo.prerequisiteQuest))
        {
            Debug.Log($"Cannot accept quest '{questDataSo.questName}': Prerequisite not met.");
            return false;
        }

        var newQuest = new QuestData(questDataSo);
        activeQuests.Add(newQuest);

        OnQuestAccepted?.Invoke(newQuest);
        Debug.Log($"Quest activated: {questDataSo.questName}");

        return true;
    }

    /// <summary>
    /// Backward compatible method name
    /// </summary>
    public void ActiveQuest(QuestDataSO questDataSo)
    {
        ActivateQuest(questDataSo);
    }


    /// <summary>
    /// Add progress for simple quests or specific targets (backward compatible)
    /// </summary>
    public void AddProgress(string questTargetID, int amount = 1)
    {
        List<QuestData> getRewardQuests = new List<QuestData>();

        Debug.Log($"[Quest] AddProgress called - targetID: {questTargetID}, amount: {amount}");
        Debug.Log($"[Quest] Active quests count: {activeQuests.Count}");

        foreach (var quest in activeQuests)
        {
            bool progressMade = false;

            // Try complex quest progress first
            if (quest.questDataSo.HasMultipleObjectives())
            {
                progressMade = quest.AddObjectiveProgress(questTargetID, amount);

                // Check for objective completion events
                if (progressMade)
                {
                    Debug.Log($"[Quest] Progress made on complex quest: {quest.questDataSo.questName}");
                    var objData = quest.FindObjectiveData(questTargetID);
                    if (objData != null)
                    {
                        OnObjectiveProgress?.Invoke(quest, objData);

                        if (objData.isCompleted)
                        {
                            OnObjectiveCompleted?.Invoke(quest, objData);
                        }
                    }
                }
            }
            else
            {
                // Simple quest progress - use new helper method
                string targetID = quest.questDataSo.GetSimpleQuestTargetID();

                Debug.Log($"[Quest] Checking simple quest '{quest.questDataSo.questName}' - targetID: {targetID}, looking for: {questTargetID}");

                if (targetID == questTargetID)
                {
                    if (!quest.CanGetReward())
                    {
                        int before = quest.currentAmount;
                        quest.AddQuestProgress(amount);
                        Debug.Log($"[Quest] Progress: {before} -> {quest.currentAmount}/{quest.GetRequiredAmount()} for quest '{quest.questDataSo.questName}'");
                        progressMade = true;
                    }
                    else
                    {
                        Debug.Log($"[Quest] Quest '{quest.questDataSo.questName}' already complete, no progress added");
                    }
                }
            }

            // Check if quest is now completable with no turn-in required
            if (quest.questDataSo.rewardType == RewardType.None && quest.CanGetReward())
            {
                getRewardQuests.Add(quest);
            }
        }

        // Auto-complete quests with no reward NPC
        foreach (var quest in getRewardQuests)
        {
            GiveQuestReward(quest.questDataSo);
            CompleteQuest(quest);
        }
    }

    /// <summary>
    /// Add progress by objective type (for complex quests)
    /// </summary>
    public void AddProgress(QuestObjectiveType objectiveType, string targetID, int amount = 1)
    {
        foreach (var quest in activeQuests)
        {
            if (!quest.questDataSo.HasMultipleObjectives())
            {
                // For simple quests, check target ID match
                if (quest.questDataSo.questTargetID == targetID)
                {
                    AddProgress(targetID, amount);
                }
                continue;
            }

            if (quest.AddObjectiveProgress(objectiveType, targetID, amount))
            {
                var objData = quest.FindObjectiveData(targetID);
                if (objData != null)
                {
                    OnObjectiveProgress?.Invoke(quest, objData);

                    if (objData.isCompleted)
                    {
                        OnObjectiveCompleted?.Invoke(quest, objData);
                    }
                }

                // Auto-complete if applicable
                if (quest.questDataSo.rewardType == RewardType.None && quest.CanGetReward())
                {
                    GiveQuestReward(quest.questDataSo);
                    CompleteQuest(quest);
                }
            }
        }
    }

    /// <summary>
    /// Complete a talk/visit objective
    /// </summary>
    public void CompleteObjective(string targetID)
    {
        AddProgress(targetID, 999);
    }

    #region Reward Collection

    /// <summary>
    /// Try to get rewards from NPC by reward type (backward compatible)
    /// </summary>
    public void TryGetRewardFrom(RewardType npcType)
    {
        Debug.Log($"[Quest] TryGetRewardFrom called with rewardType: {npcType}");
        Debug.Log($"[Quest] Active quests count: {activeQuests.Count}");

        List<QuestData> getRewardQuest = new List<QuestData>();

        foreach (var quest in activeQuests)
        {
            Debug.Log($"[Quest] Checking quest: {quest.questDataSo.questName}");
            Debug.Log($"[Quest]   - CanGetReward: {quest.CanGetReward()}");
            Debug.Log($"[Quest]   - Quest RewardType: {quest.questDataSo.rewardType}");

            // Handle delivery quests
            if (quest.questDataSo.IsDeliveryQuest() || HasDeliveryObjectives(quest))
            {
                TryCompleteDeliveryObjectives(quest);
            }

            if (quest.CanGetReward() && quest.questDataSo.rewardType == npcType)
            {
                Debug.Log($"[Quest] Quest '{quest.questDataSo.questName}' ready for turn-in!");
                getRewardQuest.Add(quest);
            }
        }

        Debug.Log($"[Quest] Quests to complete: {getRewardQuest.Count}");

        foreach (var quest in getRewardQuest)
        {
            GiveQuestReward(quest.questDataSo);
            CompleteQuest(quest);
        }
    }

    /// <summary>
    /// Try to get rewards from specific NPC by ID
    /// </summary>
    public void TryGetRewardFromNpc(string npcID)
    {
        Debug.Log($"[Quest] TryGetRewardFromNpc called with npcID: {npcID}");
        Debug.Log($"[Quest] Active quests count: {activeQuests.Count}");

        List<QuestData> getRewardQuest = new List<QuestData>();

        foreach (var quest in activeQuests)
        {
            Debug.Log($"[Quest] Checking quest: {quest.questDataSo.questName}");
            Debug.Log($"[Quest]   - CanGetReward: {quest.CanGetReward()}");
            Debug.Log($"[Quest]   - RewardGiverNpcID: {quest.questDataSo.rewardGiverNpcID}");
            Debug.Log($"[Quest]   - CanNpcGiveReward({npcID}): {quest.questDataSo.CanNpcGiveReward(npcID)}");

            // Handle delivery quests to this NPC
            TryCompleteDeliveryObjectives(quest, npcID);

            // Check if can turn in
            if (quest.CanGetReward())
            {
                // Check by NPC ID first
                if (quest.questDataSo.CanNpcGiveReward(npcID))
                {
                    Debug.Log($"[Quest] Quest '{quest.questDataSo.questName}' ready for turn-in!");
                    getRewardQuest.Add(quest);
                    continue;
                }

                // Check if reward giver matches
                if (!string.IsNullOrEmpty(quest.questDataSo.rewardGiverNpcID) &&
                    quest.questDataSo.rewardGiverNpcID == npcID)
                {
                    Debug.Log($"[Quest] Quest '{quest.questDataSo.questName}' ready for turn-in (by rewardGiverNpcID)!");
                    getRewardQuest.Add(quest);
                }
            }
        }

        Debug.Log($"[Quest] Quests to complete: {getRewardQuest.Count}");

        foreach (var quest in getRewardQuest)
        {
            GiveQuestReward(quest.questDataSo);
            CompleteQuest(quest);
        }
    }

    /// <summary>
    /// Check if quest has delivery objectives
    /// </summary>
    private bool HasDeliveryObjectives(QuestData quest)
    {
        return quest.questDataSo.IsDeliveryQuest();
    }

    /// <summary>
    /// Try to complete delivery objectives
    /// </summary>
    private void TryCompleteDeliveryObjectives(QuestData quest, string specificNpcID = null)
    {
        if (!quest.questDataSo.HasMultipleObjectives())
        {
            // Simple delivery quest
            if (quest.questDataSo.IsDeliveryQuest())
            {
                var requiredItem = quest.questDataSo.GetDeliveryItem();
                var requiredAmount = quest.questDataSo.GetSimpleQuestRequiredAmount();

                // Check if delivering to correct NPC
                if (specificNpcID != null)
                {
                    string deliverTarget = quest.questDataSo.simpleObjectiveType == QuestObjectiveType.Deliver
                        ? quest.questDataSo.deliverTargetNpcID
                        : quest.questDataSo.GetSimpleQuestTargetID();

                    if (deliverTarget != specificNpcID)
                        return;
                }

                if (requiredItem != null && HasItemAmount(requiredItem, requiredAmount))
                {
                    RemoveItemAmount(requiredItem, requiredAmount);
                    quest.AddQuestProgress(requiredAmount);
                }
            }
            return;
        }

        // Complex quest delivery objectives
        foreach (var objData in quest.objectiveProgress)
        {
            if (objData.objectiveSO.objectiveType != QuestObjectiveType.Deliver)
                continue;

            if (objData.isCompleted)
                continue;

            // Check if this is the right NPC
            if (specificNpcID != null && objData.objectiveSO.turnInNpcID != specificNpcID)
                continue;

            var requiredItem = objData.objectiveSO.requiredItem;
            var requiredAmount = objData.objectiveSO.requiredAmount;

            if (requiredItem != null && HasItemAmount(requiredItem, requiredAmount))
            {
                RemoveItemAmount(requiredItem, requiredAmount);
                objData.AddProgress(requiredAmount);

                OnObjectiveProgress?.Invoke(quest, objData);
                if (objData.isCompleted)
                {
                    OnObjectiveCompleted?.Invoke(quest, objData);
                }
            }
        }
    }

    /// <summary>
    /// Give quest rewards to player
    /// </summary>
    private void GiveQuestReward(QuestDataSO questDataSo)
    {
        if (questDataSo.rewardItems == null || questDataSo.rewardItems.Length == 0)
        {
            Debug.Log($"[Quest] Quest '{questDataSo.questName}' has no reward items.");
            return;
        }

        Debug.Log($"[Quest] Giving {questDataSo.rewardItems.Length} reward(s) for quest: {questDataSo.questName}");

        foreach (var item in questDataSo.rewardItems)
        {
            if (item == null || item.itemData == null)
            {
                Debug.LogWarning($"[Quest] Quest '{questDataSo.questName}' has null reward item!");
                continue;
            }

            Debug.Log($"[Quest] Processing reward: {item.itemData.itemName} x{item.stackSize}");

            // Try to add directly to inventory first
            if (inventory != null)
            {
                // Try different common method signatures
                bool added = TryAddToInventory(item.itemData, item.stackSize);
                if (added)
                {
                    Debug.Log($"[Quest] Added {item.stackSize}x {item.itemData.itemName} to inventory");
                }
                else
                {
                    Debug.LogWarning($"[Quest] Failed to add item to inventory - check Inventory_Player.AddItem() method");
                }
            }
            // Fallback: drop items on ground
            else if (dropManager != null)
            {
                for (int i = 0; i < item.stackSize; i++)
                {
                    dropManager.CreateItemDrop(item.itemData);
                }
                Debug.Log($"[Quest] Dropped {item.stackSize}x {item.itemData.itemName}");
            }
            else
            {
                Debug.LogError($"[Quest] Cannot give reward! Both inventory and dropManager are null!");
            }
        }

        Debug.Log($"[Quest] All rewards given for: {questDataSo.questName}");
    }

    /// <summary>
    /// Try to add item to inventory using available methods
    /// </summary>
    private bool TryAddToInventory(ItemDataSO itemData, int amount)
    {
        if (inventory == null) return false;

        try
        {
            // Your inventory uses Inventory_Item objects, not raw ItemDataSO
            // Create new Inventory_Item for each stack
            for (int i = 0; i < amount; i++)
            {
                Inventory_Item newItem = new Inventory_Item(itemData);
                inventory.AddItem(newItem);
            }
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Quest] Error adding item to inventory: {e.Message}");
            return false;
        }
    }

    #endregion

    #region Quest Status Queries

    /// <summary>
    /// Check if any quest is completable
    /// </summary>
    public bool HasCompletedQuest()
    {
        foreach (var quest in activeQuests)
        {
            // Check delivery requirements
            if (HasDeliveryObjectives(quest))
            {
                if (CanCompleteDeliveryQuest(quest))
                    return true;
            }

            if (quest.CanGetReward())
                return true;
        }
        return false;
    }

    /// <summary>
    /// Check if any quest is completable for specific reward type
    /// </summary>
    public bool HasCompletedQuestFor(RewardType rewardType)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.questDataSo.rewardType != rewardType)
                continue;

            if (HasDeliveryObjectives(quest) && CanCompleteDeliveryQuest(quest))
                return true;

            if (quest.CanGetReward())
                return true;
        }
        return false;
    }

    /// <summary>
    /// Check if any quest is completable for specific NPC
    /// </summary>
    public bool HasCompletedQuestForNpc(string npcID)
    {
        foreach (var quest in activeQuests)
        {
            if (!quest.CanGetReward()) continue;

            // Check by reward giver NPC ID
            if (!string.IsNullOrEmpty(quest.questDataSo.rewardGiverNpcID) &&
                quest.questDataSo.rewardGiverNpcID == npcID)
                return true;

            // Check additional reward givers
            if (quest.questDataSo.CanNpcGiveReward(npcID))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Get all quests that can be turned in to a specific NPC
    /// </summary>
    public QuestData[] GetTurnInableQuestsForNpc(string npcID)
    {
        var result = new List<QuestData>();

        foreach (var quest in activeQuests)
        {
            if (!quest.CanGetReward()) continue;

            // Check by reward giver NPC ID
            if (!string.IsNullOrEmpty(quest.questDataSo.rewardGiverNpcID) &&
                quest.questDataSo.rewardGiverNpcID == npcID)
            {
                result.Add(quest);
                continue;
            }

            // Check additional reward givers
            if (quest.questDataSo.CanNpcGiveReward(npcID))
            {
                result.Add(quest);
            }
        }

        return result.ToArray();
    }

    /// <summary>
    /// Get all quests that can be turned in to a specific reward type
    /// </summary>
    public QuestData[] GetTurnInableQuestsForType(RewardType rewardType)
    {
        var result = new List<QuestData>();

        foreach (var quest in activeQuests)
        {
            if (quest.questDataSo.rewardType != rewardType)
                continue;

            if (quest.CanGetReward())
            {
                result.Add(quest);
            }
        }

        return result.ToArray();
    }

    /// <summary>
    /// Check if delivery quest can be completed
    /// </summary>
    private bool CanCompleteDeliveryQuest(QuestData quest)
    {
        if (!quest.questDataSo.HasMultipleObjectives())
        {
            if (!quest.questDataSo.IsDeliveryQuest())
                return false;

            var deliveryItem = quest.questDataSo.GetDeliveryItem();
            var requiredAmount = quest.questDataSo.GetSimpleQuestRequiredAmount();

            return deliveryItem != null && HasItemAmount(deliveryItem, requiredAmount);
        }

        foreach (var objData in quest.objectiveProgress)
        {
            if (objData.objectiveSO.objectiveType != QuestObjectiveType.Deliver)
                continue;

            if (objData.isCompleted)
                continue;

            var requiredItem = objData.objectiveSO.requiredItem;
            var requiredAmount = objData.objectiveSO.requiredAmount;

            if (requiredItem != null && HasItemAmount(requiredItem, requiredAmount))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Check if quest is active
    /// </summary>
    public bool QuestIsActive(QuestDataSO questToCheck)
    {
        if (questToCheck == null) return false;
        return activeQuests.Find(q => q.questDataSo == questToCheck) != null;
    }

    /// <summary>
    /// Check if quest is completed
    /// </summary>
    public bool QuestIsCompleted(QuestDataSO questToCheck)
    {
        if (questToCheck == null) return false;
        return completedQuests.Find(q => q.questDataSo == questToCheck) != null;
    }

    /// <summary>
    /// Get progress for specific quest (backward compatible)
    /// </summary>
    public int GetQuestProgress(QuestData questToCheck)
    {
        QuestData quest = activeQuests.Find(q => q == questToCheck);
        return quest != null ? quest.currentAmount : 0;
    }

    /// <summary>
    /// Get all active objectives for a quest
    /// </summary>
    public List<QuestObjectiveData> GetActiveObjectives(QuestData quest)
    {
        if (!quest.questDataSo.HasMultipleObjectives())
            return new List<QuestObjectiveData>();

        return quest.GetIncompleteObjectives();
    }

    /// <summary>
    /// Find quest by target ID
    /// </summary>
    public QuestData FindQuestByTarget(string targetID)
    {
        foreach (var quest in activeQuests)
        {
            if (!quest.questDataSo.HasMultipleObjectives())
            {
                if (quest.questDataSo.questTargetID == targetID)
                    return quest;
            }
            else
            {
                if (quest.FindObjectiveData(targetID) != null)
                    return quest;
            }
        }
        return null;
    }

    #endregion

    #region Quest Completion

    /// <summary>
    /// Complete a quest and move to completed list
    /// </summary>
    public void CompleteQuest(QuestData questData)
    {
        activeQuests.Remove(questData);
        completedQuests.Add(questData);

        OnQuestCompleted?.Invoke(questData);
        Debug.Log($"Quest completed: {questData.questDataSo.questName}");

        // Auto-activate next quest in chain
        if (questData.questDataSo.nextQuestInChain != null)
        {
            ActivateQuest(questData.questDataSo.nextQuestInChain);
        }
    }

    #endregion

    #region Save/Load

    public void LoadData(GameData gameData)
    {
        activeQuests.Clear();
        completedQuests.Clear();

        // Load active quests
        foreach (var entry in gameData.activeQuests)
        {
            string questSaveID = entry.Key;
            int progress = entry.Value;

            QuestDataSO questDataSo = questDatabase.GetQuestById(questSaveID);
            if (questDataSo == null)
            {
                Debug.LogWarning($"Quest with ID {questSaveID} not found in database.");
                continue;
            }

            QuestData questToLoad = new QuestData(questDataSo);
            questToLoad.currentAmount = progress;

            // Load complex quest progress if available
            if (gameData.questObjectiveProgress != null &&
                gameData.questObjectiveProgress.TryGetValue(questSaveID, out var objectiveData))
            {
                questToLoad.LoadSerializableProgress(objectiveData);
            }

            activeQuests.Add(questToLoad);
        }

        // Load completed quests
        foreach (var entry in gameData.completedQuests)
        {
            string questSaveID = entry.Key;

            QuestDataSO questDataSo = questDatabase.GetQuestById(questSaveID);
            if (questDataSo == null)
            {
                Debug.LogWarning($"Completed quest with ID {questSaveID} not found in database.");
                continue;
            }

            QuestData questToLoad = new QuestData(questDataSo);
            questToLoad.currentAmount = questDataSo.requiredAmount;
            questToLoad.canGetReward = true;

            completedQuests.Add(questToLoad);
        }
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.activeQuests.Clear();
        gameData.completedQuests.Clear();

        // Initialize objective progress dictionary if needed
        if (gameData.questObjectiveProgress == null)
        {
            gameData.questObjectiveProgress = new SerializableDictionary<string, SerializableDictionary<string, int>>();
        }
        gameData.questObjectiveProgress.Clear();

        foreach (var quest in activeQuests)
        {
            if (quest == null || quest.questDataSo == null) continue;

            string saveID = quest.questDataSo.questSaveID;
            gameData.activeQuests[saveID] = quest.currentAmount;

            // Save complex quest objective progress
            if (quest.questDataSo.HasMultipleObjectives())
            {
                var progressDict = quest.GetSerializableProgress();
                var serializableDict = new SerializableDictionary<string, int>();
                foreach (var kvp in progressDict)
                {
                    serializableDict[kvp.Key] = kvp.Value;
                }
                gameData.questObjectiveProgress[saveID] = serializableDict;
            }
        }

        foreach (var quest in completedQuests)
        {
            if (quest == null || quest.questDataSo == null) continue;

            gameData.completedQuests[quest.questDataSo.questSaveID] = true;
        }

        Debug.Log($"Saved {activeQuests.Count} active quests and {completedQuests.Count} completed quests");
    }

    #endregion
}