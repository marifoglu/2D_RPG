using System;
using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    private Entity_VFX entityVfx;
    private Entity_SFX entitySfx;
    protected Entity_Stats entityStats; // Changed from private to protected
    public DamageScaleData basicAttackScale;

    public event Action<float> onDoingPhysicalDamage;

    [Header("Target Detection")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1;
    [SerializeField] private LayerMask whatIsTarget;


    private void Awake()
    {
        entityVfx = GetComponent<Entity_VFX>();
        entitySfx = GetComponent<Entity_SFX>();
        entityStats = GetComponent<Entity_Stats>();
    }

    public void PerformAttack()
    {
        bool targetGotHit = false;

        // Null check for required components
        if (entityStats == null)
        {
            Debug.LogError($"{name}: Entity_Combat cannot perform attack - Entity_Stats component is missing!");
            return;
        }

        if (targetCheck == null)
        {
            Debug.LogError($"{name}: Entity_Combat cannot perform attack - targetCheck Transform is not assigned!");
            return;
        }

        if (basicAttackScale == null)
        {
            Debug.LogError($"{name}: Entity_Combat cannot perform attack - basicAttackScale is not assigned!");
            return;
        }

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

            targetGotHit = damageable.TakeDamage(physicalDamage, elementalDamage, elementType, transform);

            if (elementType != ElementType.None)
                statusHandler?.ApplyStatusEffect(elementType, attackData.elementalEffectData);

            if (targetGotHit)
            {
                onDoingPhysicalDamage?.Invoke(physicalDamage);  
                entityVfx?.CreateOnHitVFX(target.transform, attackData.isCrit, elementType);
                entitySfx?.PlayAttackHitSFX();
            }
        }
            
        if (targetGotHit == false)
        {
            entitySfx?.PlayAttackMissSFX();
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