using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    public override void TakeDamage(float damage, Transform damageDealer)
    {
        bool wasInUninterruptibleState = enemy != null && enemy.IsInUninterruptibleState;
        
        base.TakeDamage(damage, damageDealer);

        if (isDead || wasInUninterruptibleState)
            return;

        if (enemy != null && damageDealer.GetComponent<Player>() != null)
            enemy.TryEnterBattleState(damageDealer);
    }
}