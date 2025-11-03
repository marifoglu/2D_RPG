using UnityEngine;

public class Enemy : Entity_Enemy
{

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

    [Header("Stunned Settings")]
    public float stunnedDuration = 1f;   // Default 1 second, can change per enemy in Inspector
    public Vector2 stunnedVelocity = new Vector2(3f, 3f);
    [SerializeField] protected bool canBeStunned;

    [Header("Knockback Settings")]
    public Vector2 knockbackForce = new Vector2(5f, 3f);
    public float knockbackDuration = 0.15f;

    // Public property to check if enemy is in a state that shouldn't be interrupted
    public bool IsInUninterruptibleState =>
        stateMachine.currentState == stunnedState ||
        stateMachine.currentState == deadState;

    // Track player life state to avoid attacking a dead player
    private bool playerDead;
    public bool IsPlayerDead => playerDead;

    public bool CanBeCountered { get => canBeStunned; }

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
        // Mark player as dead and clear any references/motion
        playerDead = true;
        player = null;
        rb.linearVelocity = Vector2.zero;

        // Immediately leave battle/attack and go idle
        stateMachine.ChangeState(idleState);
    }

    public void HandleCounter()
    {

        if (!canBeStunned)
        {
            return;
        }

        // Apply knockback away from player
        Transform p = GetPlayerDetection();
        if (p != null)
        {
            int dirFromPlayer = (transform.position.x < p.position.x) ? -1 : 1;
            ReceiveKnockback(new Vector2(knockbackForce.x * dirFromPlayer, knockbackForce.y), knockbackDuration);
        }
        else
        {
        }

        // StateMachine.ChangeState() already calls Exit() on the current state
        // No need to manually call it
        stateMachine.ChangeState(stunnedState);
    }

    public void TryEnterBattleState(Transform detectedPlayer)
    {
        // Do not enter battle if the player is dead
        if (playerDead)
        {
            return;
        }

        // Always update the player reference when detected
        this.player = detectedPlayer;

        // Don't interrupt uninterruptible states
        if (IsInUninterruptibleState)
        {
            return;
        }

        // Only change state if not already in battle or attacking
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
            player = PlayerDetected().transform;

        return player;
    }

    public RaycastHit2D PlayerDetected()
    {
        // If player is dead, pretend nothing is detected
        if (playerDead)
            return default;

        RaycastHit2D hit = Physics2D.Raycast(playerCheck.position, Vector2.right * facingDir, playerCheckDistance, whatIsPlayer | whatIsGround);

        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
            return default;
        return hit;
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

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