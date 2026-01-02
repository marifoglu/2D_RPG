//using System.Linq;
//using UnityEditor;
//using UnityEngine;

//[CreateAssetMenu(menuName = "RPG Setup/Quest Data/Quest Database", fileName = "Quest Database ")]
//public class QuestDatabaseSO : ScriptableObject
//{
//    public QuestDataSO[] allQuests;


//    public QuestDataSO GetQuestById(string id)
//    {
//        return allQuests.FirstOrDefault(q => q != null && q.questSaveID == id);
//    }

//#if UNITY_EDITOR
//    [ContextMenu("Auto-fill with all QuestDataSO")]
//    public void CollectItemsData()
//    {
//        string[] guids = AssetDatabase.FindAssets("t:QuestDataSO");

//        allQuests = guids
//            .Select(guid => AssetDatabase.LoadAssetAtPath<QuestDataSO>(AssetDatabase.GUIDToAssetPath(guid)))
//            .Where(q => q != null)
//            .ToArray();

//        EditorUtility.SetDirty(this);
//        AssetDatabase.SaveAssets();
//    }
//#endif
//}



using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Quest Data/Quest Database", fileName = "Quest Database")]
public class QuestDatabaseSO : ScriptableObject
{
    public QuestDataSO[] allQuests;

    /// <summary>
    /// Get quest by save ID
    /// </summary>
    public QuestDataSO GetQuestById(string id)
    {
        return allQuests.FirstOrDefault(q => q != null && q.questSaveID == id);
    }

    /// <summary>
    /// Get quest by name
    /// </summary>
    public QuestDataSO GetQuestByName(string questName)
    {
        return allQuests.FirstOrDefault(q => q != null && q.questName == questName);
    }

    /// <summary>
    /// Get all quests of a specific type
    /// </summary>
    public List<QuestDataSO> GetQuestsByType(QuestType type)
    {
        return allQuests.Where(q => q != null && q.questType == type).ToList();
    }

    /// <summary>
    /// Get all main quests
    /// </summary>
    public List<QuestDataSO> GetMainQuests()
    {
        return GetQuestsByType(QuestType.Main);
    }

    /// <summary>
    /// Get all side quests
    /// </summary>
    public List<QuestDataSO> GetSideQuests()
    {
        return GetQuestsByType(QuestType.Side);
    }

    /// <summary>
    /// Get quests that have a specific NPC as reward giver
    /// </summary>
    public List<QuestDataSO> GetQuestsForNpc(string npcID)
    {
        return allQuests.Where(q => q != null && q.CanNpcGiveReward(npcID)).ToList();
    }

    /// <summary>
    /// Get quests by reward type
    /// </summary>
    public List<QuestDataSO> GetQuestsByRewardType(RewardType rewardType)
    {
        return allQuests.Where(q => q != null && q.rewardType == rewardType).ToList();
    }

    /// <summary>
    /// Get quests that have no prerequisite (starter quests)
    /// </summary>
    public List<QuestDataSO> GetStarterQuests()
    {
        return allQuests.Where(q => q != null && q.prerequisiteQuest == null).ToList();
    }

#if UNITY_EDITOR
    [ContextMenu("Auto-fill with all QuestDataSO")]
    public void CollectItemsData()
    {
        string[] guids = AssetDatabase.FindAssets("t:QuestDataSO");

        allQuests = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<QuestDataSO>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(q => q != null)
            .ToArray();

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    [ContextMenu("Validate All Quests")]
    public void ValidateAllQuests()
    {
        int issues = 0;

        foreach (var quest in allQuests)
        {
            if (quest == null) continue;

            // Check for missing objectives in complex quests
            if (quest.isComplexQuest && (quest.objectives == null || quest.objectives.Length == 0))
            {
                Debug.LogWarning($"Quest '{quest.questName}' is marked as complex but has no objectives!");
                issues++;
            }

            // Check for null objectives
            if (quest.objectives != null)
            {
                for (int i = 0; i < quest.objectives.Length; i++)
                {
                    if (quest.objectives[i] == null)
                    {
                        Debug.LogWarning($"Quest '{quest.questName}' has null objective at index {i}!");
                        issues++;
                    }
                }
            }

            // Check for circular prerequisites
            if (quest.prerequisiteQuest == quest)
            {
                Debug.LogError($"Quest '{quest.questName}' has itself as prerequisite!");
                issues++;
            }

            // Check for circular quest chains
            if (quest.nextQuestInChain == quest)
            {
                Debug.LogError($"Quest '{quest.questName}' has itself as next quest in chain!");
                issues++;
            }
        }

        Debug.Log($"Quest validation complete. Found {issues} issues.");
    }
#endif
}