using UnityEngine;

[CreateAssetMenu(fileName = "Material data - ", menuName = "RPG Setup/Item data/Material item")]

public class ItemDataSO : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public int maxStackSize = 1;

    [Header("Item Effects")]
    public ItemEffectDataSO itemEffect;
}
