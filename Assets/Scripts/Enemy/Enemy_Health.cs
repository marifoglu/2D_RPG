using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy;

    private void Start()  // ← Changed from Awake to Start
    {
        enemy = GetComponent<Enemy>();
    }

    public override void TakeDamage(float damage, Transform damageDealer)
    {
        // Check states BEFORE applying damage
        bool wasInUninterruptibleState = enemy != null && enemy.IsInUninterruptibleState;
        
        base.TakeDamage(damage, damageDealer);

        // Don't change states if dead or if was already in uninterruptible state
        if (isDead || wasInUninterruptibleState)
            return;

        if (enemy != null && damageDealer.GetComponent<Player>() != null)
            enemy.TryEnterBattleState(damageDealer);
    }
}