using System;
using UnityEngine;

[Serializable]
public class Stat_OffenseGroup
{
    //Phsyical Attack Power
    public Stat damage;
    public Stat critPower;
    public Stat critChance;

    //Elemental Attack Power    
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;
}
