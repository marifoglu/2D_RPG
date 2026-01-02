//using System;
//using UnityEngine;

//[Serializable]
//public class QuestData
//{
//    public QuestDataSO questDataSo;
//    public int currentAmount;
//    public bool canGetReward;

//    public void AddQuestProgress(int amount = 1)
//    {
//        currentAmount += amount;
//        if (currentAmount >= questDataSo.requiredAmount)
//        {
//            currentAmount = questDataSo.requiredAmount;
//            canGetReward = CanGetReward();
//        }
//    }

//    public bool CanGetReward() => currentAmount >= questDataSo.requiredAmount;

//    public QuestData(QuestDataSO questDataSo)
//    {
//        this.questDataSo = questDataSo;
//        currentAmount = 0;
//        canGetReward = false;
//    }
//}


using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestData
{
    public QuestDataSO questDataSo;

    // Simple quest tracking (backward compatible)
    public int currentAmount;
    public bool canGetReward;

    // Complex quest tracking
    public List<QuestObjectiveData> objectiveProgress;
    private bool isInitialized;

    public QuestData(QuestDataSO questDataSo)
    {
        this.questDataSo = questDataSo;
        currentAmount = 0;
        canGetReward = false;

        InitializeObjectives();
    }

    private void InitializeObjectives()
    {
        objectiveProgress = new List<QuestObjectiveData>();

        if (questDataSo.HasMultipleObjectives())
        {
            foreach (var objective in questDataSo.objectives)
            {
                if (objective != null)
                {
                    objectiveProgress.Add(new QuestObjectiveData(objective));
                }
            }
        }

        isInitialized = true;
    }

    #region Simple Quest Methods (Backward Compatible)

    /// <summary>
    /// Add progress for simple single-objective quests
    /// </summary>
    public void AddQuestProgress(int amount = 1)
    {
        // For complex quests, use AddObjectiveProgress instead
        if (questDataSo.HasMultipleObjectives())
        {
            Debug.LogWarning($"Quest '{questDataSo.questName}' is complex. Use AddObjectiveProgress() instead.");
            return;
        }

        int required = questDataSo.GetSimpleQuestRequiredAmount();
        currentAmount += amount;
        if (currentAmount >= required)
        {
            currentAmount = required;
            canGetReward = CanGetReward();
        }
    }

    /// <summary>
    /// Check if simple quest can get reward (backward compatible)
    /// </summary>
    public bool CanGetReward()
    {
        if (questDataSo.HasMultipleObjectives())
        {
            return AreAllObjectivesComplete();
        }

        return currentAmount >= questDataSo.GetSimpleQuestRequiredAmount();
    }

    /// <summary>
    /// Get target ID for simple quest
    /// </summary>
    public string GetTargetID()
    {
        return questDataSo.GetSimpleQuestTargetID();
    }

    /// <summary>
    /// Get required amount for simple quest
    /// </summary>
    public int GetRequiredAmount()
    {
        return questDataSo.GetSimpleQuestRequiredAmount();
    }

    #endregion

    #region Complex Quest Methods

    /// <summary>
    /// Add progress to a specific objective by target ID
    /// </summary>
    public bool AddObjectiveProgress(string targetID, int amount = 1)
    {
        if (!questDataSo.HasMultipleObjectives())
        {
            // Fall back to simple quest progress
            if (questDataSo.questTargetID == targetID)
            {
                AddQuestProgress(amount);
                return true;
            }
            return false;
        }

        var objectiveData = FindObjectiveData(targetID);
        if (objectiveData == null) return false;

        // Check if sequential and previous objectives are complete
        if (objectiveData.objectiveSO.isSequential && !CanProgressSequentialObjective(objectiveData))
        {
            return false;
        }

        objectiveData.AddProgress(amount);
        UpdateCanGetReward();
        return true;
    }

    /// <summary>
    /// Add progress to a specific objective by type and target
    /// </summary>
    public bool AddObjectiveProgress(QuestObjectiveType type, string targetID, int amount = 1)
    {
        if (!questDataSo.HasMultipleObjectives())
        {
            // Fall back to simple quest progress for matching target
            if (questDataSo.questTargetID == targetID)
            {
                AddQuestProgress(amount);
                return true;
            }
            return false;
        }

        foreach (var objData in objectiveProgress)
        {
            if (objData.objectiveSO.objectiveType == type &&
                objData.objectiveSO.targetID == targetID)
            {
                if (objData.objectiveSO.isSequential && !CanProgressSequentialObjective(objData))
                    continue;

                objData.AddProgress(amount);
                UpdateCanGetReward();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Complete an objective (for Talk/Visit types that are binary)
    /// </summary>
    public bool CompleteObjective(string targetID)
    {
        return AddObjectiveProgress(targetID, 999); // Large number to ensure completion
    }

    /// <summary>
    /// Find objective data by target ID
    /// </summary>
    public QuestObjectiveData FindObjectiveData(string targetID)
    {
        foreach (var objData in objectiveProgress)
        {
            if (objData.objectiveSO.targetID == targetID)
                return objData;
        }
        return null;
    }

    /// <summary>
    /// Find objective data by objective ID
    /// </summary>
    public QuestObjectiveData FindObjectiveDataByID(string objectiveID)
    {
        foreach (var objData in objectiveProgress)
        {
            if (objData.objectiveSO.objectiveID == objectiveID)
                return objData;
        }
        return null;
    }

    /// <summary>
    /// Check if a sequential objective can be progressed
    /// </summary>
    private bool CanProgressSequentialObjective(QuestObjectiveData targetObjective)
    {
        int targetIndex = objectiveProgress.IndexOf(targetObjective);
        if (targetIndex <= 0) return true;

        // Check all previous objectives are complete
        for (int i = 0; i < targetIndex; i++)
        {
            if (!objectiveProgress[i].isCompleted)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Check if all objectives are complete
    /// </summary>
    public bool AreAllObjectivesComplete()
    {
        if (!questDataSo.HasMultipleObjectives())
        {
            return currentAmount >= questDataSo.requiredAmount;
        }

        foreach (var objData in objectiveProgress)
        {
            if (!objData.isCompleted)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Check if any objective is complete (for AnyObjective completion mode)
    /// </summary>
    public bool IsAnyObjectiveComplete()
    {
        if (!questDataSo.HasMultipleObjectives())
        {
            return currentAmount >= questDataSo.requiredAmount;
        }

        foreach (var objData in objectiveProgress)
        {
            if (objData.isCompleted)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Update canGetReward based on completion mode
    /// </summary>
    private void UpdateCanGetReward()
    {
        switch (questDataSo.completionMode)
        {
            case QuestCompletionMode.AllObjectives:
                canGetReward = AreAllObjectivesComplete();
                break;
            case QuestCompletionMode.AnyObjective:
                canGetReward = IsAnyObjectiveComplete();
                break;
            case QuestCompletionMode.Sequential:
                canGetReward = AreAllObjectivesComplete();
                break;
        }
    }

    /// <summary>
    /// Get current active objective (for sequential quests)
    /// </summary>
    public QuestObjectiveData GetCurrentActiveObjective()
    {
        if (!questDataSo.HasMultipleObjectives())
            return null;

        foreach (var objData in objectiveProgress)
        {
            if (!objData.isCompleted)
                return objData;
        }
        return null;
    }

    /// <summary>
    /// Get all incomplete objectives
    /// </summary>
    public List<QuestObjectiveData> GetIncompleteObjectives()
    {
        var incomplete = new List<QuestObjectiveData>();

        foreach (var objData in objectiveProgress)
        {
            if (!objData.isCompleted)
                incomplete.Add(objData);
        }

        return incomplete;
    }

    /// <summary>
    /// Get completion percentage of the entire quest
    /// </summary>
    public float GetOverallProgress()
    {
        if (!questDataSo.HasMultipleObjectives())
        {
            if (questDataSo.requiredAmount <= 0) return canGetReward ? 1f : 0f;
            return (float)currentAmount / questDataSo.requiredAmount;
        }

        if (objectiveProgress.Count == 0) return 0f;

        float totalProgress = 0f;
        foreach (var objData in objectiveProgress)
        {
            totalProgress += objData.GetProgressPercentage();
        }

        return totalProgress / objectiveProgress.Count;
    }

    #endregion

    #region Turn-In Methods

    /// <summary>
    /// Check if quest can be turned in to specific NPC
    /// </summary>
    public bool CanTurnInTo(string npcID)
    {
        if (!canGetReward) return false;

        // Check by reward giver NPC ID
        if (!string.IsNullOrEmpty(questDataSo.rewardGiverNpcID) && questDataSo.rewardGiverNpcID == npcID)
            return true;

        // Check additional reward givers
        if (questDataSo.CanNpcGiveReward(npcID))
            return true;

        // Check objectives that have turn-in NPC (for complex quests)
        if (questDataSo.HasMultipleObjectives())
        {
            foreach (var objData in objectiveProgress)
            {
                if (objData.isCompleted &&
                    !objData.isTurnedIn &&
                    objData.objectiveSO.turnInNpcID == npcID)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Mark objective as turned in
    /// </summary>
    public void MarkObjectiveTurnedIn(string targetID)
    {
        var objData = FindObjectiveData(targetID);
        if (objData != null)
        {
            objData.isTurnedIn = true;
        }
    }

    #endregion

    #region Serialization Support

    /// <summary>
    /// Get serializable progress data
    /// </summary>
    public Dictionary<string, int> GetSerializableProgress()
    {
        var data = new Dictionary<string, int>();

        if (!questDataSo.HasMultipleObjectives())
        {
            data["_simple_progress"] = currentAmount;
        }
        else
        {
            foreach (var objData in objectiveProgress)
            {
                data[objData.objectiveSO.objectiveID] = objData.currentAmount;
            }
        }

        return data;
    }

    /// <summary>
    /// Load progress from serialized data
    /// </summary>
    public void LoadSerializableProgress(Dictionary<string, int> data)
    {
        if (data == null) return;

        if (!questDataSo.HasMultipleObjectives())
        {
            if (data.TryGetValue("_simple_progress", out int progress))
            {
                currentAmount = progress;
                canGetReward = CanGetReward();
            }
        }
        else
        {
            foreach (var objData in objectiveProgress)
            {
                if (data.TryGetValue(objData.objectiveSO.objectiveID, out int progress))
                {
                    objData.SetProgress(progress);
                }
            }
            UpdateCanGetReward();
        }
    }

    #endregion
}