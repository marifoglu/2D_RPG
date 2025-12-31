using UnityEngine;
using UnityEngine.EventSystems;

public class UI_QuestRewardSlot : UI_ItemSlot
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (itemInSlot == null) return;

        if (ui != null && ui.itemToolTip != null)
            ui.itemToolTip.ShowToolTip(true, rect, itemInSlot, false, false, false);
    }
}