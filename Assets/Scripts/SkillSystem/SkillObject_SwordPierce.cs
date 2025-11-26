using UnityEngine;


public class SkillObject_SwordPierce : SkillObject_Sword
{
    private int amountToPierce;
    private Collider2D col;

    public override void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        base.SetupSword(swordManager, direction);
        amountToPierce = swordManager.pierceAmount;

        // Force pierce sword to use trigger collisions so physics won't stop it
        col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    // Trigger case (pierce uses trigger so this is primary)
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // If the sword is already returning, ignore
        if (shouldCombeBack)
            return;

        bool groundHit = collision.gameObject.layer == LayerMask.NameToLayer("Ground");
        Enemy enemy = collision.GetComponent<Enemy>();

        // Damage any enemy hit
        DamageEnemiesInRadius(transform, .3f);

        // If hit ground -> stop
        if (groundHit)
        {
            // ensure collider is non-trigger for correct sticking behavior
            if (col != null) col.isTrigger = false;
            StopSword(collision);
            return;
        }

        // If this collision was an enemy
        if (enemy != null)
        {
            if (amountToPierce <= 0)
            {
                if (col != null) col.isTrigger = false;
                StopSword(collision);
                return;
            }

            // consume one pierce and continue flying through
            amountToPierce--;
            return;
        }

        // Non-enemy, non-ground objects - stop for safety
        if (amountToPierce <= 0)
        {
            if (col != null) col.isTrigger = false;
            StopSword(collision);
        }
    }

    // Collision case (in case prefab uses non-trigger collider)
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        // If the sword is already returning, ignore
        if (shouldCombeBack)
            return;

        Collider2D colHit = collision.collider;
        bool groundHit = colHit.gameObject.layer == LayerMask.NameToLayer("Ground");
        Enemy enemy = colHit.GetComponent<Enemy>();

        // If using collisions, treat similarly to trigger flow:
        DamageEnemiesInRadius(transform, .3f);

        if (groundHit)
        {
            StopSword(colHit);
            return;
        }

        if (enemy != null)
        {
            if (amountToPierce <= 0)
            {
                StopSword(colHit);
                return;
            }

            amountToPierce--;
            // Important: disable physics response between sword and this enemy so sword won't be stopped/pushed
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), colHit, true);
            return;
        }

        // Non-enemy fallback
        if (amountToPierce <= 0)
            StopSword(colHit);
    }
}