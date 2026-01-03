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

    private void Start()
    {
        // Double-check inventory reference
        if (inventory == null)
        {
            inventory = GetComponent<Inventory_Player>();
            if (inventory == null)
            {
                inventory = FindFirstObjectByType<Inventory_Player>();
            }
        }
    }

    #region Inventory Helpers

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

    private void RemoveItemAmount(ItemDataSO itemData, int amount)
    {
        if (inventory == null || itemData == null) return;

        int remaining = amount;

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

    #region Quest Activation

    public bool ActivateQuest(QuestDataSO questDataSo)
    {
        if (questDataSo == null) return false;

        if (QuestIsActive(questDataSo) || QuestIsCompleted(questDataSo))
            return false;

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

    public void ActiveQuest(QuestDataSO questDataSo)
    {
        ActivateQuest(questDataSo);
    }

    #endregion

    #region Quest Progress

    public void AddProgress(string questTargetID, int amount = 1)
    {
        List<QuestData> getRewardQuests = new List<QuestData>();

        Debug.Log($"[Quest] AddProgress called - targetID: {questTargetID}, amount: {amount}");

        foreach (var quest in activeQuests)
        {
            bool progressMade = false;

            if (quest.questDataSo.HasMultipleObjectives())
            {
                progressMade = quest.AddObjectiveProgress(questTargetID, amount);

                if (progressMade)
                {
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
                string targetID = quest.questDataSo.GetSimpleQuestTargetID();

                if (targetID == questTargetID)
                {
                    if (!quest.CanGetReward())
                    {
                        quest.AddQuestProgress(amount);
                        progressMade = true;
                    }
                }
            }

            if (quest.questDataSo.rewardType == RewardType.None && quest.CanGetReward())
            {
                getRewardQuests.Add(quest);
            }
        }

        foreach (var quest in getRewardQuests)
        {
            GiveQuestReward(quest.questDataSo);
            CompleteQuest(quest);
        }
    }

    public void AddProgress(QuestObjectiveType objectiveType, string targetID, int amount = 1)
    {
        foreach (var quest in activeQuests)
        {
            if (!quest.questDataSo.HasMultipleObjectives())
            {
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

                if (quest.questDataSo.rewardType == RewardType.None && quest.CanGetReward())
                {
                    GiveQuestReward(quest.questDataSo);
                    CompleteQuest(quest);
                }
            }
        }
    }

    public void CompleteObjective(string targetID)
    {
        AddProgress(targetID, 999);
    }

    #endregion

    #region Reward Collection

    public void TryGetRewardFrom(RewardType npcType)
    {
        Debug.Log($"[Quest] TryGetRewardFrom called with rewardType: {npcType}");

        List<QuestData> getRewardQuest = new List<QuestData>();

        foreach (var quest in activeQuests)
        {
            if (HasDeliveryObjectives(quest))
            {
                TryCompleteDeliveryObjectives(quest);
            }

            if (quest.CanGetReward() && quest.questDataSo.rewardType == npcType)
            {
                Debug.Log($"[Quest] Quest '{quest.questDataSo.questName}' ready for turn-in!");
                getRewardQuest.Add(quest);
            }
        }

        foreach (var quest in getRewardQuest)
        {
            GiveQuestReward(quest.questDataSo);
            CompleteQuest(quest);
        }
    }

    public void TryGetRewardFromNpc(string npcID)
    {
        Debug.Log($"[Quest] TryGetRewardFromNpc called with npcID: {npcID}");

        List<QuestData> getRewardQuest = new List<QuestData>();

        foreach (var quest in activeQuests)
        {
            TryCompleteDeliveryObjectives(quest, npcID);

            if (quest.CanGetReward())
            {
                if (quest.questDataSo.CanNpcGiveReward(npcID))
                {
                    getRewardQuest.Add(quest);
                    continue;
                }

                if (!string.IsNullOrEmpty(quest.questDataSo.rewardGiverNpcID) &&
                    quest.questDataSo.rewardGiverNpcID == npcID)
                {
                    getRewardQuest.Add(quest);
                }
            }
        }

        foreach (var quest in getRewardQuest)
        {
            GiveQuestReward(quest.questDataSo);
            CompleteQuest(quest);
        }
    }

    public List<QuestData> GetCompletableQuests()
    {
        List<QuestData> completable = new List<QuestData>();

        foreach (var quest in activeQuests)
        {
            if (quest.CanGetReward())
            {
                completable.Add(quest);
            }
            else if (HasDeliveryObjectives(quest) && CanCompleteDeliveryQuest(quest))
            {
                completable.Add(quest);
            }
        }

        return completable;
    }

    public List<QuestData> GetCompletableQuestsForNpc(string npcID, RewardType rewardType)
    {
        List<QuestData> completable = new List<QuestData>();

        foreach (var quest in activeQuests)
        {
            if (!quest.CanGetReward() && !CanCompleteDeliveryQuest(quest))
                continue;

            // Check by NPC ID
            if (!string.IsNullOrEmpty(npcID))
            {
                if (quest.questDataSo.CanNpcGiveReward(npcID) ||
                    quest.questDataSo.rewardGiverNpcID == npcID)
                {
                    completable.Add(quest);
                    continue;
                }
            }

            // Check by reward type
            if (quest.questDataSo.rewardType == rewardType)
            {
                completable.Add(quest);
            }
        }

        return completable;
    }

    private bool HasDeliveryObjectives(QuestData quest)
    {
        return quest.questDataSo.IsDeliveryQuest();
    }

    private void TryCompleteDeliveryObjectives(QuestData quest, string specificNpcID = null)
    {
        if (!quest.questDataSo.HasMultipleObjectives())
        {
            if (quest.questDataSo.IsDeliveryQuest())
            {
                var requiredItem = quest.questDataSo.GetDeliveryItem();
                var requiredAmount = quest.questDataSo.GetSimpleQuestRequiredAmount();

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

        foreach (var objData in quest.objectiveProgress)
        {
            if (objData.objectiveSO.objectiveType != QuestObjectiveType.Deliver)
                continue;

            if (objData.isCompleted)
                continue;

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

    private void GiveQuestReward(QuestDataSO questDataSo)
    {
        if (questDataSo.rewardItems == null || questDataSo.rewardItems.Length == 0)
        {
            Debug.Log($"[Quest] Quest '{questDataSo.questName}' has no reward items.");
            return;
        }

        // Ensure inventory reference is valid
        if (inventory == null)
        {
            inventory = GetComponent<Inventory_Player>();
            if (inventory == null)
            {
                inventory = FindFirstObjectByType<Inventory_Player>();
            }
        }

        Debug.Log($"[Quest] Giving {questDataSo.rewardItems.Length} reward(s) for quest: {questDataSo.questName}");
        Debug.Log($"[Quest] Inventory reference: {(inventory != null ? "VALID" : "NULL")}");

        foreach (var rewardItem in questDataSo.rewardItems)
        {
            if (rewardItem == null || rewardItem.itemData == null)
            {
                Debug.LogWarning($"[Quest] Quest '{questDataSo.questName}' has null reward item!");
                continue;
            }

            ItemDataSO itemData = rewardItem.itemData;
            int amount = rewardItem.stackSize;

            Debug.Log($"[Quest] Processing reward: {itemData.itemName} x{amount}");

            // Try to add directly to inventory
            if (inventory != null)
            {
                bool success = AddRewardToInventory(itemData, amount);

                if (success)
                {
                    Debug.Log($"[Quest] SUCCESS: Added {amount}x {itemData.itemName} to inventory");
                }
                else
                {
                    Debug.LogWarning($"[Quest] FAILED: Could not add {itemData.itemName} to inventory, dropping instead");
                    DropRewardItems(itemData, amount);
                }
            }
            else
            {
                Debug.LogWarning($"[Quest] No inventory found, dropping items");
                DropRewardItems(itemData, amount);
            }
        }

        // Force UI update
        if (inventory != null)
        {
            inventory.TriggerUpdateUI();
        }

        Debug.Log($"[Quest] All rewards processed for: {questDataSo.questName}");
    }
    private bool AddRewardToInventory(ItemDataSO itemData, int amount)
    {
        if (inventory == null || itemData == null) return false;

        try
        {
            // Check if it's a material - goes to storage stash
            if (itemData.itemType == ItemType.Material)
            {
                if (inventory.storage != null)
                {
                    for (int i = 0; i < amount; i++)
                    {
                        Inventory_Item newItem = new Inventory_Item(itemData);
                        inventory.storage.AddMaterialToStash(newItem);
                    }
                    Debug.Log($"[Quest] Added {amount}x {itemData.itemName} to material stash");
                    return true;
                }
            }

            // Regular items go to inventory
            for (int i = 0; i < amount; i++)
            {
                Inventory_Item newItem = new Inventory_Item(itemData);

                // Check if we can add
                if (inventory.CanAddItem(newItem))
                {
                    inventory.AddItem(newItem);
                }
                else
                {
                    // Inventory full - drop remaining
                    Debug.LogWarning($"[Quest] Inventory full! Dropping remaining {amount - i}x {itemData.itemName}");
                    DropRewardItems(itemData, amount - i);
                    return true; // Partial success
                }
            }

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Quest] Error adding reward to inventory: {e.Message}\n{e.StackTrace}");
            return false;
        }
    }

    private void DropRewardItems(ItemDataSO itemData, int amount)
    {
        if (dropManager == null)
        {
            dropManager = GetComponent<Entity_DropManager>();
        }

        if (dropManager != null)
        {
            for (int i = 0; i < amount; i++)
            {
                dropManager.CreateItemDrop(itemData);
            }
            Debug.Log($"[Quest] Dropped {amount}x {itemData.itemName}");
        }
        else
        {
            Debug.LogError($"[Quest] CRITICAL: Cannot give reward - no inventory and no drop manager!");
        }
    }

    #endregion

    #region Quest Status Queries

    public bool HasCompletedQuest()
    {
        foreach (var quest in activeQuests)
        {
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

    public bool HasCompletedQuestForNpc(string npcID)
    {
        foreach (var quest in activeQuests)
        {
            if (!quest.CanGetReward()) continue;

            if (!string.IsNullOrEmpty(quest.questDataSo.rewardGiverNpcID) &&
                quest.questDataSo.rewardGiverNpcID == npcID)
                return true;

            if (quest.questDataSo.CanNpcGiveReward(npcID))
                return true;
        }
        return false;
    }

    public QuestData[] GetTurnInableQuestsForNpc(string npcID)
    {
        var result = new List<QuestData>();

        foreach (var quest in activeQuests)
        {
            if (!quest.CanGetReward()) continue;

            if (!string.IsNullOrEmpty(quest.questDataSo.rewardGiverNpcID) &&
                quest.questDataSo.rewardGiverNpcID == npcID)
            {
                result.Add(quest);
                continue;
            }

            if (quest.questDataSo.CanNpcGiveReward(npcID))
            {
                result.Add(quest);
            }
        }

        return result.ToArray();
    }

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

    public bool QuestIsActive(QuestDataSO questToCheck)
    {
        if (questToCheck == null) return false;
        return activeQuests.Find(q => q.questDataSo == questToCheck) != null;
    }

    public bool QuestIsCompleted(QuestDataSO questToCheck)
    {
        if (questToCheck == null) return false;
        return completedQuests.Find(q => q.questDataSo == questToCheck) != null;
    }

    public int GetQuestProgress(QuestData questToCheck)
    {
        QuestData quest = activeQuests.Find(q => q == questToCheck);
        return quest != null ? quest.currentAmount : 0;
    }

    public List<QuestObjectiveData> GetActiveObjectives(QuestData quest)
    {
        if (!quest.questDataSo.HasMultipleObjectives())
            return new List<QuestObjectiveData>();

        return quest.GetIncompleteObjectives();
    }

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

    public void CompleteQuest(QuestData questData)
    {
        activeQuests.Remove(questData);
        completedQuests.Add(questData);

        OnQuestCompleted?.Invoke(questData);
        Debug.Log($"Quest completed: {questData.questDataSo.questName}");

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

            if (gameData.questObjectiveProgress != null &&
                gameData.questObjectiveProgress.TryGetValue(questSaveID, out var objectiveData))
            {
                questToLoad.LoadSerializableProgress(objectiveData);
            }

            activeQuests.Add(questToLoad);
        }

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