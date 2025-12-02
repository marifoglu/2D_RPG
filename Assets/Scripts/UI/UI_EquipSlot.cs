using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipSlot : UI_ItemSlot
{
    public ItemType slotType;

    private void OnValidate()
    {
        gameObject.name = "UI_Equipment Slot : " + slotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(itemSlot == null)
            return;

        inventory.UnEquipItem(itemSlot);
    }
}
