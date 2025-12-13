using UnityEngine;

public class UI_PlayerStats : MonoBehaviour
{
    private UI_StatSlot[] UIStatSlots;
    private Inventory_Player inventory;

    private void Awake()
    {
        UIStatSlots = GetComponentsInChildren<UI_StatSlot>();

        inventory = FindFirstObjectByType<Inventory_Player>();
        inventory.OnInventoryChange += UpdateStatsUI;
    }

    private void Start()
    {
        UpdateStatsUI();
    }

    private void UpdateStatsUI()
    {
        foreach (var slot in UIStatSlots)
        {
            slot.UpdateStaValue();
        }
    }
}
