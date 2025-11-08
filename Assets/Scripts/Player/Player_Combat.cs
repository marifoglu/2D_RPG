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
            IDamageable damageable = target.GetComponent<IDamageable>();
            float elementalDamage = 0f; // Assuming no elemental damage for counter-attack

            if (counterable == null)
                continue; // if not counterable, skip

            if (counterable.CanBeCountered)
            {
                counterable.HandleCounter();

                if (damageable != null)
                {
                    damageable.TakeDamage(counterAttackDamage, elementalDamage, ElementType.None, transform);
                }
                hasPerformedCounter = true;
            }

        }
        return hasPerformedCounter;
    }

    public float GetCounterRecoveryDuration() => counterRecovery;
}

