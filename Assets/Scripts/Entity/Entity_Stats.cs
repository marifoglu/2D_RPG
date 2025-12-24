using UnityEngine;


public class Entity_Stats : MonoBehaviour
{
    public Stat_SetupSO defaultStatSetup; 

    public Stat_ResourceGroup resources;
    public Stat_OffenseGroup offense;
    public Stat_DefenseGroup defense;
    public Stat_MajorGroup major;

    protected virtual void Awake()
    {
        // Optionally apply default stats on awake
        // ApplyStatSetup();
    }

    public AttackData GetAttackData(DamageScaleData damageScaleData)
    {
        return new AttackData(this, damageScaleData);
    }

    // Health Section
    public float GetMaxHealth()
    {
        float baseMaxHealth = resources.maxHealth.GetValue();
        float bonusMaxHealth = major.vitality.GetValue() * 5f;
        
        float finalMaxHealth = baseMaxHealth + bonusMaxHealth;

        return finalMaxHealth;
    }
    public float GetBaseDamage() => offense.damage.GetValue() + major.strength.GetValue(); // Bonus damage from Strength +1 per STR
    public float GetCritChance() => offense.critChance.GetValue() + (major.agility.GetValue() * 0.3f); // Bonus crit chance from Agility 0.3% per AGI
    public float GetCritPower() => offense.critPower.GetValue() + (major.strength.GetValue() * 0.5f); // Bonus crit power from Strength +1% per STR
    public float GetBaseArmor() => defense.armor.GetValue() + major.vitality.GetValue(); // Bonus armor from Vitality +1 per VIT

    // Physical Damage Section
    public float GetPhysicalDamage(out bool isCrit, float scaleFactor = 1)
    {
        float baseDamage = GetBaseDamage();
        float totalCritChance = GetCritChance();
        float totalCritPower = GetCritPower() / 100;

        isCrit = Random.Range(0f, 100f) < totalCritChance;
        float finalDamage = isCrit ? baseDamage * totalCritPower : baseDamage;

        return finalDamage * scaleFactor;
    }

    // Heavy Attack Damage Section
    public float GetHeavyAttackDamage(out bool isCrit, float scaleFactor = 1)
    {
        float baseDamage = offense.damage.GetValue();
        float bonusDamage = major.strength.GetValue();
        float totalBaseDamage = baseDamage + bonusDamage;

        // Heavy attacks deal 150% base damage (adjust multiplier as needed: 1.5f = 150%, 2.0f = 200%, etc.)
        totalBaseDamage *= 1.5f;

        float baseCritChance = offense.critChance.GetValue();
        float bonusCritChance = major.agility.GetValue() * 0.5f;  // bonus crit chance from Agility 0.5% per AGI
        float totalCritChance = baseCritChance + bonusCritChance;

        float baseCritPower = offense.critPower.GetValue();
        float bonusCritPower = major.strength.GetValue() * 0.5f; // bonus crit power from Strength +1% per STR
        float totalCritPower = (baseCritPower + bonusCritPower) / 100; // Total crit power as a multiplier

        isCrit = Random.Range(0f, 100f) < totalCritChance;
        float finalDamage = isCrit ? totalBaseDamage * totalCritPower : totalBaseDamage;

        return finalDamage * scaleFactor;
    }

    // Evasion Section
    public float GetEvasion()
    {
        float baseEvasion = defense.evasion.GetValue(); // Base evasion from equipment and buffs
        float bonusEvasion = major.agility.GetValue() * 5f;  //each point in agility adds %0.5 evasion

        float totalEvasion = baseEvasion + bonusEvasion; // Total evasion before capping
        float evesionCap = 50f; // Max evasion is 50%
        float finalEvasion = Mathf.Min(totalEvasion, evesionCap); // Cap evasion at 50%

        return finalEvasion;
    }
    public float GetArmorReduction()
    {
        float armorReduction = offense.armorReduction.GetValue();

        return armorReduction;
    }
    public float GetArmorMitigation(float armorReduction)
    {
        float totalArmor = GetBaseArmor();

        float reductionMultiplier = Mathf.Clamp(1f - armorReduction, 0, 1);
        float effectiveArmor = totalArmor * reductionMultiplier;

        float mitigation = effectiveArmor / (effectiveArmor + 100f); // Mitigation formula
        float mitigationCap = 0.75f; // Cap mitigation at 75%

        float finalMitigation = Mathf.Clamp(mitigation, 0f, mitigationCap);


        return finalMitigation;
    }
    public float GetElementalResistance(ElementType elementType)
    {
        float baseResistance = 0f;
        float bonusResistance = major.intelligence.GetValue() * .5f; // + .5% elemental resistance per INT

        switch (elementType)
        {
            case ElementType.Fire:
                baseResistance = defense.fireResist.GetValue();
                break;
            case ElementType.Ice:
                baseResistance = defense.iceResist.GetValue();
                break;
            case ElementType.Lightning:  
                baseResistance = defense.lightingResist.GetValue();
                break;
        }

        float resistace = baseResistance + bonusResistance;
        float resistanceCap = 75f; // Cap resistance at 75%
        float finalResistance = Mathf.Clamp(resistace, 0f, resistanceCap) / 100; // Convert to multiplier

        return finalResistance;
    }
    public float GetElementalDamage(out ElementType elementType, float scaleFactor = 1)
    {
        float fireDamage = offense.fireDamage.GetValue();
        float iceDamage = offense.iceDamage.GetValue();
        float lightingDamage = offense.lightningDamage.GetValue();

        float bonusElementalDamage = major.intelligence.GetValue(); // +1 elemental damage per INT

        // Elemental damage check
        float highiestElementalDamage = fireDamage;
        elementType = ElementType.Fire;


        if (iceDamage > highiestElementalDamage) { 
            highiestElementalDamage = iceDamage;
            elementType = ElementType.Ice;
        }

        if (lightingDamage > highiestElementalDamage)
        {
            highiestElementalDamage = lightingDamage;
            elementType = ElementType.Lightning;
        }

        if (highiestElementalDamage <= 0)
        {
            elementType = ElementType.None;
            return 0;
        }

        float bonusFireDamage = (elementType == ElementType.Fire) ? 0 :  fireDamage * 5f;
        float bonusIceDamage = (elementType == ElementType.Ice) ? 0 :  iceDamage * 5f;
        float bonusLightingDamage = (elementType == ElementType.Lightning) ? 0 :  lightingDamage * 5f;

        float weakerElementalDamage = bonusFireDamage + bonusIceDamage + bonusLightingDamage;

        float finalElementalDamage = highiestElementalDamage + weakerElementalDamage + bonusElementalDamage;

        return finalElementalDamage * scaleFactor;
    }

    // Counter Attack Damage Section
    public float GetCounterAttackDamage()
    {
        float baseCounterDamage = offense.counterAttackDamage.GetValue();
        float bonusCounterDamage = major.strength.GetValue() * 0.5f; // Optional: bonus from STR

        float finalCounterDamage = baseCounterDamage + bonusCounterDamage;

        return finalCounterDamage;
    }
    public float GetMaxStamina()
    {
        float baseMaxStamina = resources.maxStamina.GetValue();
        // Optional: Bonus from stats (like Vitality)
        // float bonusStamina = major.vitality.GetValue() * 2f;

        return baseMaxStamina; // + bonusStamina, i will decide later if i want bonus from stats
    }
    public Stat GetStatByType(StatType statType)
    {
        switch (statType)
        {
            case StatType.MaxHealth:
                return resources.maxHealth;
            case StatType.HealthRegen:
                return resources.healthRegen;

            case StatType.Strength:
                return major.strength;
            case StatType.Agility:
                return major.agility;
            case StatType.Intelligence:
                return major.intelligence;
            case StatType.Vitality:
                return major.vitality;

            case StatType.Armor:
                return defense.armor;
            case StatType.Evasion:
                return defense.evasion;

            case StatType.Damage:
                return offense.damage;
            case StatType.CritChance:
                return offense.critChance;
            case StatType.CritPower:
                return offense.critPower;
            case StatType.AttackSpeed:
                return offense.attackSpeed;
            case StatType.ArmorReduction:
                return offense.armorReduction;

            case StatType.FireDamage:
                return offense.fireDamage;
            case StatType.IceDamage:
                return offense.iceDamage;
            case StatType.LightningDamage:
                return offense.lightningDamage;

            case StatType.FireResistance:
                return defense.fireResist;
            case StatType.IceResistance:
                return defense.iceResist;
            case StatType.LightningResistance:
                return defense.lightingResist;

            case StatType.ElementalDamage:
                // ElementalDamage is calculated dynamically, not a direct stat
                // Return null as it's handled separately in UI_StatSlot.UpdateStaValue()
                return null;

            default:
                Debug.LogWarning($"StatType {statType} not found!");
                return null;
        }
    }

    [ContextMenu("Apply Default Stat Setup")]
    public void ApplyStatSetup()
    {
        if (defaultStatSetup == null)
        {
            Debug.LogWarning("Default Stat Setup is not assigned!");
            return;
        }

        resources.maxHealth.SetBaseValue(defaultStatSetup.maxHealth);
        resources.healthRegen.SetBaseValue(defaultStatSetup.healthRegen);
        resources.maxStamina.SetBaseValue(defaultStatSetup.maxStamina); // Stamina
        resources.staminaRegen.SetBaseValue(defaultStatSetup.staminaRegen);

        major.strength.SetBaseValue(defaultStatSetup.strength);
        major.agility.SetBaseValue(defaultStatSetup.agility);
        major.intelligence.SetBaseValue(defaultStatSetup.intelligence);
        major.vitality.SetBaseValue(defaultStatSetup.vitality);

        offense.damage.SetBaseValue(defaultStatSetup.damage);
        offense.attackSpeed.SetBaseValue(defaultStatSetup.attackSpeed);
        offense.critChance.SetBaseValue(defaultStatSetup.critChance);
        offense.critPower.SetBaseValue(defaultStatSetup.critPower);
        offense.armorReduction.SetBaseValue(defaultStatSetup.armorReduction);

        offense.fireDamage.SetBaseValue(defaultStatSetup.fireDamage);
        offense.iceDamage.SetBaseValue(defaultStatSetup.iceDamage);
        offense.lightningDamage.SetBaseValue(defaultStatSetup.lightningDamage);

        defense.armor.SetBaseValue(defaultStatSetup.armor);
        defense.evasion.SetBaseValue(defaultStatSetup.evasion);

        defense.fireResist.SetBaseValue(defaultStatSetup.fireResistance);
        defense.iceResist.SetBaseValue(defaultStatSetup.iceResistance);
        defense.lightingResist.SetBaseValue(defaultStatSetup.lightninResistance);
    }
}