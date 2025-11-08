using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    private Entity_VFX entityVfx;
    private Entity_Stats entityStats;

    public float damage = 10f;

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

        // Hard block: if this combat belongs to an enemy and it’s unsafe (ledge/wall), do not attack.
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
            if (damageable == null) continue;

            float damage = entityStats.GetPhysicalDamage(out bool isCrit);
            float elementalDamage = entityStats.GetElementelDamage(out ElementType elementType);

            bool targetGotHit = damageable.TakeDamage(damage, elementalDamage,elementType, transform);
            if (targetGotHit)
            {
                entityVfx?.CreateOnHitVFX(target.transform, isCrit);
            }
        }
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