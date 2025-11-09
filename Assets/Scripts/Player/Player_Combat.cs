using UnityEditor;
using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [Header("Counter Attack Settings")]
    [SerializeField] private float counterRecovery = .1f;

    public bool CounterAttackPerformed()
    {
        bool hasPerformedCounter = false;

        foreach (var target in GetDetectCollider())
        {
            ICounterable counterable = target.GetComponent<ICounterable>();

            if (counterable == null || !counterable.CanBeCountered)
                continue; // if not counterable, skip

            IDamageable damageable = target.GetComponent<IDamageable>();
            float elementalDamage = 0f; // No elemental damage for counter-attack

            if (damageable != null)
            {
                float counterAttackDamage = entityStats.GetCounterAttackDamage(); // Dynamic stat

                bool damageApplied = damageable.TakeDamage(
                    counterAttackDamage,
                    elementalDamage,
                    ElementType.None,
                    transform
                );

                if (damageApplied)
                {
                    hasPerformedCounter = true;

                    // Only apply knockback and counter effects if enemy didn't die
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