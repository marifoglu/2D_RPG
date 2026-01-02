//using System;
//using UnityEngine;

//[Serializable]
//public class DialogueNPCData
//{
//    public RewardType npcRewardType;
//    public QuestDataSO[] quests;

//    public DialogueNPCData(RewardType npcRewardType, QuestDataSO[] quests)
//    {
//        this.npcRewardType = npcRewardType;
//        this.quests = quests;
//    }
//}



using System;
using UnityEngine;

[Serializable]
public class DialogueNPCData
{
    [Header("NPC Identity")]
    public string npcID;
    public string npcName;

    [Header("Reward Settings")]
    public RewardType npcRewardType;

    [Header("Available Quests")]
    [Tooltip("Quests this NPC can give to the player")]
    public QuestDataSO[] availableQuests;

    [Header("Quest Turn-In")]
    [Tooltip("If true, this NPC can receive quest turn-ins based on quest settings")]
    public bool canReceiveQuestTurnIns = true;

    // Backward compatible constructor
    public DialogueNPCData(RewardType npcRewardType, QuestDataSO[] quests)
    {
        this.npcRewardType = npcRewardType;
        this.availableQuests = quests;
        this.canReceiveQuestTurnIns = true;
    }

    // New full constructor
    public DialogueNPCData(string npcID, string npcName, RewardType rewardType, QuestDataSO[] quests, bool canReceiveTurnIns = true)
    {
        this.npcID = npcID;
        this.npcName = npcName;
        this.npcRewardType = rewardType;
        this.availableQuests = quests;
        this.canReceiveQuestTurnIns = canReceiveTurnIns;
    }

    /// <summary>
    /// Get quests that the player can accept from this NPC
    /// </summary>
    public QuestDataSO[] GetAcceptableQuests(Player_QuestManager questManager)
    {
        if (availableQuests == null || availableQuests.Length == 0)
            return new QuestDataSO[0];

        var acceptable = new System.Collections.Generic.List<QuestDataSO>();

        foreach (var quest in availableQuests)
        {
            if (quest == null) continue;

            // Skip if already active or completed
            if (questManager.QuestIsActive(quest) || questManager.QuestIsCompleted(quest))
                continue;

            // Skip if prerequisite not met
            if (quest.prerequisiteQuest != null && !questManager.QuestIsCompleted(quest.prerequisiteQuest))
                continue;

            acceptable.Add(quest);
        }

        return acceptable.ToArray();
    }

    /// <summary>
    /// Check if this NPC has any quests available for the player
    /// </summary>
    public bool HasAvailableQuests(Player_QuestManager questManager)
    {
        return GetAcceptableQuests(questManager).Length > 0;
    }

    /// <summary>
    /// Check if player can turn in any quests to this NPC
    /// </summary>
    public bool HasTurnInableQuests(Player_QuestManager questManager)
    {
        if (!canReceiveQuestTurnIns) return false;

        // Check by NPC ID
        if (!string.IsNullOrEmpty(npcID) && questManager.HasCompletedQuestForNpc(npcID))
            return true;

        // Check by reward type
        if (questManager.HasCompletedQuestFor(npcRewardType))
            return true;

        return false;
    }
}