using Unity.VisualScripting;
using UnityEngine;

public class UI_Quest : MonoBehaviour, ISaveable
{
    private GameData currentGameData;

    private UI_QuestSlot[] questSlot;
    public Player_QuestManager questManager;

    [SerializeField] private UI_ItemSlotParent inventorySlots;
    [SerializeField] private UI_QuestPreview questPreview;

    private void Awake()
    {
        questSlot = GetComponentsInChildren<UI_QuestSlot>(true);
        questManager = Player.instance.questManager;
    }
    public void SetupQuestUI(QuestDataSO[] questToSetup)
    {
        foreach (var slot in questSlot)
        {
            slot.gameObject.SetActive(false);
        }

        for (int i = 0; i < questToSetup.Length; i++)
        {
            questSlot[i].gameObject.SetActive(true);
            questSlot[i].SetupQuestSlot(questToSetup[i]);
        }

        inventorySlots.UpdateSlots(Player.instance.inventory.itemList);
        questPreview.MakeQuestPreviewEmpty();

        UpdateQuestList();
    }

    public void UpdateQuestList()
    {
        foreach (var slot in questSlot)
        {
            if (slot.questInSlot == null) continue;

            if (slot.gameObject.activeSelf && CanTakeQuest(slot.questInSlot) == false)
                slot.gameObject.SetActive(false);
        }
    }
    private bool CanTakeQuest(QuestDataSO questToCheck)
    {
        bool questActive = questManager.QuestIsActive(questToCheck);
        if(currentGameData != null)
        {
            bool questCompleted = currentGameData.completedQuests.TryGetValue(questToCheck.questSaveID, out bool isCompleted) && isCompleted;

            return questActive == false && questCompleted == false;
        }
        return questActive == false;
    }
    public UI_QuestPreview GetQuestPreview() => questPreview;

    public void LoadData(GameData gameData)
    {
        currentGameData = gameData;
    }

    public void SaveData(ref GameData gameData)
    {
    }
}
