using System;
using UnityEngine;

[Serializable]
public class QuestObjectiveData
{
    public QuestObjectiveSO objectiveSO;
    public int currentAmount;
    public bool isCompleted;
    public bool isTurnedIn;

    public QuestObjectiveData(QuestObjectiveSO objective)
    {
        objectiveSO = objective;
        currentAmount = 0;
        isCompleted = false;
        isTurnedIn = false;
    }

    public void AddProgress(int amount = 1)
    {
        if (isCompleted) return;

        currentAmount += amount;
        if (currentAmount >= objectiveSO.requiredAmount)
        {
            currentAmount = objectiveSO.requiredAmount;
            isCompleted = true;
        }
    }

    public void SetProgress(int amount)
    {
        currentAmount = Mathf.Clamp(amount, 0, objectiveSO.requiredAmount);
        isCompleted = currentAmount >= objectiveSO.requiredAmount;
    }

    public bool CanProgress()
    {
        return !isCompleted;
    }

    public float GetProgressPercentage()
    {
        if (objectiveSO.requiredAmount <= 0) return isCompleted ? 1f : 0f;
        return (float)currentAmount / objectiveSO.requiredAmount;
    }

    public string GetProgressText()
    {
        return objectiveSO.GetProgressText(currentAmount);
    }
}