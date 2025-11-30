using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler
{
    public Inventory_Item itemSlot { get; private set; }
    private Inventory_Player inventory;

    [Header("UI Item Slot")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemStackSize;

    private void Awake()
    {
        inventory = FindAnyObjectByType<Inventory_Player>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (itemSlot == null)
            return;

        inventory.TryEquipItem(itemSlot);
    }

    public void UpdateSlot(Inventory_Item item)
    {
        itemSlot = item;

        if(itemSlot == null)
        {
            itemStackSize.text = "";
            itemIcon.color = Color.clear;
            return;
        }

        Color color = Color.white; color.a = .9f;
        itemIcon.color = color;
        itemIcon.sprite = itemSlot.itemData.itemIcon;
        itemStackSize.text = item.stackSize > 1 ? itemSlot.stackSize.ToString() : "";
    }

  
}
