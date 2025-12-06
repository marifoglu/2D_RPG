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


    public void ShowToolTip(bool show, RectTransform targetTransform, Inventory_Item itemToShow)
    {
        base.ShowToolTip(show, targetTransform);

        itemName.text = itemToShow.itemData.itemName;
        itemType.text = itemToShow.itemData.itemType.ToString();
        itemInfo.text = GetItemInfo(itemToShow);
    }

    public string GetItemInfo(Inventory_Item item)
    {
        if(item.itemData.itemType == ItemType.Material)
            return "Used  for crafting various items.";

        if (item.itemData.itemType == ItemType.Consumable)
            return item.itemData.itemEffect.effectDescription;

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("");

        foreach (var mod in item.modifiers)
        {
            string modType = GetStatNameByType(mod.statType);
            string modValue = IsPercentageStat(mod.statType) ? mod.value.ToString() + "%" : mod.value.ToString();

            sb.AppendLine("+ " + modValue + "  " + modType);
        }   
        return sb.ToString();
    }

    private string GetStatNameByType(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHealth: return "Max Health";
            case StatType.HealthRegen: return "Health Regenaration";
            case StatType.Strength: return "Strength";
            case StatType.Agility: return "Agility";
            case StatType.Intelligence:  return "Intelligence";
            case StatType.AttackSpeed:  return "Attack Speed";
            case StatType.Damage: return "Damage";
            case StatType.CritChance:  return "Critical Chance";
            case StatType.CritPower: return "ACritical Power";
            case StatType.ArmorReduction: return "ArmorReduction";

            case StatType.FireDamage: return "Fire Damage";
            case StatType.IceDamage: return "Ice Damage";
            case StatType.LightningDamage: return "Lightning Damage";

            case StatType.Armor: return "Armor";
            case StatType.Evasion: return "Evasion";

            case StatType.FireResistance: return "Fire Resistance";
            case StatType.IceResistance: return "Ice Resistance";
            case StatType.LightningResistance: return "Lightning Resistance";

            default:
                return "Unknown Stat";
        }
    }

    private bool IsPercentageStat(StatType type)
    {
        switch (type)
        {
            case StatType.CritChance:
            case StatType.CritPower:
            case StatType.ArmorReduction:
            case StatType.Evasion:
            case StatType.FireResistance:
            case StatType.IceResistance:
            case StatType.LightningResistance:
            case StatType.AttackSpeed:
                return true;
            default:
                return false;
        }
    }
}
