using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment data - ", menuName = "RPG Setup/Item data/Equipment item")]

public class EquipmentDataSO : ItemDataSO
{
    [Header("Item modifier")]
    public ItemModifier[] modifiers;
}

[Serializable]
public struct ItemModifier
{
    public StatType statType;
    public float value;
}
