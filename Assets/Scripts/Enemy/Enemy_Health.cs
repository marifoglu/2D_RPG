using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    public override bool TakeDamage(float damage, Transform damageDealer)
    {
        bool wasHit = enemy != null && enemy.IsInUninterruptibleState;
        
        base.TakeDamage(damage, damageDealer);

        if (isDead || wasHit)
            return false;

        if (enemy != null && damageDealer.GetComponent<Player>() != null)
            enemy.TryEnterBattleState(damageDealer);

        return true;
    }

}