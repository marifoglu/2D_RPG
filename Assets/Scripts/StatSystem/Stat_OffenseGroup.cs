using System;
using UnityEngine;

[Serializable]
public class Stat_OffenseGroup
{

    public Stat attackSpeed;


    //Phsyical Attack Power
    public Stat damage;
    public Stat critPower;
    public Stat critChance;
    public Stat armorReduction;
    public Stat counterAttackDamage; // counter attacks maybe?

    //Elemental Attack Power    
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;
}
