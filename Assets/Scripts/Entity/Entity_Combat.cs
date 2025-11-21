using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    private Entity_VFX entityVfx;
    protected Entity_Stats entityStats; // Changed from private to protected

    public DamageScaleData basicAttackScale;

    [Header("Target Detection")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1;
    [SerializeField] private LayerMask whatIsTarget;

 
    private void Awake()
    {
        entityVfx = GetComponent<Entity_VFX>();
        entityStats = GetComponent<Entity_Stats>();
    }

    public void PerformAttack()
    {
        // If the owner died, ignore any late animation events
        var ownerHealth = GetComponent<Entity_Health>();
        if (ownerHealth != null && ownerHealth.isDead)
            return;

        // Hard block: if combat belongs to an enemy and it's unsafe (ledge-wall), do not attack.
        var ownerEnemy = GetComponent<Enemy>();
        if (ownerEnemy != null)
        {
            if (ownerEnemy.edgeDetected || ownerEnemy.wallDetected)
                return;

            if (ownerEnemy.IsPlayerDead)
                return;
        }

        foreach (var target in GetDetectCollider())
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable == null) 
                continue;

            AttackData attackData = entityStats.GetAttackData(basicAttackScale);
            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

            float physicalDamage = attackData.physicalDamage;
            float elementalDamage = attackData.elementaldamage;
            ElementType elementType = attackData.elementType;

            bool targetGotHit = damageable.TakeDamage(physicalDamage, elementalDamage, elementType, transform);

            if (elementType != ElementType.None)
                statusHandler?.ApplyStatusEffect(elementType, attackData.elementalEffectData);

            if (targetGotHit)
                entityVfx?.CreateOnHitVFX(target.transform, attackData.isCrit, elementType); 
        }
    }






    //public void PerformAttack()
    //{
    //    // If the owner died, ignore any late animation events
    //    var ownerHealth = GetComponent<Entity_Health>();
    //    if (ownerHealth != null && ownerHealth.isDead)
    //        return;

    //    // Hard block: if combat belongs to an enemy and it's unsafe (ledge-wall), do not attack.
    //    var ownerEnemy = GetComponent<Enemy>();
    //    if (ownerEnemy != null)
    //    {
    //        if (ownerEnemy.edgeDetected || ownerEnemy.wallDetected)
    //            return;

    //        if (ownerEnemy.IsPlayerDead)
    //            return;
    //    }

    //    foreach (var target in GetDetectCollider())
    //    {
    //        IDamageable damageable = target.GetComponent<IDamageable>();
    //        if (damageable == null)
    //            continue;

    //        ElementalEffectData effectData = new ElementalEffectData(entityStats, basicAttackScale);


    //        float elementalDamage = entityStats.GetElementelDamage(out ElementType elementType, 0.6f);
    //        float damage = entityStats.GetPhysicalDamage(out bool isCrit);

    //        bool targetGotHit = damageable.TakeDamage(damage, elementalDamage, elementType, transform);

    //        if (elementType != ElementType.None)
    //            target.GetComponent<Entity_StatusHandler>().ApplyStatusEffect(elementType, effectData);

    //        if (targetGotHit)
    //            entityVfx?.CreateOnHitVFX(target.transform, isCrit, elementType);
    //    }
    //}



    protected Collider2D[] GetDetectCollider()
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}