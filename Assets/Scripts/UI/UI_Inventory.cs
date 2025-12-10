using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    private Inventory_Player inventory;
    private UI_EquipSlot[] uiEquipSlot;

    [SerializeField] private UI_ItemSlotParent inventorySlotsParent;
    [SerializeField] private Transform uiEquipSlotParent;

    private void Awake()
    {

        uiEquipSlot = uiEquipSlotParent.GetComponentsInChildren<UI_EquipSlot>();

        inventory = FindFirstObjectByType<Inventory_Player>();
        inventory.OnInventoryChange += UpdateUI;

        UpdateUI();
    }

    private void UpdateUI()
    {
        UpdateEquipmentSlot();
        inventorySlotsParent.UpdateSlots(inventory.itemList);
    }
    private void UpdateEquipmentSlot()
    {
        List<Inventory_EquipmentSlot> equipList = inventory.equipList;

        for(int i = 0; i < uiEquipSlot.Length; i++)
        {
            var playerEquipSlot = equipList[i];

            if(playerEquipSlot.HasItem() == false)
                uiEquipSlot[i].UpdateSlot(null);
            else
                uiEquipSlot[i].UpdateSlot(playerEquipSlot.equipedItem);
        }
    }
}
