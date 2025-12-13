using System;
using System.Text;
using TMPro;
using UnityEditor.Playables;
using UnityEngine;

public class UI_ItemToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemType;
    [SerializeField] private TextMeshProUGUI itemInfo;

    [SerializeField] private TextMeshProUGUI itemPrice;
    [SerializeField] private Transform merchantInfo;

    public void ShowToolTip(bool show, RectTransform targetTransform, Inventory_Item itemToShow, bool buyPrice = false, bool showMerchantInfo = false)
    {
        base.ShowToolTip(show, targetTransform);

        merchantInfo.gameObject.SetActive(showMerchantInfo);

        int price = buyPrice ? itemToShow.buyPrice : Mathf.FloorToInt(itemToShow.sellPrice);
        int totalPrice = price * itemToShow.stackSize;

        // Debug logging to see what's happening
        Debug.Log($"Item: {itemToShow.itemData.itemName}, buyPrice: {itemToShow.buyPrice}, sellPrice: {itemToShow.sellPrice}, itemData.itemPrice: {itemToShow.itemData.itemPrice}");

        string fullStackPrice = ($"Price: {price}x{itemToShow.stackSize} - {totalPrice}g.");
        string singleStackPrice = ($"Price: {price}g.");

        itemPrice.text = itemToShow.stackSize > 1 ? fullStackPrice : singleStackPrice;
        itemName.text = itemToShow.itemData.itemName;
        itemType.text = itemToShow.itemData.itemType.ToString();
        itemInfo.text = itemToShow.GetItemInfo();
    }
}