using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    private Entity_VFX entityVfx;
    protected Entity_Stats entityStats; // Changed from private to protected

    public float damage = 10f;

    [Header("Target Detection")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1;
    [SerializeField] private LayerMask whatIsTarget;

    [Header("Status Effect Details")]
    [SerializeField] private float defaultDuration = 3f;
    [SerializeField] private float chillSlowMultiplier = .2f;
     
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

            float elementalDamage = entityStats.GetElementelDamage(out ElementType elementType);
            float damage = entityStats.GetPhysicalDamage(out bool isCrit);

            bool targetGotHit = damageable.TakeDamage(damage, elementalDamage, elementType, transform);

            if(elementType != ElementType.None) { 
                 ApplyStatusEffect(target.transform, elementType);
            }

            if (targetGotHit)
            {
                entityVfx.UpdateOnHitColor(elementType);
                entityVfx?.CreateOnHitVFX(target.transform, isCrit);
            }
        }
    }

    public void ApplyStatusEffect(Transform target, ElementType elementType)
    {
        Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

        if (statusHandler == null)
            return;

        if (elementType == ElementType.Ice && statusHandler.CanBeApplied(ElementType.Ice))
            statusHandler.AppliedChilledEffect(defaultDuration, chillSlowMultiplier);
    }

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