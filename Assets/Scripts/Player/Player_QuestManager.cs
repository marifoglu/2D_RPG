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























using System.Collections.Generic;
using UnityEngine;

public class Player_QuestManager : MonoBehaviour, ISaveable
{
    public List<QuestData> activeQuests;
    public List<QuestData> complateQuests;

    private Entity_DropManager dropManager;
    private Inventory_Player inventory;

    [Header("Quest Database")]
    [SerializeField] private QuestDatabaseSO questDatabase;

    private void Awake()
    {
        dropManager = GetComponent<Entity_DropManager>();
        inventory = GetComponent<Inventory_Player>();
    }

    public void TryGetRewardFrom(RewardType npcType)
    {
        List<QuestData> getRewardQuest = new List<QuestData>();

        foreach (var quest in activeQuests)
        {
            // Delivery quest check
            if (quest.questDataSo.questType == QuestType.Delivery)
            {
                var requiredItem = quest.questDataSo.itemToDelivery;
                var requiredAmount = quest.questDataSo.requiredAmount;

                if (inventory.HasItemAmount(requiredItem, requiredAmount))
                {
                    inventory.RemoveItemAmount(requiredItem, requiredAmount);
                    quest.AddQuestProgress(requiredAmount);
                }
            }
            if (quest.CanGetReward() && quest.questDataSo.rewardType == npcType)
                getRewardQuest.Add(quest);
        }

        foreach (var quest in getRewardQuest)
        {
            GiveQuestReward(quest.questDataSo);
            CompleteQuest(quest);
        }
    }

    private void GiveQuestReward(QuestDataSO questDataSo)
    {
        foreach (var item in questDataSo.rewardItems)
        {
            if (item == null || item.itemData == null) continue;

            for (int i = 0; i < item.stackSize; i++)
            {
                dropManager.CreateItemDrop(item.itemData);
            }
        }
    }

    public bool HasComplatedQuest()
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            // Delivery quest check

            QuestData quest = activeQuests[i];

            if (quest.questDataSo.questType == QuestType.Delivery)
            {
                var requiredItem = quest.questDataSo.itemToDelivery;
                var requiredAmount = quest.questDataSo.requiredAmount;

                if (inventory.HasItemAmount(requiredItem, requiredAmount))
                    return true;

            }

            if (quest.CanGetReward())
                return true;
        }
        return false;
    }

    public void AddProgress(string questTargetID, int amount = 1)
    {
        List<QuestData> getRewardQuests = new List<QuestData>();

        foreach (var quest in activeQuests)
        {
            if (quest.questDataSo.questTargetID != questTargetID)
                continue;

            if (quest.CanGetReward() == false)
                quest.AddQuestProgress(amount);

            if (quest.questDataSo.rewardType == RewardType.None && quest.CanGetReward())
                getRewardQuests.Add(quest);
        }
        foreach (var quest in getRewardQuests)
        {
            GiveQuestReward(quest.questDataSo);
            CompleteQuest(quest);
        }
    }

    public void ActiveQuest(QuestDataSO questDataSo)
    {
        activeQuests.Add(new QuestData(questDataSo));
    }

    public bool QuestIsActive(QuestDataSO questToCheck)
    {
        if (questToCheck == null)
            return false;

        return activeQuests.Find(q => q.questDataSo == questToCheck) != null;
    }

    public int GetQuestProgress(QuestData questToCheck)
    {
        QuestData quest = activeQuests.Find(q => q == questToCheck);
        return quest != null ? quest.currentAmount : 0;
    }

    public void CompleteQuest(QuestData questData)
    {
        complateQuests.Add(questData);
        activeQuests.Remove(questData);
    }

    public void LoadData(GameData gameData)
    {
        activeQuests.Clear();
        complateQuests.Clear();

        foreach (var entry in gameData.activeQuests)
        {
            string questSaveID = entry.Key;
            int progress = entry.Value;

            QuestDataSO questDataSo = questDatabase.GetQuestById(questSaveID);

            if (questDataSo == null)
            {
                Debug.LogWarning($"Quest with ID {questSaveID} not found in the database.");
                continue;
            }

            QuestData questToLoad = new QuestData(questDataSo);
            questToLoad.currentAmount = progress;

            activeQuests.Add(questToLoad);
        }

        foreach (var entry in gameData.completedQuests)
        {
            string questSaveID = entry.Key;

            QuestDataSO questDataSo = questDatabase.GetQuestById(questSaveID);

            if (questDataSo == null)
            {
                Debug.LogWarning($"Completed quest with ID {questSaveID} not found in the database.");
                continue;
            }

            QuestData questToLoad = new QuestData(questDataSo);
            questToLoad.currentAmount = questDataSo.requiredAmount;
            questToLoad.canGetReward = true;

            complateQuests.Add(questToLoad);
        }
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.activeQuests.Clear();
        gameData.completedQuests.Clear();

        foreach (var quest in activeQuests)
        {
            if (quest == null || quest.questDataSo == null)
                continue;

            gameData.activeQuests[quest.questDataSo.questSaveID] = quest.currentAmount;
        }

        foreach (var quest in complateQuests)
        {
            if (quest == null || quest.questDataSo == null)
                continue;

            gameData.completedQuests[quest.questDataSo.questSaveID] = true;
        }

        Debug.Log($"Saved {activeQuests.Count} active quests and {complateQuests.Count} completed quests");
    }

}