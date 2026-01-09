using UnityEngine;

public class Skill_Backstab : Skill_Base
{
    [Header("Backstab Settings")]
    [SerializeField] private float maxTargetDistance = 10f;
    [SerializeField] private float teleportOffsetBehindEnemy = 1.2f;
    [SerializeField] private float stunDuration = 1.5f;
    [SerializeField] private Vector2 stunKnockback = new Vector2(2f, 1f);

    [Header("Damage Settings")]
    [SerializeField] private float backstabDamageMultiplier = 2.5f;
    public DamageScaleData backstabDamageScale;

    [Header("VFX")]
    [SerializeField] private GameObject disappearVFX;
    [SerializeField] private GameObject appearVFX;
    [SerializeField] private GameObject backstabHitVFX;

    [Header("Detection")]
    [SerializeField] private LayerMask enemyLayer;

    private Transform currentTarget;
    private Vector3 teleportDestination;

    // Uses normal cooldown from base class - no override needed

    public override void TryUseSkill()
    {
        if (!CanUseSkill())
            return;

        Transform target = FindClosestEnemy();

        if (target == null)
        {
            Debug.Log("No enemy in range for backstab!");
            return;
        }

        currentTarget = target;
        teleportDestination = CalculateTeleportPosition(target);

        // Enter backstab state
        player.StateMachine.ChangeState(player.backstabState);
        SetSkillOnCooldown();
    }

    public Transform FindClosestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.transform.position, maxTargetDistance, enemyLayer);

        if (enemies.Length == 0)
            return null;

        Transform closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (var enemyCollider in enemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();

            if (enemy == null || enemy.enemyHealth.isDead)
                continue;

            float distance = Vector2.Distance(player.transform.position, enemyCollider.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = enemyCollider.transform;
            }
        }

        return closest;
    }

    private Vector3 CalculateTeleportPosition(Transform target)
    {
        Enemy enemy = target.GetComponent<Enemy>();
        int enemyFacingDir = enemy != null ? enemy.facingDir : 1;

        float offsetX = -enemyFacingDir * teleportOffsetBehindEnemy;
        Vector3 behindPosition = target.position + new Vector3(offsetX, 0, 0);

        RaycastHit2D groundCheck = Physics2D.Raycast(behindPosition + Vector3.up, Vector2.down, 3f, player.whatIsGround);

        if (groundCheck.collider != null)
        {
            behindPosition.y = groundCheck.point.y + 0.1f;
        }

        return behindPosition;
    }

    public void ExecuteTeleport()
    {
        if (currentTarget == null)
            return;

        if (disappearVFX != null)
            Instantiate(disappearVFX, player.transform.position, Quaternion.identity);

        player.TeleportPlayer(teleportDestination);

        if (appearVFX != null)
            Instantiate(appearVFX, player.transform.position, Quaternion.identity);

        FaceTarget();
    }

    private void FaceTarget()
    {
        if (currentTarget == null)
            return;

        float directionToTarget = currentTarget.position.x - player.transform.position.x;

        if ((directionToTarget > 0 && player.facingDir < 0) || (directionToTarget < 0 && player.facingDir > 0))
            player.Flip();
    }

    public void ExecuteBackstabAttack()
    {
        if (currentTarget == null)
            return;

        Enemy enemy = currentTarget.GetComponent<Enemy>();
        IDamageable damageable = currentTarget.GetComponent<IDamageable>();

        if (damageable == null)
            return;

        DamageScaleData scaleToUse = backstabDamageScale ?? damageScaleData;
        float physicalDamage = player.stats.GetPhysicalDamage(out bool isCrit, scaleToUse.physical);
        float elementalDamage = player.stats.GetElementalDamage(out ElementType elementType, scaleToUse.elemental);

        physicalDamage *= backstabDamageMultiplier;

        bool hitLanded = damageable.TakeDamage(physicalDamage, elementalDamage, elementType, player.transform);

        if (hitLanded)
        {
            if (backstabHitVFX != null)
                Instantiate(backstabHitVFX, currentTarget.position, Quaternion.identity);

            Entity_StatusHandler statusHandler = currentTarget.GetComponent<Entity_StatusHandler>();
            if (elementType != ElementType.None && statusHandler != null)
            {
                ElementalEffectData effectData = new ElementalEffectData(player.stats, scaleToUse);
                statusHandler.ApplyStatusEffect(elementType, effectData);
            }

            if (enemy != null && !enemy.enemyHealth.isDead)
                ApplyStunToEnemy(enemy);

            player.vfx.CreateOnHitVFX(currentTarget, isCrit, elementType);
        }
    }

    private void ApplyStunToEnemy(Enemy enemy)
    {
        if (enemy.stunnedState != null)
        {
            Vector2 knockback = new Vector2(stunKnockback.x * player.facingDir, stunKnockback.y);
            enemy.ReceiveKnockback(knockback, 0.1f);

            float originalStunDuration = enemy.stunnedDuration;
            enemy.stunnedDuration = stunDuration;

            enemy.stateMachine.ChangeState(enemy.stunnedState);

            enemy.StartCoroutine(ResetStunDuration(enemy, originalStunDuration));
        }
    }

    private System.Collections.IEnumerator ResetStunDuration(Enemy enemy, float originalDuration)
    {
        yield return null;
        enemy.stunnedDuration = originalDuration;
    }

    public void ClearTarget()
    {
        currentTarget = null;
    }

    public Transform GetCurrentTarget() => currentTarget;
    public Vector3 GetTeleportDestination() => teleportDestination;
    public float GetStunDuration() => stunDuration;

    private void OnDrawGizmosSelected()
    {
        if (player == null)
            return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(player.transform.position, maxTargetDistance);
    }
}