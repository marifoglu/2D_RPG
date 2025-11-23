using UnityEngine;

public class Skill_ObjectBase : MonoBehaviour
{
    [SerializeField] private GameObject onHitVfx;
    [Space]
    [SerializeField] protected LayerMask whatIsEnemy;
    [SerializeField] protected Transform targetCheck;
    [SerializeField] protected float checkRadius = 1f;

    protected Animator anim;
    protected Entity_Stats entityStats;
    protected DamageScaleData damageScaleData;
    protected ElementType usedElement;
    protected bool targetGotHit;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }
    protected void DamageEnemiesInRadius(Transform transform, float radius)
    {
        foreach(var target in GetEnemiesAround(transform, radius))
        {
            IDamageable damageable = target.GetComponent<IDamageable>();

            if (damageable == null)
                continue;

            AttackData attackData = entityStats.GetAttackData(damageScaleData);  
            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

            float physicalDamage = attackData.physicalDamage;
            float elementalDamage = attackData.elementaldamage;
            ElementType elementType = attackData.elementType;


            targetGotHit = damageable.TakeDamage(physicalDamage, elementalDamage, elementType, transform);

            if (elementType != ElementType.None)
                statusHandler?.ApplyStatusEffect(elementType, attackData.elementalEffectData);

            if(targetGotHit)
                Instantiate(onHitVfx, target.transform.position, Quaternion.identity);

            usedElement = elementType;
        }
    }

    protected Transform FindClosestTarget()
    {
        Transform target = null;
        float closestDistance = Mathf.Infinity;

        foreach(var enemy in GetEnemiesAround(transform, 10))
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                target = enemy.transform;
                closestDistance = distance;
            }
        }
        return target;
    }

    protected Collider2D[] GetEnemiesAround(Transform transform, float radius)
    {
        return Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy);
    }
 
    protected virtual void OnDrawGizmos()
    {
        if (targetCheck == null)
            targetCheck = transform;

        Gizmos.color = Color.darkOrange;
        Gizmos.DrawWireSphere(targetCheck.position, checkRadius);
    }
}
