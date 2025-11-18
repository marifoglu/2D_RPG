using UnityEngine;

public class Skill_ObjectBase : MonoBehaviour
{
    [SerializeField] protected LayerMask whatIsEnemy;
    [SerializeField] protected Transform targetCheck;
    [SerializeField] protected float checkRadius = 1f;

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
            
                damageable.TakeDamage(10, 0, ElementType.None, transform); // Example damage values
            
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
