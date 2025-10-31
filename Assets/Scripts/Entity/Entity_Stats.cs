using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stat maxHealth;
    public Stat vitality;

    //public float maxHealth = 100f;
    //public float vitality = 7f;
    //public float agility;
    //public float strength;
    //public float intellgence;
    //public float damage;
    //public float crtChance;


    // Equipped weapon addes  +5 Damage, +10 Crit Chance, +3 Evasion
    // Portions add +10 armor for 10 secs
    public float GetMaxHealth()
    {
        float baseHp = maxHealth.GetValue();
        float bonusHp = vitality.GetValue() * 5f;
        
        return baseHp + bonusHp;
    }
}
