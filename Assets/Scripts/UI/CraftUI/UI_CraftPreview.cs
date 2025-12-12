//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class UI_CraftPreview : MonoBehaviour
//{
//    private Inventory_Item itemToCraft;
//    private Inventory_Storage storage;
//    private UI_CraftPreviewMaterialSlot[] craftPreviewSlots;

//    [Header("Item Preview Setup")]
//    [SerializeField] private Image itemIcon;
//    [SerializeField] private TextMeshProUGUI itemName;
//    [SerializeField] private TextMeshProUGUI itemInfo;
//    [SerializeField] private TextMeshProUGUI buttonText;

//    public void SetupPreviewSlots(Inventory_Storage storage)
//    {
//        this.storage = storage;

//        craftPreviewSlots = GetComponentsInChildren<UI_CraftPreviewMaterialSlot>();
//        foreach (var slot in craftPreviewSlots)
//        {
//            slot.gameObject.SetActive(false);
//        }
//    }

//    public void UpdateCraftPreview(ItemDataSO itemData)
//    {
//        itemToCraft = new Inventory_Item(itemData);

//        itemIcon.sprite = itemData.itemIcon;
//        itemName.text = itemData.itemName;
//        itemInfo.text = itemToCraft.GetItemInfo();

//        UpdateCraftPreviewSlots();
//    }

//    public void ConfirmCraft()
//    {
//        if (itemToCraft == null)
//        {
//            buttonText.text = "No Item Selected";
//            return;
//        }

//        if (storage.HasEnoughMaterials(itemToCraft) && storage.playerInventory.CanAddItem(itemToCraft))
//        {
//            storage.ConsumeMaterials(itemToCraft);
//            storage.playerInventory.AddItem(itemToCraft);
//        }
//        UpdateCraftPreviewSlots();
//    }
//    private void UpdateCraftPreviewSlots()
//    {
//        foreach (var slot in craftPreviewSlots)
//        {
//            slot.gameObject.SetActive(false);
//        }

//        for (int i = 0; i < itemToCraft.itemData.craftReceipe.Length; i++)
//        {
//            Inventory_Item requiredItem = itemToCraft.itemData.craftReceipe[i];
//            int availableAmount = storage.GetAvailableAmountOf(requiredItem.itemData);
//            int requiredAmount = requiredItem.stackSize;

//            craftPreviewSlots[i].gameObject.SetActive(true);
//            craftPreviewSlots[i].SetupMaterialSlot(requiredItem.itemData, availableAmount, requiredAmount);
//        }
//    }
//}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftPreview : MonoBehaviour
{
    private Inventory_Item itemToCraft;
    private Inventory_Storage storage;
    private UI_CraftPreviewMaterialSlot[] craftPreviewSlots;

    [Header("Item Preview Setup")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemInfo;
    [SerializeField] private TextMeshProUGUI buttonText;

    public void SetupPreviewSlots(Inventory_Storage storage)
    {
        this.storage = storage;

        craftPreviewSlots = GetComponentsInChildren<UI_CraftPreviewMaterialSlot>();
        foreach (var slot in craftPreviewSlots)
        {
            slot.gameObject.SetActive(false);
        }
    }

    public void UpdateCraftPreview(ItemDataSO itemData)
    {
        itemToCraft = new Inventory_Item(itemData);

        itemIcon.sprite = itemData.itemIcon;
        itemName.text = itemData.itemName;
        itemInfo.text = itemToCraft.GetItemInfo();

        UpdateCraftPreviewSlots();
    }

    public void ConfirmCraft()
    {
        Debug.Log("=== ConfirmCraft Called ===");

        if (itemToCraft == null)
        {
            Debug.LogWarning("Craft failed: No item selected to craft");
            SetButtonText("No Item Selected");
            return;
        }

        Debug.Log($"Attempting to craft: {itemToCraft.itemData.itemName}");

        if (storage == null)
        {
            Debug.LogError("Craft failed: Storage is null");
            SetButtonText("Storage Error");
            return;
        }

        bool hasEnoughMaterials = storage.HasEnoughMaterials(itemToCraft);
        Debug.Log($"Has enough materials: {hasEnoughMaterials}");

        bool canAddItem = storage.playerInventory.CanAddItem(itemToCraft);
        Debug.Log($"Can add item to inventory: {canAddItem}");

        if (hasEnoughMaterials && canAddItem)
        {
            Debug.Log("Crafting item...");
            storage.ConsumeMaterials(itemToCraft);
            storage.playerInventory.AddItem(itemToCraft);
            Debug.Log($"Successfully crafted: {itemToCraft.itemData.itemName}");
            SetButtonText("Crafted!");
        }
        else
        {
            if (!hasEnoughMaterials)
            {
                Debug.LogWarning("Craft failed: Not enough materials");
                SetButtonText("Not Enough Materials");
            }
            else if (!canAddItem)
            {
                Debug.LogWarning("Craft failed: Inventory is full");
                SetButtonText("Inventory Full");
            }
        }

        UpdateCraftPreviewSlots();
    }

    private void SetButtonText(string text)
    {
        if (buttonText != null)
        {
            buttonText.text = text;
        }
        else
        {
            Debug.LogWarning($"ButtonText UI element is not assigned. Message: {text}");
        }
    }

    private void UpdateCraftPreviewSlots()
    {
        if (itemToCraft == null)
        {
            Debug.LogWarning("Cannot update craft preview slots: itemToCraft is null");
            return;
        }

        if (craftPreviewSlots == null || craftPreviewSlots.Length == 0)
        {
            Debug.LogError("Cannot update craft preview slots: craftPreviewSlots array is null or empty");
            return;
        }

        foreach (var slot in craftPreviewSlots)
        {
            slot.gameObject.SetActive(false);
        }

        int recipeLength = itemToCraft.itemData.craftReceipe.Length;

        if (recipeLength > craftPreviewSlots.Length)
        {
            Debug.LogError($"Not enough preview slots! Recipe requires {recipeLength} slots but only {craftPreviewSlots.Length} available. Add more UI_CraftPreviewMaterialSlot children to the craft preview panel.");
            recipeLength = craftPreviewSlots.Length; // Only process what we have
        }

        for (int i = 0; i < recipeLength; i++)
        {
            Inventory_Item requiredItem = itemToCraft.itemData.craftReceipe[i];
            int availableAmount = storage.GetAvailableAmountOf(requiredItem.itemData);
            int requiredAmount = requiredItem.stackSize;

            Debug.Log($"Material {i}: {requiredItem.itemData.itemName} - Available: {availableAmount}/{requiredAmount}");

            craftPreviewSlots[i].gameObject.SetActive(true);
            craftPreviewSlots[i].SetupMaterialSlot(requiredItem.itemData, availableAmount, requiredAmount);
        }
    }
}