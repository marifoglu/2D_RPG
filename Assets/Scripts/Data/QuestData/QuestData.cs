using System;
using UnityEngine;

[Serializable]
public class QuestData
{
    public QuestDataSO questDataSo;
    public int currentAmount;
    public bool canGetReward;

    public void AddQuestProgress(int amount = 1)
    {
        currentAmount += amount;
        if (currentAmount >= questDataSo.requiredAmount)
        {
            currentAmount = questDataSo.requiredAmount;
            canGetReward = CanGetReward();
        }
    }

    public bool CanGetReward() => currentAmount >= questDataSo.requiredAmount;

    public QuestData(QuestDataSO questDataSo)
    {
        this.questDataSo = questDataSo;
        currentAmount = 0;
        canGetReward = false;
    }
}
