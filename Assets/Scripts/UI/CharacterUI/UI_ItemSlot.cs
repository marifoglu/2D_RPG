using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Inventory_Item itemInSlot { get; private set; }
    protected Inventory_Player inventory;
    protected UI ui;
    protected RectTransform rect;

    [Header("UI Item Slot")]
    [SerializeField] protected Image itemIcon;
    [SerializeField] protected TextMeshProUGUI itemStackSize;

    protected virtual void Awake()
    {
        ui = GetComponentInParent<UI>();
        if (ui == null)
            ui = FindAnyObjectByType<UI>();

        rect = GetComponent<RectTransform>();
        inventory = FindAnyObjectByType<Inventory_Player>();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null || itemInSlot.itemData.itemType == ItemType.Material)
            return;

        bool alternativeInput = Input.GetKey(KeyCode.V);
        if (alternativeInput)
        {
            inventory.RemoveOneItem(itemInSlot);
        }
        else
        {
            if (itemInSlot.itemData.itemType == ItemType.Consumable)
            {


                inventory.TryUseItem(itemInSlot);
            }
            else
            {
                inventory.TryEquipItem(itemInSlot);
            }
        }

        if (itemInSlot == null && ui != null && ui.itemToolTip != null)
            ui.itemToolTip.ShowToolTip(false, null);
    }

    public void UpdateSlot(Inventory_Item item)
    {
        itemInSlot = item;

        if (itemInSlot == null)
        {
            itemStackSize.text = "";
            itemIcon.color = Color.clear;
            return;
        }

        Color color = Color.white; color.a = .9f;
        itemIcon.color = color;
        itemIcon.sprite = itemInSlot.itemData.itemIcon;
        itemStackSize.text = item.stackSize > 1 ? itemInSlot.stackSize.ToString() : "";
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (itemInSlot == null)
            return;

        if (ui != null && ui.itemToolTip != null)
            ui.itemToolTip.ShowToolTip(true, rect, itemInSlot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ui != null && ui.itemToolTip != null)
            ui.itemToolTip.ShowToolTip(false, null);
    }
}