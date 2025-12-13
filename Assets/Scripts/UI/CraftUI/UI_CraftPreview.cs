using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftPreview : MonoBehaviour
{
    private Inventory_Item itemToCraft;
    private Inventory_Storage storage;
    private UI_CraftPreviewSlot[] craftPreviewSlots;

    [Header("Item Preview Setup")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemInfo;
    [SerializeField] private TextMeshProUGUI buttonText;

    public void SetupCraftPreview(Inventory_Storage storage)
    {
        this.storage = storage;

        craftPreviewSlots = GetComponentsInChildren<UI_CraftPreviewSlot>();
        
        foreach (var slot in craftPreviewSlots)
            slot.gameObject.SetActive(false);
    }

    public void ConfirmCraft()
    {
        if (itemToCraft == null)
        {
            buttonText.text = "Pick an item";
            return;
        }

        if (storage.CanCraftItem(itemToCraft))
            storage.CraftItem(itemToCraft);

        UpdateCraftPreviewSlots();
    }

    public void UpdateCraftPreview(ItemDataSO itemData)
    {
        itemToCraft = new Inventory_Item(itemData);

        itemIcon.sprite = itemData.itemIcon;
        itemName.text = itemData.itemName;
        itemInfo.text = itemToCraft.GetItemInfo();

        UpdateCraftPreviewSlots();
    }



    private void UpdateCraftPreviewSlots()
    {
        if (itemToCraft == null)
            return;
        

        if (craftPreviewSlots == null || craftPreviewSlots.Length == 0)
            return;
        

        foreach (var slot in craftPreviewSlots)
            slot.gameObject.SetActive(false);

        int recipeLength = itemToCraft.itemData.craftReceipe.Length;

        if (recipeLength > craftPreviewSlots.Length)
        {
            recipeLength = craftPreviewSlots.Length; // Only process what we have
        }

        for (int i = 0; i < recipeLength; i++)
        {
            Inventory_Item requiredItem = itemToCraft.itemData.craftReceipe[i];
            int availableAmount = storage.GetAvailableAmountOf(requiredItem.itemData);
            int requiredAmount = requiredItem.stackSize;

            craftPreviewSlots[i].gameObject.SetActive(true);
            craftPreviewSlots[i].SetupPreviewSlot(requiredItem.itemData, availableAmount, requiredAmount);
        }
    }
}