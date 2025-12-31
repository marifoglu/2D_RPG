using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private Image[] rewarQuickPreviewSlot;

    public QuestDataSO questInSlot { get; private set; }
    private UI_QuestPreview questPreview;

    public void SetupQuestSlot(QuestDataSO questDataSO)
    {
        questPreview = transform.root.GetComponentInChildren<UI_Quest>().GetQuestPreview();
        questInSlot = questDataSO;
        questName.text = questDataSO.questName;

        foreach (var prevIcon in rewarQuickPreviewSlot)
        {
            prevIcon.gameObject.SetActive(false);
        }

        for(int i = 0; i < questInSlot.rewardItems.Length; i++)
        {
            if (questInSlot.rewardItems[i] == null || questInSlot.rewardItems[i].itemData == null) continue;
            
            Image slot = rewarQuickPreviewSlot[i];

            slot.gameObject.SetActive(true);
            slot.sprite = questDataSO.rewardItems[i].itemData.itemIcon;
            slot.GetComponentInChildren<TextMeshProUGUI>().text = questDataSO.rewardItems[i].stackSize.ToString();
        }
    }

    public void UpdateQuestPreview()
    {
        questPreview.SetupQuestPreview(questInSlot);
    }
}
