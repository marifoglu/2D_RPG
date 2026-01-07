using UnityEngine;

public class Player_UpCombat : MonoBehaviour
{
    private Entity_VFX entityVfx;
    protected Entity_Stats entityStats;

    public DamageScaleData upAttackScale;

    [Header("Target Detection - Above Player")]
    [SerializeField] private Transform upTargetCheck;
    [SerializeField] private float upTargetCheckRadius = 1.5f;
    [SerializeField] private LayerMask whatIsTarget;

    private void Awake()
    {
        entityVfx = GetComponent<Entity_VFX>();
        entityStats = GetComponent<Entity_Stats>();
    }

    public void PerformUpAttack()
    {
        var ownerHealth = GetComponent<Entity_Health>();
        if (ownerHealth != null && ownerHealth.isDead)
            return;

        foreach (var target in GetDetectCollider())
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable == null)
                continue;

            AttackData attackData = entityStats.GetAttackData(upAttackScale);
            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

            float physicalDamage = attackData.physicalDamage;
            float elementalDamage = attackData.elementaldamage;
            ElementType elementType = attackData.elementType;

            bool targetGotHit = damageable.TakeDamage(physicalDamage, elementalDamage, elementType, transform);

            if (elementType != ElementType.None && targetGotHit)
                statusHandler?.ApplyStatusEffect(elementType, attackData.elementalEffectData);

            if (targetGotHit)
                entityVfx?.CreateOnHitVFX(target.transform, attackData.isCrit, elementType);
        }
    }

    protected Collider2D[] GetDetectCollider()
    {
        return Physics2D.OverlapCircleAll(upTargetCheck.position, upTargetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()
    {
        if (upTargetCheck == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(upTargetCheck.position, upTargetCheckRadius);
    }
}