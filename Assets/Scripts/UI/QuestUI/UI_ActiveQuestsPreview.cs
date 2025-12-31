using TMPro;
using UnityEngine;

public class UI_ActiveQuestsPreview : MonoBehaviour
{
    private Player_QuestManager questManager;

    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI progress;
    [SerializeField] private UI_QuestRewardSlot[] questRewardSlots;

    public void SetupQuestPreview(QuestData questData)
    {
        questManager = Player.instance.questManager;
        QuestDataSO questSO = questData.questDataSo;

        questName.text = questSO.questName;
        description.text = questSO.questDescription;

        progress.text = questSO.questGoal + " " + questManager.GetQuestProgress(questData) + "/" + questSO.requiredAmount;

        foreach(var obj in questRewardSlots)
            obj.gameObject.SetActive(false);

        for(int i = 0; i < questSO.rewardItems.Length; i++)
        {
            questRewardSlots[i].gameObject.SetActive(true);
            questRewardSlots[i].UpdateSlot(questSO.rewardItems[i]);
        }
    }

}
