using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    public override bool TakeDamage(float damage, float elementalDamage,ElementType elementType, Transform damageDealer)
    {
        // Enemy can't be interrupted, ignore damage
        if (enemy != null && enemy.IsInUninterruptibleState)
            return false;

        bool wasHit = base.TakeDamage(damage, elementalDamage,elementType, damageDealer);

        if (!wasHit)
            return false;

        if (damageDealer.GetComponent<Player>() != null)
            enemy.TryEnterBattleState(damageDealer);

        return true;
    }

}