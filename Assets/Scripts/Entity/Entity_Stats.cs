using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stat maxHealth;
    public Stat_MajorGroup major;
    public Stat_OffenseGroup offense;
    public Stat_DefenseGroup defense;

    // Equipped weapon addes  +5 Damage, +10 Crit Chance, +3 Evasion
    // Portions add +10 armor for 10 secs
    public float GetMaxHealth()
    {
        float baseHp = maxHealth.GetValue();
        float bonusHp = major.vitality.GetValue() * 5f;
        
        return baseHp + bonusHp;
    }

    public float GetEvasion()
    {
        float baseEvasion = defense.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue() * 5f;  //each point in agility adds %0.5 evasion

        return baseEvasion + bonusEvasion;
    }
}
