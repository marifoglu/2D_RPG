using System;

public class ElementalEffectData
{
    public float chillSlowMultiplier;
    public float chillDuration;

    public float burnDamage;
    public float burnDuration;

    public float lightningDamage;
    public float lightningDuration;
    public float lightningCharge;

    public ElementalEffectData(Entity_Stats entityStats, DamageScaleData damageScale)
    {
        chillSlowMultiplier = damageScale.chillSlowMultiplier;
        chillDuration = damageScale.chillDuration;

        burnDamage = entityStats.offense.fireDamage.GetValue() * damageScale.burnDamageScale;
        burnDuration = damageScale.burnDuration;

        lightningDamage = entityStats.offense.lightningDamage.GetValue() * damageScale.lightningDamageScale;
        lightningDuration = damageScale.lightningDuration;
        lightningCharge = damageScale.lightningCharge;




    }





}