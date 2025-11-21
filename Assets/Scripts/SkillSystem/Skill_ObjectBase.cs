using UnityEngine;

public class Skill_ObjectBase : MonoBehaviour
{
    [SerializeField] protected LayerMask whatIsEnemy;
    [SerializeField] protected Transform targetCheck;
    [SerializeField] protected float checkRadius = 1f;

    protected Entity_Stats entityStats;
    protected DamageScaleData damageScaleData;

    protected Collider2D[] EnemiesAround(Transform transform, float radius)
    {
        return Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy);
    }

    protected void DamageEnemiesInRadius(Transform transform, float radius)
    {
        foreach(var target in EnemiesAround(transform, radius))
        {
            IDamageable damageable = target.GetComponent<IDamageable>();

            if (damageable == null)
                continue;

            ElementalEffectData elementalEffectData = new ElementalEffectData(entityStats, damageScaleData);

            float physicalDamage = entityStats.GetPhysicalDamage(out bool isCrit, damageScaleData.physical);
            float elementalDamage = entityStats.GetElementelDamage(out ElementType element, damageScaleData.elemental);

            damageable.TakeDamage(physicalDamage, elementalDamage, element, transform); // Example damage values

            if (element != ElementType.None)
                target.GetComponent<Entity_StatusHandler>().ApplyStatusEffect(element, elementalEffectData);

        }
    }
    protected Transform FindClosestTarget()
    {
        Transform target = null;
        float closestDistance = Mathf.Infinity;

        foreach(var enemy in EnemiesAround(transform, 10))
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

    protected virtual void OnDrawGizmos()
    {
        if (targetCheck == null)
            targetCheck = transform;

        Gizmos.color = Color.darkOrange;
        Gizmos.DrawWireSphere(targetCheck.position, checkRadius);
    }
}
