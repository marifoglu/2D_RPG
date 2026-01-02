using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Quest Data/New Objective", fileName = "Objective - ")]
public class QuestObjectiveSO : ScriptableObject
{
    [Header("Objective Identity")]
    public string objectiveID;
    public string objectiveDescription;

    [Header("Objective Type")]
    public QuestObjectiveType objectiveType;

    [Header("Target Settings")]
    [Tooltip("Enemy name, NPC name, Item ID, Location ID, etc.")]
    public string targetID;
    public int requiredAmount = 1;

    [Header("Item Settings (for Collect/Deliver)")]
    public ItemDataSO requiredItem;

    [Header("Optional Settings")]
    [Tooltip("If true, this objective must be completed before the next one can progress")]
    public bool isSequential = false;

    [Tooltip("Optional: Specific NPC that can acknowledge this objective completion")]
    public string turnInNpcID;

    [Header("UI Settings")]
    public Sprite objectiveIcon;
    [Tooltip("Short text shown in quest tracker")]
    public string trackerText;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(objectiveID))
        {
            objectiveID = System.Guid.NewGuid().ToString();
        }

        // Auto-generate tracker text if empty
        if (string.IsNullOrEmpty(trackerText))
        {
            trackerText = GenerateTrackerText();
        }
#endif
    }

    private string GenerateTrackerText()
    {
        return objectiveType switch
        {
            QuestObjectiveType.Kill => $"Kill {targetID}",
            QuestObjectiveType.Talk => $"Talk to {targetID}",
            QuestObjectiveType.Collect => $"Collect {requiredItem?.itemName ?? targetID}",
            QuestObjectiveType.Deliver => $"Deliver {requiredItem?.itemName ?? "item"} to {targetID}",
            QuestObjectiveType.Visit => $"Visit {targetID}",
            QuestObjectiveType.Interact => $"Interact with {targetID}",
            _ => objectiveDescription
        };
    }

    public string GetProgressText(int currentAmount)
    {
        if (requiredAmount <= 1)
        {
            return trackerText;
        }
        return $"{trackerText} ({currentAmount}/{requiredAmount})";
    }
}