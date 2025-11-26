using UnityEngine;

public class SkillObject_Sword : SkillObject_Base
{
    protected Skill_SwordThrow swordManager;

    protected Transform playerTransform;
    protected bool shouldCombeBack;
    protected float comeBackSpeed = 20f;
    protected float maxAllowedDistance = 25f;
    protected bool isStuck;
    private Collider2D myCollider;

    protected override void Awake()
    {
        base.Awake();
        myCollider = GetComponent<Collider2D>();
    }

    protected virtual void Update()
    {
        if (!isStuck)
            transform.right = rb.linearVelocity;

        HandleComeBack();
    }

    public virtual void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        rb.linearVelocity = direction;
        this.swordManager = swordManager;

        playerTransform = swordManager.transform.root;
        entityStats = swordManager.player.stats;
        damageScaleData = swordManager.damageScaleData;
    }
    public void GetSwordBackToPlayer() => shouldCombeBack = true;

    protected void HandleComeBack()
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance > maxAllowedDistance)
            GetSwordBackToPlayer();

        if (shouldCombeBack == false)
            return;

        // If stuck, restore physics and collider before returning
        if (isStuck)
        {
            transform.SetParent(null, true);
            if (myCollider != null)
                myCollider.enabled = true;

            // Make the rigidbody dynamic again for the return flight
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
            isStuck = false;
        }

        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, comeBackSpeed * Time.deltaTime);

        if (distance < .5f)
            Destroy(gameObject);
    }

    // Support both trigger and collision callbacks so prefab configuration won't break behavior
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (shouldCombeBack)
            return;

        StopAndDamage(collision);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (shouldCombeBack)
            return;

        StopAndDamage(collision.collider);
    }

    private void StopAndDamage(Collider2D collision)
    {
        StopSword(collision);
        DamageEnemiesInRadius(transform, 1);
    }

    protected void StopSword(Collider2D collision)
    {
        if (isStuck)
            return;

        isStuck = true;

        // Disable collider so it won't be pushed/push other colliders while stuck
        if (myCollider != null)
            myCollider.enabled = false;

        // Snap sword to the contact point so it won't "slide" or appear to fall
        Vector3 contactPoint = collision.ClosestPoint(transform.position);
        transform.position = contactPoint;

        // Fully stop physics
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.simulated = false;

        // If we hit an enemy, parent to that enemy so the sword moves with it (stuck to enemy)
        // If we hit ground, parent to ground so it visually sticks to terrain
        // Otherwise leave unparented but frozen in place
        Enemy enemy = collision.GetComponent<Enemy>();
        bool isGround = collision.gameObject.layer == LayerMask.NameToLayer("Ground");

        if (enemy != null)
        {
            transform.SetParent(enemy.transform, true); // stick to moving enemy
        }
        else if (isGround)
        {
            transform.SetParent(collision.transform, true); // stick to ground
        }
        else
        {
            transform.SetParent(null, true); // frozen in world space
        }

        // keep bodyType as Kinematic to avoid physics forcing movement while parented
        rb.bodyType = RigidbodyType2D.Kinematic;
    }
}