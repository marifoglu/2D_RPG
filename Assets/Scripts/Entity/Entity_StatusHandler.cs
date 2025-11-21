using System.Collections;
using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private Entity entity;
    private Entity_VFX entityVFX;
    private Entity_Stats entityStats;
    private Entity_Health entityHealth;
    private ElementType currentEffect = ElementType.None;

    [Header("Lightning Effect Settings")]
    [SerializeField] private GameObject lightingStrikeVfx;
    [SerializeField] private float currentCharge;
    [SerializeField] private float maximumCharge = 1;
    private Coroutine lightningCo;
    
    private void Awake()
    {
        entity = GetComponent<Entity>();
        entityVFX = GetComponent<Entity_VFX>();
        entityStats = GetComponent<Entity_Stats>();
        entityHealth = GetComponent<Entity_Health>();
    }
    public void ApplyStatusEffect(ElementType element, ElementalEffectData effectData)
    {
        if(element == ElementType.Ice && CanBeApplied(ElementType.Ice))
            ApplyChillEffect(effectData.chillDuration, effectData.chillSlowMultiplier);

        if(element == ElementType.Fire && CanBeApplied(ElementType.Fire))
            ApplyBurnEffect(effectData.burnDuration, effectData.burnDamage);

        if(element == ElementType.Lighting && CanBeApplied(ElementType.Lighting))
            ApplyLightingEffect(effectData.lightningDuration, effectData.lightningDamage, effectData.lightningCharge);

    }
    public void ApplyLightingEffect(float duration, float damage, float charge)
    {

        float lightningResistance = entityStats.GetElementalResistance(ElementType.Lighting);
        float finalCharge = charge * (1 - lightningResistance);
        currentCharge += finalCharge;

        if (currentCharge >= maximumCharge)
        {
            DoLightingStrike(damage);
            StopLightningEffect();
            return;
        }

        if(lightningCo != null)
            StopCoroutine(lightningCo);
        lightningCo = StartCoroutine(LightningEffectCo(duration));
    }

    private void DoLightingStrike(float damage)
    {
        Instantiate(lightingStrikeVfx, transform.position, Quaternion.identity);
        entityHealth.ReduceHealth(damage);
    }


    private void StopLightningEffect()
    {
        currentEffect = ElementType.None;
        currentCharge = 0;
        entityVFX.StopAllVFX();
    }

    private IEnumerator LightningEffectCo(float duration)
    {
        currentEffect = ElementType.Lighting;
        entityVFX.PlayStatusVfx(duration, ElementType.Lighting);

        yield return new WaitForSeconds(duration);

        StopLightningEffect();
    }
    public void ApplyBurnEffect(float duration, float fireDamage)
    {
        float fireResistance = entityStats.GetElementalResistance(ElementType.Fire);  
        float finalDamage = fireDamage  * (1 - fireResistance);

        StartCoroutine(BurnEffectCo(duration, finalDamage));
    }
    private IEnumerator BurnEffectCo(float durattion, float totalDamage)
    {
        currentEffect = ElementType.Fire;
        entityVFX.PlayStatusVfx(durattion, ElementType.Fire);

        int ticksPerSecond = 2;
        int tickCount = Mathf.RoundToInt(ticksPerSecond * durattion);

        float damagePerTick = totalDamage / tickCount;
        float tickInterval = 1f / ticksPerSecond;

        for (int i = 0; i < tickCount; i++)
        {
            entityHealth.ReduceHealth(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
        }

        currentEffect = ElementType.None;

    }


    public void ApplyChillEffect(float duration, float slowMultiplier)
    {
        float iceResistance = entityStats.GetElementalResistance(ElementType.Ice);  
        float finalDuration = duration * (1 - iceResistance);

        StartCoroutine(ChillEffectCo(finalDuration, slowMultiplier));
    }

    private IEnumerator ChillEffectCo(float duration, float slowMultiplier)
    {
        entity.SlowDownEntity(duration, slowMultiplier);
        currentEffect = ElementType.Ice;
        entityVFX.PlayStatusVfx(duration, ElementType.Ice);
        
        yield return new WaitForSeconds(duration);
        currentEffect = ElementType.None;
    }
    public bool CanBeApplied(ElementType element)
    {
        if(currentEffect == ElementType.Lighting && element == ElementType.Lighting)
            return true;


        return currentEffect == ElementType.None;
    }
}
