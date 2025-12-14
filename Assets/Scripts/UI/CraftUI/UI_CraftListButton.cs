using Unity.VisualScripting;
using UnityEngine;

public class UI_CraftListButton : MonoBehaviour
{
    [SerializeField] private ItemListDataSO craftData;
    private UI_CraftSlot[] craftSlots;

    public void SetCraftSlots(UI_CraftSlot[] craftSlots) => this.craftSlots = craftSlots;

    public void UpdateCraftSlots()
    {
        if (craftData == null)
        {
            Debug.LogWarning("Craft data is not assigned in UI_CraftListButton.");
            return;
        }

        if (craftSlots == null || craftSlots.Length == 0)
        {
            Debug.LogWarning("Craft slots are not assigned or empty in UI_CraftListButton.");
            return;
        }

        foreach (var slot in craftSlots)
        {
            slot.gameObject.SetActive(false);
        }

        int itemCount = Mathf.Min(craftData.itemList.Length, craftSlots.Length);

        for (int i = 0; i < itemCount; i++)
        {
            ItemDataSO itemData = craftData.itemList[i];

            craftSlots[i].gameObject.SetActive(true);
            craftSlots[i].SetupButton(itemData);
        }

        if (craftData.itemList.Length > craftSlots.Length)
        {
            Debug.LogWarning($"Craft data has {craftData.itemList.Length} items but only {craftSlots.Length} slots available. Some items won't be displayed.");
        }
    }
}