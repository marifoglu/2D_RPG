using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ActiveQuestsSlots : MonoBehaviour
{
    private QuestData questInSlot;
    private UI_ActiveQuestsPreview questPreview;

    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private Image[] questRewardPreview;

    public void SetActiveQuestSlot(QuestData questToSetup)
    {
        questPreview = transform.root.GetComponentInChildren<UI_ActiveQuestsPreview>();
        questInSlot = questToSetup;

        questName.text = questInSlot.questDataSo.questName;

        Inventory_Item[] reward = questInSlot.questDataSo.rewardItems;

        foreach (var previewIcon in questRewardPreview)
        {
            previewIcon.gameObject.SetActive(false);
        }

        for(int i = 0; i < reward.Length; i++)
        {
            if (reward[i] != null) continue;
            Image preview = questRewardPreview[i];

            preview.gameObject.SetActive(true);
            preview.sprite = reward[i].itemData.itemIcon;
            preview.GetComponentInChildren<TextMeshProUGUI>().text = reward[i].stackSize.ToString();
        }
    }

    public void SetupPreview()
    {
        questPreview.SetupQuestPreview(questInSlot);
    }

}
