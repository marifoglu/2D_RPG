using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy; // Cached reference

    private void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    public override bool TakeDamage(float damage, Transform damageDealer)
    {
        // Enemy can't be interrupted, ignore damage
        if (enemy != null && enemy.IsInUninterruptibleState)
            return false;

        bool wasHit = base.TakeDamage(damage, damageDealer);

        if (!wasHit)
            return false;

        if (damageDealer.GetComponent<Player>() != null)
            enemy.TryEnterBattleState(damageDealer);

        return true;
    }

}



//using UnityEngine;

//public class Enemy_Health : Entity_Health
//{
//    private Enemy enemy => GetComponent<Enemy>();



//    public override bool TakeDamage(float damage, Transform damageDealer)
//    {
//        bool wasHit = (enemy != null) && (enemy.IsInUninterruptibleState);

//        base.TakeDamage(damage, damageDealer);

//        if (isDead || wasHit)
//            return false;

//        if (enemy != null && damageDealer.GetComponent<Player>() != null)
//            enemy.TryEnterBattleState(damageDealer);

//        return true;
//    }

//}