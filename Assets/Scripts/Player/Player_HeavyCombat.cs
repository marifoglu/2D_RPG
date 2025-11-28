using UnityEngine;

public class Player_HeavyCombat : MonoBehaviour
{
    private Entity_VFX entityVfx;
    protected Entity_Stats entityStats;

    public DamageScaleData heavyAttackScale;

    [Header("Target Detection")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1.2f;
    [SerializeField] private LayerMask whatIsTarget;

    private void Awake()
    {
        entityVfx = GetComponent<Entity_VFX>();
        entityStats = GetComponent<Entity_Stats>();
    }

    public void PerformHeavyAttack()
    {
        // If the owner died, ignore any late animation events
        var ownerHealth = GetComponent<Entity_Health>();
        if (ownerHealth != null && ownerHealth.isDead)
            return;

        foreach (var target in GetDetectCollider())
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable == null)
                continue;

            // Use heavy attack damage calculation
            float heavyDamage = entityStats.GetHeavyAttackDamage(out bool isCrit, heavyAttackScale.physical);
            float elementalDamage = entityStats.GetElementelDamage(out ElementType elementType, heavyAttackScale.elemental);

            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

            bool targetGotHit = damageable.TakeDamage(heavyDamage, elementalDamage, elementType, transform);

            if (elementType != ElementType.None && targetGotHit)
                statusHandler?.ApplyStatusEffect(elementType, new ElementalEffectData(entityStats, heavyAttackScale));

            if (targetGotHit)
                entityVfx?.CreateOnHitVFX(target.transform, isCrit, elementType);
        }
    }

    protected Collider2D[] GetDetectCollider()
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()
    {
        if (targetCheck == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}