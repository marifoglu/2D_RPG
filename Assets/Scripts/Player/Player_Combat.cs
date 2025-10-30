using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [Header("Counter Attack Settings")]
    [SerializeField] private float counterRecovery = .1f;
    [SerializeField] private float counterAttackDamage = 20f;
    public bool CounterAttackPerformed()
    {
        bool hasPerformedCounter = false;

        foreach (var targer in GetDetectCollider())
        {
            ICounterable counterable = targer.GetComponent<ICounterable>();
            IDamageable damageable = targer.GetComponent<IDamageable>();


            if (counterable == null) 
                continue; // if not counterable, skip

            if (counterable.CanBeCountered) 
            {
                counterable.HandleCounter();

                if (damageable != null)
                {
                    damageable.TakeDamage(counterAttackDamage, transform);
                }
                hasPerformedCounter = true;
            }

        }
        return hasPerformedCounter;
    }

    public float GetCounterRecoveryDuration() => counterRecovery;
}
