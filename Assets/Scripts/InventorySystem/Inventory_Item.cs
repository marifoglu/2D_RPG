using System;
using System.Text;
using UnityEngine;

[Serializable]
public class Inventory_Item
{
    public ItemDataSO itemData;
    public int stackSize = 1;
    private string itemId;
    public ItemModifier[] modifiers { get; private set; }
    public ItemEffectDataSO itemEffect;

    public int buyPrice { get; private set; }
    public float sellPrice { get; private set; }

    public Inventory_Item(ItemDataSO itemData)
    {
        this.itemData = itemData;
        itemEffect = itemData.itemEffect;
        buyPrice = itemData.itemPrice;
        sellPrice = itemData.itemPrice * 0.35f; // Lower price for selling to merchants

        modifiers = EquipmentData()?.modifiers;

        itemId = itemData.itemName + " - " + Guid.NewGuid();
    }

    public void AddModifiers(Entity_Stats playerStats)
    {
        if (modifiers == null || modifiers.Length == 0)
            return;

        foreach (var mod in modifiers)
        {
            Stat statToModify = playerStats.GetStatByType(mod.statType);
            if (statToModify != null)
                statToModify.AddModifier(mod.value, itemId);
        }
    }

    public void RemoveModifiers(Entity_Stats playerStats)
    {
        if (modifiers == null || modifiers.Length == 0)
            return;

        foreach (var mod in modifiers)
        {
            Stat statToModify = playerStats.GetStatByType(mod.statType);
            if (statToModify != null)
                statToModify.RemoveModifier(itemId);
        }
    }

    private EquipmentDataSO EquipmentData()
    {
        if (itemData is EquipmentDataSO equipment)
            return equipment;
        return null;
    }

    public bool CanAddStack() => stackSize < itemData.maxStackSize;
    public void AddStack() => stackSize++;
    public void RemoveStack() => stackSize--;

    public void AddItemEffect(Player player) => itemEffect?.Subscribe(player);
    public void RemoveItemEffect() => itemEffect?.Unsubscribe();

    public string GetItemInfo()
    {
        StringBuilder sb = new StringBuilder();

        // Handle Material items
        if (itemData.itemType == ItemType.Material)
        {
            sb.AppendLine("");
            sb.AppendLine("Used for crafting");
            sb.AppendLine("");
            sb.AppendLine("");
            return sb.ToString();
        }

        // Handle Consumable items
        if (itemData.itemType == ItemType.Consumable)
        {
            sb.AppendLine("");
            if (itemEffect != null && !string.IsNullOrEmpty(itemEffect.effectDescription))
            {
                sb.AppendLine(itemEffect.effectDescription);
            }
            else
            {
                sb.AppendLine("Consumable item");
            }
            sb.AppendLine("");
            sb.AppendLine("");
            return sb.ToString();
        }

        // Handle Equipment items (Weapon, Armor, Trinket)
        sb.AppendLine("");

        // Only show modifiers if this is equipment and has modifiers
        if (modifiers != null && modifiers.Length > 0)
        {
            foreach (var mod in modifiers)
            {
                string modType = GetStatNameByType(mod.statType);
                string modValue = IsPercentageStat(mod.statType) ? mod.value.ToString() + "%" : mod.value.ToString();

                sb.AppendLine("+ " + modValue + "  " + modType);
            }
        }
        else
        {
            // If no modifiers, show a placeholder
            sb.AppendLine("No stat bonuses");
        }

        // Show unique effect if present
        if (itemEffect != null && !string.IsNullOrEmpty(itemEffect.effectDescription))
        {
            sb.AppendLine("");
            sb.AppendLine("Unique Effect : ");
            sb.AppendLine(itemEffect.effectDescription);
        }

        sb.AppendLine("");
        sb.AppendLine("");

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
            case StatType.Intelligence: return "Intelligence";
            case StatType.AttackSpeed: return "Attack Speed";
            case StatType.Damage: return "Damage";
            case StatType.CritChance: return "Critical Chance";
            case StatType.CritPower: return "Critical Power";
            case StatType.ArmorReduction: return "Armor Reduction";

            case StatType.FireDamage: return "Fire Damage";
            case StatType.IceDamage: return "Ice Damage";
            case StatType.LightningDamage: return "Lightning Damage";

            case StatType.Armor: return "Armor";
            case StatType.Evasion: return "Evasion";
            case StatType.Vitality: return "Vitality";

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