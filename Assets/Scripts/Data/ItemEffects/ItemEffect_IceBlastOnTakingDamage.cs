using UnityEngine;

[CreateAssetMenu(fileName = "Item effect data - Ice blast on taking damage", menuName = "RPG Setup/Item data/Item effect/Ice blast")]
public class ItemEffect_IceBlastOnTakingDamage : ItemEffectDataSO
{
    [SerializeField] private ElementalEffectData elementalEffectData;
    [SerializeField] private float IceDamage;
    [SerializeField] private LayerMask whatIsEnemy;

    [Space]
    [SerializeField] private float healthPercentTrigger = .3f;
    [SerializeField] private float cooldownDuration;
    private float lastTriggerTime = -999f;
    [Header("VFX effect")]
    [SerializeField] private GameObject iceBlastVFX;
    [SerializeField] private GameObject onHitVfx;
    
    override public void ExecuteEffect()
    {
        bool noCooldown = Time.time >= lastTriggerTime + cooldownDuration;
        bool reachedThreashold = player.health.GetHealthPercentage() <= healthPercentTrigger;

        if(noCooldown && reachedThreashold)
        {
            // play vfx
            player.vfx.CreateEffectOf(iceBlastVFX, player.transform);
            lastTriggerTime = Time.time;

            DamageEnemiesWithIce();
        }
    }

    private void DamageEnemiesWithIce()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.transform.position, 1.5f, whatIsEnemy);

        foreach(var target in enemies)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            
            if (damageable != null) continue;

            bool targetGotHit = damageable.TakeDamage(0, IceDamage, ElementType.Ice, player.transform);

            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();
            statusHandler?.ApplyStatusEffect(ElementType.Ice, elementalEffectData);

            if(targetGotHit)
            {
                // play on hit vfx
                player.vfx.CreateEffectOf(onHitVfx, target.transform);
            }
        }
    }
    override public void Subscribe(Player player)
    {
        base.Subscribe(player);
        player.health.OnTakingDamage += ExecuteEffect;
    }

    override public void Unsubscribe()
    {
        base.Unsubscribe();
        player.health.OnTakingDamage -= ExecuteEffect;
        player = null;
    } 
}
