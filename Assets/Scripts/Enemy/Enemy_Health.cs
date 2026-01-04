//using UnityEngine;

//public class Enemy_Health : Entity_Health
//{
//    private Enemy enemy;
//    private Player_QuestManager questManager;

//    protected override void Start()
//    {
//        base.Start();

//        enemy = GetComponent<Enemy>();
//        questManager = Player.instance.questManager;
//    } 


//    public override bool TakeDamage(float damage, float elementalDamage,ElementType elementType, Transform damageDealer)
//    {
//        if(canTakeDamage == false)
//            return false;

//        // Enemy can't be interrupted, ignore damage
//        if (enemy != null && enemy.IsInUninterruptibleState)
//            return false;

//        bool wasHit = base.TakeDamage(damage, elementalDamage,elementType, damageDealer);

//        if (!wasHit)
//            return false;

//        if (damageDealer.GetComponent<Player>() != null)
//            enemy.TryEnterBattleState(damageDealer);

//        return true;
//    }

//    protected override void Die()
//    {
//        base.Die();

//        questManager.AddProgress(enemy.questTargetID); 
//    }

//}
using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy;
    private Player_QuestManager questManager;
    private Player_ExperienceManager expManager;

    protected override void Start()
    {
        base.Start();

        enemy = GetComponent<Enemy>();

        Player player = Player.instance;
        if (player != null)
        {
            questManager = player.questManager;
            expManager = player.GetComponent<Player_ExperienceManager>();
        }
    }


    public override bool TakeDamage(float damage, float elementalDamage, ElementType elementType, Transform damageDealer)
    {
        if (canTakeDamage == false)
            return false;

        // Enemy can't be interrupted, ignore damage
        if (enemy != null && enemy.IsInUninterruptibleState)
            return false;

        bool wasHit = base.TakeDamage(damage, elementalDamage, elementType, damageDealer);

        if (!wasHit)
            return false;

        if (damageDealer.GetComponent<Player>() != null)
            enemy.TryEnterBattleState(damageDealer);

        return true;
    }

    protected override void Die()
    {
        base.Die();

        // Grant quest progress
        if (questManager != null && enemy != null)
        {
            questManager.AddProgress(enemy.questTargetID);
        }

        // Grant experience based on enemy rarity
        if (expManager != null && enemy != null)
        {
            expManager.AddExperienceFromEnemy(enemy.enemyRarity);
        }
    }

}