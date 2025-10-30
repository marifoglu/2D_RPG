using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [Header("Counter Attack Settings")]
    [SerializeField] private float counterRecovery = .1f;
    public bool CounterAttackPerformed()
    {
        bool hasPerformedCounter = false;

        foreach (var targer in GetDetectCollider())
        {
            ICounterable counterable = targer.GetComponent<ICounterable>();


            if (counterable != null) 
                continue; // if not counterable, skip

            if (counterable.CanBeCountered) 
            {
                counterable.HandleCounter();
                hasPerformedCounter = true;
            }

        }
        return hasPerformedCounter;
    }

    public float GetCounterRecoveryDuration() => counterRecovery;
}
