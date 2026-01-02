using System;
using UnityEngine;

[Serializable]
public class DialogueNPCData
{
    public RewardType npcRewardType;
    public QuestDataSO[] quests;

    public DialogueNPCData(RewardType npcRewardType, QuestDataSO[] quests)
    {
        this.npcRewardType = npcRewardType;
        this.quests = quests;
    }
}
