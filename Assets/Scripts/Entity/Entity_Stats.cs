using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
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

    public float GetArmorMitigation()
    {
        float baseArmor = defense.armor.GetValue();
        float bonusArmor = major.vitality.GetValue();
        float totalArmor = baseArmor + bonusArmor;  

        float mitigation = totalArmor / (totalArmor + 100f); // Mitigation formula
        float mitigationCap = 0.75f; // Cap mitigation at 75%

        float finalMitigation = Mathf.Clamp(mitigation, 0f, mitigationCap);


        return finalMitigation;
    }


}
