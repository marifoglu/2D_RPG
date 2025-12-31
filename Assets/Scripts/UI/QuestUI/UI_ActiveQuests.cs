using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UI_ActiveQuests : MonoBehaviour
{
    private Player_QuestManager questManager;
    private UI_ActiveQuestsSlots[] questsSlots;

    private void Awake()
    {
        questManager = Player.instance.questManager;
        questsSlots = GetComponentsInChildren<UI_ActiveQuestsSlots>(true);
    }

    private void OnEnable()
    {
        List<QuestData> quests = questManager.activeQuests;

        foreach (var slot in questsSlots)
            slot.gameObject.SetActive(false);

        for (int i = 0; i < quests.Count; i++)
        {
            questsSlots[i].gameObject.SetActive(true);
            questsSlots[i].SetActiveQuestSlot(quests[i]);
        }

        if(quests.Count > 0)
            questsSlots[0].SetupPreview();
    }
}
