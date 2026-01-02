//using UnityEditor;
//using UnityEngine;

//public enum RewardType { Merchant, Blacksmith, None };
//public enum QuestType { Main, Side, Kill, Talk, Delivery };

//[CreateAssetMenu(menuName = "RPG Setup/Quest Data/New Quest", fileName = "Quest - ")]
//public class QuestDataSO : ScriptableObject
//{
//    public string questSaveID;
//    [Space]
//    public QuestType questType;
//    public string questName;
//    [TextArea] public string questDescription;
//    [TextArea] public string questGoal;

//    public string questTargetID; // Enemy name, NPC name or Itemname etc..
//    public int requiredAmount;
//    public ItemDataSO itemToDelivery; // only for delivery quests

//    [Header("Reward")]
//    public RewardType rewardType;
//    public Inventory_Item[] rewardItems;

//    private void OnValidate()
//    {
//#if UNITY_EDITOR
//        string path = AssetDatabase.GetAssetPath(this);
//        questSaveID = AssetDatabase.AssetPathToGUID(path);
//#endif

//    }
//}


using UnityEditor;
using UnityEngine;

public enum RewardType { Merchant, Blacksmith, None }
public enum QuestCategory { Main, Side }
public enum QuestCompletionMode { AllObjectives, AnyObjective, Sequential }

// Keep old enum for backward compatibility but mark as obsolete
public enum QuestType { Main, Side, Kill, Talk, Delivery }

[CreateAssetMenu(menuName = "RPG Setup/Quest Data/New Quest", fileName = "Quest - ")]
public class QuestDataSO : ScriptableObject
{
    [Header("Quest Identity")]
    public string questSaveID;
    public QuestCategory questCategory = QuestCategory.Side;
    public string questName;
    [TextArea] public string questDescription;
    [TextArea] public string questGoal;

    [Header("Quest Complexity")]
    [Tooltip("Simple = single objective, Complex = multiple objectives")]
    public bool isComplexQuest = false;
    public QuestCompletionMode completionMode = QuestCompletionMode.AllObjectives;

    [Header("Complex Quest - Multiple Objectives")]
    [Tooltip("List of objectives for complex quests. Only used when isComplexQuest = true")]
    public QuestObjectiveSO[] objectives;

    // ==================== SIMPLE QUEST SETTINGS ====================
    [Header("Simple Quest - Objective Type")]
    [Tooltip("What does the player need to do?")]
    public QuestObjectiveType simpleObjectiveType = QuestObjectiveType.Kill;

    [Header("Simple Quest - Kill Settings")]
    [Tooltip("Enemy ID to kill (must match enemy's targetID or name)")]
    public string killTargetEnemyID;
    [Tooltip("How many to kill")]
    public int killAmount = 1;

    [Header("Simple Quest - Talk Settings")]
    [Tooltip("NPC ID to talk to (must match NPC's npcID)")]
    public string talkTargetNpcID;

    [Header("Simple Quest - Collect Settings")]
    [Tooltip("Item to collect")]
    public ItemDataSO collectItem;
    [Tooltip("How many to collect")]
    public int collectAmount = 1;

    [Header("Simple Quest - Deliver Settings")]
    [Tooltip("Item to deliver")]
    public ItemDataSO deliverItem;
    [Tooltip("How many to deliver")]
    public int deliverAmount = 1;
    [Tooltip("NPC ID to deliver to")]
    public string deliverTargetNpcID;

    [Header("Simple Quest - Visit Settings")]
    [Tooltip("Location/Trigger ID to visit")]
    public string visitLocationID;

    // ==================== BACKWARD COMPATIBILITY ====================
    [HideInInspector] public QuestType questType; // Old field, kept for compatibility
    [HideInInspector] public string questTargetID; // Old field
    [HideInInspector] public int requiredAmount = 1; // Old field
    [HideInInspector] public ItemDataSO itemToDelivery; // Old field

    [Header("Reward Settings")]
    public RewardType rewardType;
    [Tooltip("NPC ID that gives the reward. If empty, uses rewardType matching.")]
    public string rewardGiverNpcID;
    [Tooltip("Additional NPCs that can give rewards")]
    public string[] additionalRewardGiverNpcIDs;
    public Inventory_Item[] rewardItems;

    [Header("Quest Chain (Optional)")]
    [Tooltip("Quest that must be completed before this one can be accepted")]
    public QuestDataSO prerequisiteQuest;
    [Tooltip("Quest that becomes available after completing this one")]
    public QuestDataSO nextQuestInChain;

    [Header("UI Settings")]
    public Sprite questIcon;
    public bool showInTracker = true;

    private void OnValidate()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        questSaveID = AssetDatabase.AssetPathToGUID(path);

        // Sync old fields for backward compatibility
        SyncBackwardCompatibleFields();
#endif
    }

    /// <summary>
    /// Sync new fields to old fields for backward compatibility
    /// </summary>
    private void SyncBackwardCompatibleFields()
    {
        if (isComplexQuest) return;

        switch (simpleObjectiveType)
        {
            case QuestObjectiveType.Kill:
                questTargetID = killTargetEnemyID;
                requiredAmount = killAmount;
                questType = QuestType.Kill;
                break;
            case QuestObjectiveType.Talk:
                questTargetID = talkTargetNpcID;
                requiredAmount = 1;
                questType = QuestType.Talk;
                break;
            case QuestObjectiveType.Collect:
                questTargetID = collectItem != null ? collectItem.saveID : "";
                requiredAmount = collectAmount;
                break;
            case QuestObjectiveType.Deliver:
                questTargetID = deliverTargetNpcID;
                requiredAmount = deliverAmount;
                itemToDelivery = deliverItem;
                questType = QuestType.Delivery;
                break;
            case QuestObjectiveType.Visit:
                questTargetID = visitLocationID;
                requiredAmount = 1;
                break;
        }
    }

    /// <summary>
    /// Get the target ID for simple quests based on objective type
    /// </summary>
    public string GetSimpleQuestTargetID()
    {
        if (isComplexQuest) return null;

        return simpleObjectiveType switch
        {
            QuestObjectiveType.Kill => killTargetEnemyID,
            QuestObjectiveType.Talk => talkTargetNpcID,
            QuestObjectiveType.Collect => collectItem?.saveID ?? "",
            QuestObjectiveType.Deliver => deliverTargetNpcID,
            QuestObjectiveType.Visit => visitLocationID,
            _ => questTargetID // Fallback to old field
        };
    }

    /// <summary>
    /// Get the required amount for simple quests
    /// </summary>
    public int GetSimpleQuestRequiredAmount()
    {
        if (isComplexQuest) return 0;

        return simpleObjectiveType switch
        {
            QuestObjectiveType.Kill => killAmount,
            QuestObjectiveType.Talk => 1,
            QuestObjectiveType.Collect => collectAmount,
            QuestObjectiveType.Deliver => deliverAmount,
            QuestObjectiveType.Visit => 1,
            _ => requiredAmount // Fallback
        };
    }

    /// <summary>
    /// Get delivery item for simple quests
    /// </summary>
    public ItemDataSO GetDeliveryItem()
    {
        if (simpleObjectiveType == QuestObjectiveType.Deliver)
            return deliverItem;
        return itemToDelivery; // Fallback
    }

    /// <summary>
    /// Check if this is a delivery type quest
    /// </summary>
    public bool IsDeliveryQuest()
    {
        if (isComplexQuest)
        {
            foreach (var obj in objectives)
            {
                if (obj != null && obj.objectiveType == QuestObjectiveType.Deliver)
                    return true;
            }
            return false;
        }

        return simpleObjectiveType == QuestObjectiveType.Deliver || questType == QuestType.Delivery;
    }

    /// <summary>
    /// Check if this quest uses the complex multi-objective system
    /// </summary>
    public bool HasMultipleObjectives()
    {
        return isComplexQuest && objectives != null && objectives.Length > 0;
    }

    /// <summary>
    /// Get the total number of objectives (1 for simple quests)
    /// </summary>
    public int GetObjectiveCount()
    {
        if (HasMultipleObjectives())
            return objectives.Length;
        return 1;
    }

    /// <summary>
    /// Check if a specific NPC can give rewards for this quest
    /// </summary>
    public bool CanNpcGiveReward(string npcID)
    {
        if (string.IsNullOrEmpty(npcID)) return false;

        // Check primary reward giver
        if (rewardGiverNpcID == npcID) return true;

        // Check additional reward givers
        if (additionalRewardGiverNpcIDs != null)
        {
            foreach (var id in additionalRewardGiverNpcIDs)
            {
                if (id == npcID) return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Get objective by index (returns null for simple quests if index > 0)
    /// </summary>
    public QuestObjectiveSO GetObjective(int index)
    {
        if (!HasMultipleObjectives())
            return null;

        if (index < 0 || index >= objectives.Length)
            return null;

        return objectives[index];
    }

    /// <summary>
    /// Find objective by target ID
    /// </summary>
    public QuestObjectiveSO FindObjectiveByTarget(string targetID)
    {
        if (!HasMultipleObjectives())
            return null;

        foreach (var obj in objectives)
        {
            if (obj != null && obj.targetID == targetID)
                return obj;
        }
        return null;
    }
}