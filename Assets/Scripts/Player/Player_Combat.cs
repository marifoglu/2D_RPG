using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [Header("Counter Attack Settings")]
    [SerializeField] private float counterRecovery = .1f;
    [SerializeField] private float counterAttackDamage = 20f;

    public bool CounterAttackPerformed()
    {
        bool hasPerformedCounter = false;

        foreach (var target in GetDetectCollider())
        {
            ICounterable counterable = target.GetComponent<ICounterable>();

            if (counterable == null || !counterable.CanBeCountered)
                continue; // if not counterable, skip

            IDamageable damageable = target.GetComponent<IDamageable>();
            float elementalDamage = 0f; // Noelemental damage for counter-attack

            // CRITICAL: Apply damage BEFORE handling counter state
            if (damageable != null)
            {
                bool damageApplied = damageable.TakeDamage(
                    counterAttackDamage,
                    elementalDamage,
                    ElementType.None,
                    transform
                );

                if (damageApplied)
                {
                    hasPerformedCounter = true;
                }
            }

            // Handle counter state change AFTER damage is applied
            counterable.HandleCounter();
        }

        return hasPerformedCounter;
    }

    public float GetCounterRecoveryDuration() => counterRecovery;
}