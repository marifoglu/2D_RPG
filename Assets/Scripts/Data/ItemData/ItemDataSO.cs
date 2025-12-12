using UnityEngine;

[CreateAssetMenu(fileName = "Material data - ", menuName = "RPG Setup/Item data/Material item")]
public class ItemDataSO : ScriptableObject
{
    [Header("Merchant Details")]
    [Range(0, 10000)]
    public int itemPrice;
    public int minStackSizeAtShop = 1;
    public int maxStackSizeAtShop = 1;

    [Header("Craft Details")]
    public Inventory_Item[] craftReceipe;

    [Header("Item Details")]
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public int maxStackSize = 1;

    [Header("Item Effects")]
    public ItemEffectDataSO itemEffect;


}
