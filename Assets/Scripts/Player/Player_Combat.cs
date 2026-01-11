using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [Header("Counter Attack Settings")]
    [SerializeField] private float counterRecovery = .1f;
    [SerializeField] private LayerMask whatIsCounterable;

    public bool CounterAttackPerformed()
    {
        bool hasPerformedCounter = false;

        Collider2D[] targets = GetDetectColliders(whatIsCounterable);

        foreach (var target in targets)
        {
            ICounterable counterable = target.GetComponent<ICounterable>();

            if (counterable == null)
                continue;

            IDamageable damageable = target.GetComponent<IDamageable>();
            float elementalDamage = 0f;

            if (damageable != null)
            {
                float counterAttackDamage = entityStats.GetCounterAttackDamage();

                bool damageApplied = damageable.TakeDamage(
                    counterAttackDamage,
                    elementalDamage,
                    ElementType.None,
                    transform
                );

                if (damageApplied)
                {
                    hasPerformedCounter = true;

                    Entity_Health targetHealth = target.GetComponent<Entity_Health>();
                    if (targetHealth != null && !targetHealth.isDead)
                    {
                        counterable.HandleCounter();
                    }
                }
            }
        }

        return hasPerformedCounter;
    }
    public float GetCounterRecoveryDuration() => counterRecovery;
}