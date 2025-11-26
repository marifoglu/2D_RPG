using UnityEngine;

public class SkillObject_SwordSpin : SkillObject_Sword
{
    private int maxDistance;
    private float attackPerSecond;
    private float attackTimer;

    public override void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        base.SetupSword(swordManager, direction);

        anim.SetTrigger("Spin");

        maxDistance = swordManager.maxDistance;
        attackPerSecond = swordManager.attackPerSecond;

        // fallback: force return after maxSpinDuration
        Invoke(nameof(GetSwordBackToPlayer), swordManager.maxSpinDuration);

        // initialize timer so the first attack happens immediately
        attackTimer = 0f;
    }

    protected override void Update()
    {
        // Don't execute until the sword is properly initialized
        if (rb == null || playerTransform == null)
            return;

        HandleAttack();
        HandleStopping();
        HandleComeBack();
    }

    private void HandleAttack()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            DamageEnemiesInRadius(transform, 1f);
            attackTimer = 1f / Mathf.Max(0.0001f, attackPerSecond);
        }
    }

    private void HandleStopping()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > maxDistance && rb.simulated == true)
            rb.simulated = false;
    }

    // Hit-first-enemy behavior: when hitting an enemy, immediately damage then return to player
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (shouldCombeBack)
            return;

        Enemy enemy = collision.GetComponent<Enemy>();
        bool isGround = collision.gameObject.layer == LayerMask.NameToLayer("Ground");

        if (enemy != null)
        {
            // damage once and return
            DamageEnemiesInRadius(transform, 1f);
            GetSwordBackToPlayer();
            return;
        }

        if (isGround)
        {
            // stick to ground like base behavior
            StopSword(collision);
            return;
        }

        // otherwise ignore and keep spinning
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (shouldCombeBack)
            return;

        Collider2D col = collision.collider;
        Enemy enemy = col.GetComponent<Enemy>();
        bool isGround = col.gameObject.layer == LayerMask.NameToLayer("Ground");

        if (enemy != null)
        {
            DamageEnemiesInRadius(transform, 1f);
            GetSwordBackToPlayer();
            return;
        }

        if (isGround)
        {
            StopSword(col);
            return;
        }
    }
}