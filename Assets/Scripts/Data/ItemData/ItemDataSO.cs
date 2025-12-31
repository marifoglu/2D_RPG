using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Material data - ", menuName = "RPG Setup/Item data/Material item")]
public class ItemDataSO : ScriptableObject
{
    public string saveID;

    [Header("Merchant Details")]
    [Range(0, 10000)]
    public int itemPrice;
    public int minStackSizeAtShop = 1;
    public int maxStackSizeAtShop = 1;

    [Header("Drop Details")]
    [Range(0, 1000)]
    public int itemRarity = 100;
    [Range(0, 100)]
    public float dropChance;
    [Range(0, 100)]
    public float maxDropChance = 65f;

    [Header("Craft Details")]
    public Inventory_Item[] craftReceipe;

    [Header("Item Details")]
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public int maxStackSize = 1;

    [Header("Item Effects")]
    public ItemEffectDataSO itemEffect;

    public void OnValidate()
    {
        dropChance = GetDropChance();

#if UNITY_EDITOR
        string assetPath = AssetDatabase.GetAssetPath(this);
        saveID = AssetDatabase.AssetPathToGUID(assetPath);
#endif
    }

    public float GetDropChance()
    {
        float maxRarity = 1000f;
        float chance = (maxRarity - itemRarity + 1) / maxRarity * 100;

        return Mathf.Min(chance, maxDropChance);
    }
}
