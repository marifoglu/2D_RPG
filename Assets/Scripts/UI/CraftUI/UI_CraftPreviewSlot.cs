using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftPreviewSlot : MonoBehaviour
{
    [SerializeField] private Image materialIcon;
    [SerializeField] private TextMeshProUGUI materialNameValue;

    public void SetupPreviewSlot(ItemDataSO itemData, int availableAmount, int requiredAmount)
    {
        materialIcon.sprite = itemData.itemIcon;
        materialNameValue.text = itemData.itemName + " - " + availableAmount + "/" + requiredAmount;
    }

}
