using UnityEngine;

[CreateAssetMenu(fileName = "Default stat menu", menuName = "RPG Setup/Default stat setup")]
public class Stat_SetupSO : ScriptableObject
{
    [Header("Resources")]
    public float maxHealth=100f;
    public float healthRegen;
    public float maxStamina = 100f;
    public float staminaRegen = 10f; // Stamina per second

    [Header("Offense - Physical Damage")]
    public float damage = 10f;
    public float attackSpeed = 1f;
    public float critChance;
    public float critPower = 150f;
    public float armorReduction;

    [Header("Offense - Elemental Damage")]
    public float fireDamage;
    public float iceDamage;
    public float lightningDamage;


    [Header("Defense - Physical Damage")]
    public float armor;
    public float evasion;

    [Header("Defense - Elemental Damage")]
    public float fireResistance;
    public float iceResistance;
    public float lightninResistance;

    [Header("Major Stats")]
    public float strength;
    public float agility;
    public float intelligence;
    public float vitality ;
}
