using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public ElementType elementType;
    public Stat maxHealth;
    public Stat_MajorGroup major;
    public Stat_OffenseGroup offense;
    public Stat_DefenseGroup defense;


    // Health Section
    public float GetMaxHealth()
    {
        float baseMaxHealth = maxHealth.GetValue();
        float bonusMaxHealth = major.vitality.GetValue() * 5f;
        
        float finalMaxHealth = baseMaxHealth + bonusMaxHealth;

        return finalMaxHealth;
    }

    // Physical Damage Section
    public float GetPhysicalDamage(out bool isCrit)
    {
        float baseDamage = offense.damage.GetValue();
        float bonusDamage = major.strength.GetValue();
        float totalBaseDamage = baseDamage + bonusDamage;

        float baseCritChance = offense.critChance.GetValue();
        float bonusCritChance = major.agility.GetValue() * 0.5f;  //bonus crt chance from Agility %0.5 per AGI
        float totalCritChance = baseCritChance + bonusCritChance;

        float baseCritPower = offense.critPower.GetValue();
        float bonusCritPower = major.strength.GetValue() * .5f; //bonus crit power from Strength +1% per STR
        float totalCritPower = baseCritPower + bonusCritPower /100; // Total crit power as a multiplier

        isCrit = Random.Range(0f, 100f) < totalCritChance;
        float finalDamage = isCrit ? totalBaseDamage * totalCritPower: totalBaseDamage;

        return finalDamage;
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
        float baseArmor = defense.armor.GetValue();
        float bonusArmor = major.vitality.GetValue();
        float totalArmor = baseArmor + bonusArmor;  

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
        float bonusResistance = major.intellgence.GetValue() * .5f; // + .5% elemental resistance per INT

        switch (elementType)
        {
            case ElementType.Fire:
                baseResistance = defense.fireResist.GetValue();
                break;
            case ElementType.Ice:
                baseResistance = defense.iceResist.GetValue();
                break;
            case ElementType.Lighting:  
                baseResistance = defense.lightingResist.GetValue();
                break;
        }

        float resistace = baseResistance + bonusResistance;
        float resistanceCap = 75f; // Cap resistance at 75%
        float finalResistance = Mathf.Clamp(resistace, 0f, resistanceCap) / 100; // Convert to multiplier

        return finalResistance;
    }
    public float GetElementelDamage(out ElementType elementType)
    {
        float fireDamage = offense.fireDamage.GetValue();
        float iceDamage = offense.iceDamage.GetValue();
        float lightingDamage = offense.lightingDamage.GetValue();

        float bonusElementalDamage = major.intellgence.GetValue(); // +1 elemental damage per INT

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
            elementType = ElementType.Lighting;
        }

        if (highiestElementalDamage <= 0)
        {
            elementType = ElementType.None;
            return 0;
        }

        float bonusFireDamage = (fireDamage == highiestElementalDamage) ? 0 :  fireDamage * 5f;
        float bonusIceDamage = (iceDamage == highiestElementalDamage) ? 0 :  iceDamage * 5f;
        float bonusLightingDamage = (lightingDamage == highiestElementalDamage) ? 0 :  lightingDamage * 5f;

        float weakerElementalDamage = bonusFireDamage + bonusIceDamage + bonusLightingDamage;

        float finalElementalDamage = highiestElementalDamage + weakerElementalDamage + bonusElementalDamage;

        return finalElementalDamage;
    }


}

// Done