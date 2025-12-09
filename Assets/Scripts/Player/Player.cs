using System;
using System.Collections;
using UnityEngine;

public class Player : Entity
{
    private UI ui;

    public static event Action OnPlayerDeath;
    public PlayerInputSet input { get; private set; }
    public Player_SkillManager skillManager { get; private set; }
    public Player_VFX vfx { get; private set; }
    public Entity_Health health { get; private set; }
    public Entity_StatusHandler statusHandler { get; private set; }

    public Player_Combat combat;

    #region State variables
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_WallSlideState wallSlideState { get; private set; }
    public Player_WallJumpState wallJumpState { get; private set; }
    public Player_DashState dashState { get; private set; }
    public Player_BasicAttackState basicAttackState { get; private set; }
    public Player_JumpAttackState jumpAttackState { get; private set; }
    public Player_LedgeClimbState ledgeClimbState { get; private set; }
    public Player_DeadState deadState { get; private set; }
    public Player_CounterAttackState counterAttackState { get; private set; }
    public Player_SwordThrowState swordThrowState { get; private set; }
    public Player_DomainExpansionState domainExpansionState { get; private set; }
    public Player_LadderClimbState ladderClimbState { get; private set; }
    public Player_HeavyAttackState heavyAttackState { get; private set; }
    public Player_DodgeBackState dodgeBackState { get; private set; }
    #endregion

    [Header("Attack Details")]
    public Vector2[] attackVelocity;
    public Vector2 jumpAttackVelocity;
    public float attackVelocityDuration = .1f;
    public float comboResetTime = 1;
    private Coroutine queuedAttackCo;

    [Header("Heavy Attack Details")]
    public Vector2 heavyAttackVelocity = new Vector2(10f, 0f); // Single velocity, no array!
    public float heavyAttackVelocityDuration = 0.15f;

    [Header("Movement Details")]
    public float moveSpeed;
    public float jumpForce = 5;
    public Vector2 wallJumpForce;
    [Range(0, 1)]
    public float inAirMoveMultiplier = .7f;
    [Range(0, 1)]
    public float wallSlideSlowMultiplier = .7f;
    [Space]
    public float dashDuration = .25f;
    public float dashSpeed = 20f;
    [Space]
    public float dodgeBackDistance = 5f;
    public float dodgeBackSpeed = 5f;

    public Vector2 moveInput { get; private set; }
    public Vector2 mousePosition { get; private set; }

    [Header("Ledge Climb Cooldown")]
    public float ledgeClimbCooldown = 0.5f;
    private float lastLedgeClimbTime;

    [Header("One-Way Platform")]
    public bool isDroppingThroughPlatform { get; set; } = false;

    [Header("Ultimate Attack")]
    public float riseSpeed = 25f;
    public float riseMaxDistance = 3f;

    [Header("Ladder System")]
    private bool isOnLadder;
    private Ladder currentLadder;
    [SerializeField] private LayerMask whatIsLadder;
    [SerializeField] private float ladderDetectionRadius = 0.5f;
    [SerializeField] private float ladderEnterYOffset = 0.4f;
    [SerializeField] private float minDistanceBelowTopToGrab = 0.5f;
    [SerializeField] private float ladderExitIgnoreDuration = 0.5f;
    private float ladderIgnoreUntil = 0f;


    protected override void Awake()
    {
        base.Awake();

        input = new PlayerInputSet();

        ui = FindAnyObjectByType<UI>();
        vfx = GetComponent<Player_VFX>();
        health = GetComponent<Entity_Health>();
        statusHandler = GetComponent<Entity_StatusHandler>();
        skillManager = GetComponent<Player_SkillManager>();
        combat = GetComponent<Player_Combat>();

        idleState = new Player_IdleState(this, stateMachine, "Idle");
        moveState = new Player_MoveState(this, stateMachine, "Move");
        jumpState = new Player_JumpState(this, stateMachine, "JumpFall");
        fallState = new Player_FallState(this, stateMachine, "JumpFall");
        wallSlideState = new Player_WallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new Player_WallJumpState(this, stateMachine, "JumpFall");
        dashState = new Player_DashState(this, stateMachine, "Dash");
        basicAttackState = new Player_BasicAttackState(this, stateMachine, "BasicAttack");
        jumpAttackState = new Player_JumpAttackState(this, stateMachine, "JumpAttack");
        ledgeClimbState = new Player_LedgeClimbState(this, stateMachine, "LedgeClimb");
        deadState = new Player_DeadState(this, stateMachine, "Dead");
        counterAttackState = new Player_CounterAttackState(this, stateMachine, "CounterAttack");
        swordThrowState = new Player_SwordThrowState(this, stateMachine, "SwordThrow");
        domainExpansionState = new Player_DomainExpansionState(this, stateMachine, "JumpFall");
        ladderClimbState = new Player_LadderClimbState(this, stateMachine, "LadderClimb");
        heavyAttackState = new Player_HeavyAttackState(this, stateMachine, "HeavyAttack");
        dodgeBackState = new Player_DodgeBackState(this, stateMachine, "BackwardDodge");
    }

    public void ForceFallState()
    {
        stateMachine.ChangeState(fallState);
    }

    public void StartDropThrough()
    {
        isDroppingThroughPlatform = true;
        stateMachine.ChangeState(fallState);
    }

    public void EndDropThrough()
    {
        isDroppingThroughPlatform = false;
    }
    public void TeleportPlayer(Vector3 position) => transform.position = position;

    public bool CanLedgeClimb => Time.time >= lastLedgeClimbTime + ledgeClimbCooldown;

    public void SetLedgeClimbCooldown()
    {
        lastLedgeClimbTime = Time.time;
    }

    override protected void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
        DetectLadder();
    }

    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        float originalMoveSpeed = moveSpeed;
        float originalJumpForce = jumpForce;
        float originalAnimSpeed = anim.speed;
        Vector2 originalWallJump = wallJumpForce;
        Vector2 originalJumpAttack = jumpAttackVelocity;
        Vector2 originalHeavyAttackVelocity = heavyAttackVelocity; // No array!
        Vector2[] originalAttackVelocity = new Vector2[attackVelocity.Length];

        float speedMultiplier = 1 - slowMultiplier;

        moveSpeed *= speedMultiplier;
        jumpForce *= speedMultiplier;
        anim.speed *= speedMultiplier;
        wallJumpForce *= speedMultiplier;
        jumpAttackVelocity *= speedMultiplier;
        heavyAttackVelocity *= speedMultiplier; // Simple multiplication!

        for (int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] *= speedMultiplier;
        }

        yield return new WaitForSeconds(duration);

        moveSpeed = originalMoveSpeed;
        jumpForce = originalJumpForce;
        anim.speed = originalAnimSpeed;
        wallJumpForce = originalWallJump;
        jumpAttackVelocity = originalJumpAttack;
        heavyAttackVelocity = originalHeavyAttackVelocity; // Restore!

        for (int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] = originalAttackVelocity[i];
        }
    }

    public override void EntityDeath()
    {
        base.EntityDeath();

        OnPlayerDeath?.Invoke();

        stateMachine.ChangeState(deadState);
    }

    private void OnEnable()
    {
        if (input == null)
            input = new PlayerInputSet();

        input.Enable();
        input.PlayerCharacter.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.PlayerCharacter.Movement.canceled += ctx => moveInput = Vector2.zero;
        input.PlayerCharacter.Mouse.performed += ctx => mousePosition = ctx.ReadValue<Vector2>();

        input.PlayerCharacter.Spell.performed += ctx => skillManager.shard.TryUseSkill();
        input.PlayerCharacter.Spell.performed += ctx => skillManager.timeEcho.TryUseSkill();

        input.PlayerCharacter.ToggleSkillTreeUi.performed += ctx => ui.ToggleSkillTreeUI();
        input.PlayerCharacter.ToggleInventoryUI.performed += ctx => ui.ToggleInventoryUI();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    public void EnterAttackStateWithDelay()
    {
        if (queuedAttackCo != null)
            StopCoroutine(queuedAttackCo);
        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }

    private IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(basicAttackState);
    }

    #region Ladder Methods
    private void DetectLadder()
    {
        if (Time.time < ladderIgnoreUntil)
        {
            currentLadder = null;
            return;
        }

        Vector2 detectOrigin = (Vector2)transform.position + Vector2.up * ladderEnterYOffset;
        Collider2D ladderCollider = Physics2D.OverlapCircle(detectOrigin, ladderDetectionRadius, whatIsLadder);

        if (ladderCollider != null)
        {
            Ladder detectedLadder = ladderCollider.GetComponent<Ladder>();
            if (detectedLadder != null && detectedLadder.IsPlayerInZone(transform))
            {
                Vector2 ladderTop = detectedLadder.GetTopPosition();
                if (transform.position.y <= ladderTop.y + 0.05f)
                {
                    currentLadder = detectedLadder;
                    return;
                }
            }
        }

        if (currentLadder != null && !currentLadder.IsPlayerInZone(transform))
        {
            currentLadder = null;
            isOnLadder = false;
        }
    }

    public bool CanGrabLadder()
    {
        if (Time.time < ladderIgnoreUntil)
            return false;

        if (currentLadder == null || isOnLadder)
            return false;

        Vector2 ladderTop = currentLadder.GetTopPosition();
        if (transform.position.y >= ladderTop.y - minDistanceBelowTopToGrab)
            return false;

        return true;
    }

    public void SetLadderState(bool onLadder, Ladder ladder)
    {
        isOnLadder = onLadder;
        currentLadder = ladder;

        if (!onLadder)
            ladderIgnoreUntil = Time.time + ladderExitIgnoreDuration;
    }

    public bool IsOnLadder() => isOnLadder;

    public Ladder GetCurrentLadder() => currentLadder;

    public void TryGrabLadder()
    {
        if (CanGrabLadder())
        {
            SetLadderState(true, currentLadder);
            stateMachine.ChangeState(ladderClimbState);
        }
    }
    #endregion
}