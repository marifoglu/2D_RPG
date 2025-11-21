using System;
using UnityEngine;

[Serializable]
public class AttackData
{
    public float physicalDamage;
    public float elementaldamage;
    public bool isCrit;
    public ElementType elementType;

    public ElementalEffectData elementalEffectData;

    public AttackData(Entity_Stats entityStats, DamageScaleData damageScaleData)
    {
        physicalDamage = entityStats.GetPhysicalDamage(out isCrit, damageScaleData.physical);
        elementaldamage = entityStats.GetElementelDamage(out elementType, damageScaleData.elemental);

        elementalEffectData = new ElementalEffectData(entityStats, damageScaleData);
    }
}
