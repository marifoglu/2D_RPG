using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    public Enemy_Health enemyHealth { get; private set; }
    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;
    public Enemy_AttackState attackState;
    public Enemy_BattleState battleState;
    public Enemy_DeadState deadState;
    public Enemy_StunnedState stunnedState;

    [Header("Battle Settings")]
    public float battleMoveSpeed = 3f;
    public float attackDistance = 2f;
    public float battleTimeDuration = 3f;
    public float minRetreatDistance = 3f;
    public Vector2 retreatVelocity;

    [Header("Movement Settings")]
    public float idleTime = 2f;
    public float moveSpeed = 1.4f;
    [Range(0, 2)]
    public float moveAnimSpeedMultiplier = 1.0f;


    [Header("Player Detection Settings")]
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private float playerCheckDistance = 10f;
    public Transform player { get; private set; }
    public float activeSlowMultiplier { get; private set; } = 1;

    [Header("Stunned Settings")]
    public float stunnedDuration = 1f;
    public Vector2 stunnedVelocity = new Vector2(3f, 3f);
    [SerializeField] protected bool canBeStunned;

    [Header("Knockback Settings")]
    public Vector2 knockbackForce = new Vector2(5f, 3f);
    public float knockbackDuration = 0.15f;

    public bool IsInUninterruptibleState =>
        stateMachine.currentState == stunnedState ||
        stateMachine.currentState == deadState;

    private bool playerDead;
    public bool IsPlayerDead => playerDead;

    public bool CanBeCountered { get => canBeStunned; }

    public float GetMoveSpeed() => moveSpeed * activeSlowMultiplier;
    public float GetBattleMoveSpeed() => battleMoveSpeed * activeSlowMultiplier;

    protected override void Awake()
    {
        // Set enemy edge detection BEFORE base.Awake()
        useEnemyEdgeDetection = true;

        base.Awake();
        enemyHealth = GetComponent<Enemy_Health>();
        // Initialize all states in Awake
        idleState = new Enemy_IdleState(this, stateMachine, "Idle");
        moveState = new Enemy_MoveState(this, stateMachine, "Move");
        battleState = new Enemy_BattleState(this, stateMachine, "Battle");
        deadState = new Enemy_DeadState(this, stateMachine, "Dead");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "Stunned");

        // You need to create Enemy_AttackState - it was referenced but not instantiated
        // If you don't have it yet, comment out this line:
        attackState = new Enemy_AttackState(this, stateMachine, "Attack");
    }

    protected override void Start()
    {
        base.Start();

        // Start in idle state
        stateMachine.Initialize(idleState);
    }

    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        activeSlowMultiplier = 1 - slowMultiplier;

        anim.speed *= activeSlowMultiplier;

        yield return new WaitForSeconds(duration);

        StopSlowDown();
    }

    public void EnableCounterWindow(bool enable)
    {
        canBeStunned = enable;
    }

    public override void EntityDeath()
    {
        base.EntityDeath();
        stateMachine.ChangeState(deadState);
    }

    private void HandlePlayerDeath()
    {
        playerDead = true;
        player = null;
        rb.linearVelocity = Vector2.zero;
        stateMachine.ChangeState(idleState);
    }

    public void HandleCounter()
    {
        if (!canBeStunned)
        {
            return;
        }

        Transform p = GetPlayerDetection();
        if (p != null)
        {
            int dirFromPlayer = (transform.position.x < p.position.x) ? -1 : 1;
            ReceiveKnockback(new Vector2(knockbackForce.x * dirFromPlayer, knockbackForce.y), knockbackDuration);
        }

        stateMachine.ChangeState(stunnedState);
    }

    public void TryEnterBattleState(Transform detectedPlayer)
    {
        if (playerDead)
        {
            return;
        }

        this.player = detectedPlayer;

        if (IsInUninterruptibleState)
        {
            return;
        }

        if (stateMachine.currentState == battleState)
        {
            return;
        }

        if (stateMachine.currentState == attackState)
        {
            return;
        }

        stateMachine.ChangeState(battleState);
    }

    public Transform GetPlayerDetection()
    {
        if (player == null)
        {
            var hit = PlayerDetected();
            if (hit.collider != null)
                player = hit.transform;
        }

        return player;
    }

    public RaycastHit2D PlayerDetected()
    {
        if (playerDead)
            return default;

        RaycastHit2D hit = Physics2D.Raycast(playerCheck.position, Vector2.right * facingDir, playerCheckDistance, whatIsPlayer | whatIsGround);

        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
            return default;

        return hit;
    }

    public override void StopSlowDown()
    {
        activeSlowMultiplier = 1;
        anim.speed = 1;
        base.StopSlowDown();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (playerCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDir * playerCheckDistance), playerCheck.position.y));

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDir * attackDistance), playerCheck.position.y));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDir * minRetreatDistance), playerCheck.position.y));
    }

    private void OnEnable()
    {
        Player.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= HandlePlayerDeath;
    }
}