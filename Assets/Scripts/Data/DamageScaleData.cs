using System;
using UnityEngine;

[Serializable]
public class DamageScaleData
{
    [Header("Damage")]
    public float physical = 1f;
    public float elemental = 1f;

    [Header("Chill")]
    public float chillDuration = 3f;
    public float chillSlowMultiplier = 0.2f;

    [Header("Burn")]
    public float burnDuration = 3f;
    public float burnDamageScale = 1f;

    [Header("Shock")]
    public float lightningDuration = 3f;
    public float lightningDamageScale = 1f;
    public float lightningCharge = 1f;



}
