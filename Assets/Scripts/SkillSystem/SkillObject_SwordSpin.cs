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

        Invoke(nameof(GetSwordBackToPlayer), swordManager.maxSpinDuration);
    }
    protected override void Update()
    {
        // Don't execute until the sword is properly initialized
        if (rb == null || playerTransform == null)
            return;

        HandleAttack();
        HandleStopping();
        HandleComeback();
    }
    private void HandleAttack()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            DamageEnemiesInRadius(transform, 1f);
            attackTimer = 1f / attackPerSecond;
        }
    }
    private void HandleStopping()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > maxDistance && rb.simulated == true)
            rb.simulated = false;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        rb.simulated = false;
    }
}
