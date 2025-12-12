using UnityEngine;

public class UI_Craft : MonoBehaviour
{

    [SerializeField] private UI_ItemSlotParent inventoryParent;
    private Inventory_Player inventory;

    private UI_CraftSlot[] craftSlots;
    private UI_CraftListButton[] craftListButton; 
    private UI_CraftPreview craftPreview;

    public void SetupCraftUI(Inventory_Storage storage)
    {
        inventory = storage.playerInventory;
        inventory.OnInventoryChange += UpdateUI;
        UpdateUI();

        craftPreview = GetComponentInChildren<UI_CraftPreview>();
        craftPreview.SetupPreviewSlots(storage);
        SetupCraftListButtons();
    }
    private void SetupCraftListButtons()
    {
        craftSlots = GetComponentsInChildren<UI_CraftSlot>();
        craftListButton = GetComponentsInChildren<UI_CraftListButton>();

        foreach (var slots in craftSlots)
            slots.gameObject.SetActive(false);
        
        foreach (var button in craftListButton)
            button.SetCraftSlots(craftSlots);
    }
        
    private void UpdateUI() => inventoryParent.UpdateSlots(inventory.itemList);

}
