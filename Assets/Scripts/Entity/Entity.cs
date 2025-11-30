using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public event Action OnFlipped;
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Entity_Stats stats { get; private set; }
    protected StateMachine stateMachine;

    private bool facingRight = true;
    public int facingDir { get; private set; } = 1;

    [Header("Collision Detection")]
    public LayerMask whatIsGround;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform primaryWallCheck;
    [SerializeField] private Transform secondaryWallCheck;
    [SerializeField] private Transform ledgeCheck;

    [Header("Edge Detection")]
    [SerializeField] protected float edgeForwardOffset = 0.5f;
    [SerializeField] protected float edgeDownDistance = 0.25f;
    [SerializeField] protected bool useEnemyEdgeDetection = false;

    [Header("Slope Detection")]
    [SerializeField] private float slopeCheckDistance = 1.25f;
    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private float slopeOverlapRadius = 0.1f;
    [SerializeField] private PhysicsMaterial2D noFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;

    [Header("Ledge Climb Details")]
    [SerializeField] private Vector2 ledgeClimbOffset;

    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }
    public bool ledgeDetected { get; private set; }
    public bool edgeDetected { get; private set; }

    // Slope variables
    public bool isOnSlope { get; private set; }
    public bool canWalkOnSlope { get; private set; }
    public float slopeDownAngle { get; private set; }
    public Vector2 slopeNormalPerp { get; private set; }
    private float slopeSideAngle;
    private float lastSlopeAngle;
    protected float defaultGravity;
    private Vector2 capsuleColliderSize;
    private CapsuleCollider2D cc;

    // Condition Variables
    private bool isKnocked = false;
    private Coroutine knockbackCo;
    private Coroutine slowDownCo;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine();
        stats = GetComponent<Entity_Stats>();
        cc = GetComponent<CapsuleCollider2D>();
        if (cc != null)
        {
            capsuleColliderSize = cc.size;
        }
        else
        {
            // Fallback for enemies without CapsuleCollider2D
            var boxCol = GetComponent<BoxCollider2D>();
            if (boxCol != null)
                capsuleColliderSize = boxCol.size;
            else
                capsuleColliderSize = new Vector2(1f, 2f); // Default size
        }
        defaultGravity = rb.gravityScale;
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        HandleCollisionDetection();
        if (stateMachine != null && stateMachine.currentState != null)
        {
            stateMachine.UpdateActiveState();
        }
    }

    public virtual void StopSlowDown()
    {
        slowDownCo = null;

    }
    public virtual void SlowDownEntity(float duration, float slowMultiplier, bool canOverrideSlowDownEffect = false)
    {
        if (slowDownCo != null)
        {
            if (canOverrideSlowDownEffect)
                StopCoroutine(slowDownCo);
            else
                return;

        }

        slowDownCo = StartCoroutine(SlowDownEntityCo(duration, slowMultiplier));
    }

    protected virtual IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        yield return null;
    }
    public virtual void EntityDeath()
    {
        //Debug.Log("Entity Dead");
    }

    public void EnterStaggerState(EntityState staggerState)
    {
        stateMachine.ChangeState(staggerState);
    }

    public void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();
    }

    public void ReceiveKnockback(Vector2 konckback, float duration)
    {
        if (knockbackCo != null)
            StopCoroutine(knockbackCo);
        knockbackCo = StartCoroutine(KnockbackCo(konckback, duration));
    }

    private IEnumerator KnockbackCo(Vector2 knockback, float duration)
    {
        isKnocked = true;
        rb.linearVelocity = knockback;
        yield return new WaitForSeconds(duration);
        rb.linearVelocity = Vector2.zero;
        isKnocked = false;
    }
    public void SetVelocityRaw(float xVelocity, float yVelocity)
    {
        if (isKnocked)
            return;

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }
    public void SetVelocity(float xInput, float yVelocity)
    {
        if (isKnocked) return;

        // Enemy uses simplified slope logic
        if (useEnemyEdgeDetection)
        {
            // If on a slope and grounded, move along the slope direction
            if (groundDetected && isOnSlope && canWalkOnSlope && Mathf.Abs(xInput) > 0.01f)
            {
                Vector2 slopeDir = slopeNormalPerp.normalized;
                if (Mathf.Sign(slopeDir.x) != Mathf.Sign(xInput))
                    slopeDir *= -1;
                rb.linearVelocity = new Vector2(slopeDir.x * Mathf.Abs(xInput), slopeDir.y * Mathf.Abs(xInput));
            }
            else
            {
                rb.linearVelocity = new Vector2(xInput, yVelocity);
            }
            HandleFlip(xInput);
            return;
        }

        // Player slope logic
        bool touchingSlopeButNotDetectedYet =
        groundDetected && Mathf.Abs(xInput) > 0.01f && rb.linearVelocity.x == 0f && !isOnSlope && IsTouchingSlope();

        if (touchingSlopeButNotDetectedYet)
        {
            rb.linearVelocity = new Vector2(xInput * 2f, -2f);
            return;
        }
        if (isOnSlope && canWalkOnSlope && groundDetected)
        {
            Vector2 slopeDir = slopeNormalPerp.normalized;
            if (Mathf.Sign(slopeDir.x) != Mathf.Sign(xInput))
                slopeDir *= -1;
            rb.linearVelocity = slopeDir * Mathf.Abs(xInput);
        }
        else
        {
            rb.linearVelocity = new Vector2(xInput, yVelocity);
            if (!isOnSlope && groundDetected)
                rb.gravityScale = defaultGravity;
        }
        HandleFlip(xInput);
    }

    public void HandleFlip(float xVelcoity)
    {
        if (xVelcoity > 0 && facingRight == false)
            Flip();
        else if (xVelcoity < 0 && facingRight)
            Flip();
    }

    public void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDir = facingDir * -1;
        OnFlipped?.Invoke();
    }

    
    private void HandleCollisionDetection()
    {
        // Ground detection for simple raycast from groundCheck
        if (useEnemyEdgeDetection)
        {
            // Enemy uses simple ground check
            groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius, whatIsGround);
        }
        else
        {
            // Player uses more complex ground check
            // **FIXED: Use only the circle check, remove the raycast that causes issues with one-way platforms**
            groundDetected = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        }

        // Wall detection (same for both)
        if (secondaryWallCheck != null)
        {
            wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround)
            && Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        }
        else
        {
            wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        }

        // Ledge detection (for player only)
        if (ledgeCheck != null && !useEnemyEdgeDetection)
        {
            bool wallAtBody = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
            bool wallAtHead = Physics2D.Raycast(ledgeCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
            ledgeDetected = wallAtBody && !wallAtHead;
        }
        else
        {
            ledgeDetected = false;
        }

        // Edge detection - different method based on entity type
        if (useEnemyEdgeDetection)
        {
            // Enemy-style: check if ground exists ahead at feet level
            Vector2 feetPos = (Vector2)groundCheck.position;
            Vector2 forwardProbe = feetPos + Vector2.right * facingDir * edgeForwardOffset;
            float rayLength = groundCheckRadius + edgeDownDistance;

            bool groundAhead = Physics2D.Raycast(forwardProbe, Vector2.down, rayLength, whatIsGround);
            edgeDetected = groundDetected && !groundAhead;
        }
        else
        {
            // Player-style edge detection
            Vector2 frontPos = (Vector2)transform.position + new Vector2(facingDir * 0.5f, -0.5f);
            edgeDetected = !Physics2D.Raycast(frontPos, Vector2.down, 0.6f, whatIsGround);
        }
    }



    public Vector2 DetermineLedgePosition()
    {
        // Only allow ledge climbing if not using enemy detection
        if (useEnemyEdgeDetection)
            return transform.position;

        Vector2 wallTopCheck = new Vector2(primaryWallCheck.position.x, ledgeCheck.position.y);
        RaycastHit2D topHit = Physics2D.Raycast(wallTopCheck, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        if (topHit.collider != null)
        {
            Vector2 ledgePos = new Vector2(
                            topHit.point.x + (ledgeClimbOffset.x * -facingDir),
                            topHit.point.y + ledgeClimbOffset.y);
            return ledgePos;
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
            if (hit.collider != null)
            {
                Vector2 ledgePos = new Vector2(
                hit.point.x + (ledgeClimbOffset.x * -facingDir),
                hit.point.y + ledgeClimbOffset.y);
                return ledgePos;
            }
        }
        return transform.position;
    }

    protected virtual void FixedUpdate()
    {
        // Both player and enemy need slope calculations for proper movement
        SlopeCheck();
        ApplySlopeFriction();
    }

    private bool IsTouchingSlope()
    {
        Vector2 feetPos = (Vector2)transform.position + Vector2.down * (capsuleColliderSize.y * 0.5f + 0.05f);
        return Physics2D.OverlapCircle(feetPos, slopeOverlapRadius, whatIsGround);
    }

    private void SlopeCheck()
    {
        // Use groundCheck position for enemies, capsule collider for player
        Vector2 checkPos;

        if (useEnemyEdgeDetection && groundCheck != null)
        {
            // Enemy: check from groundCheck position
            checkPos = groundCheck.position;
        }
        else if (cc != null)
        {
            // Player: check from capsule collider bottom
            float yOffset = capsuleColliderSize.y * 0.5f + 0.05f;
            checkPos = (Vector2)transform.position + Vector2.down * yOffset;
        }
        else
        {
            // Fallback: use transform position
            checkPos = (Vector2)transform.position + Vector2.down * 0.5f;
        }

        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);
        if (hit)
        {
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            isOnSlope = slopeDownAngle > 0.1f;
            Debug.DrawRay(hit.point, hit.normal * 0.5f, Color.green);
        }
        else
        {
            slopeDownAngle = 0f;
            isOnSlope = false;
        }
        canWalkOnSlope = slopeDownAngle > 0.1f && slopeDownAngle <= maxSlopeAngle;
    }

    public void ApplySlopeFriction()
    {
        if (noFriction == null || fullFriction == null) return;

        var col = GetComponent<Collider2D>();
        if (col == null) return;

        // Check if this is a Player on a ladder
        var player = this as Player;
        if (player != null && player.IsOnLadder())
        {
            // Disable friction completely while on ladder
            col.sharedMaterial = noFriction;
            rb.gravityScale = 0f; // Keep gravity disabled
            return;
        }

        // Check if this is a Player dropping through a platform
        if (player != null && player.isDroppingThroughPlatform)
        {
            // Use no friction and full gravity when dropping through platform
            col.sharedMaterial = noFriction;
            rb.gravityScale = defaultGravity;
            return;
        }

        // Don't apply slope friction or gravity changes during Domain Expansion
        if (player != null && stateMachine.currentState == player.domainExpansionState)
        {
            col.sharedMaterial = noFriction;
            return;
        }

        // Both enemy and player use similar friction logic
        bool slopeWalk = isOnSlope && canWalkOnSlope && groundDetected && (rb.linearVelocity.y > -0.6f && rb.linearVelocity.y < 0.6f);

        if (slopeWalk)
        {
            col.sharedMaterial = fullFriction;

            if (groundDetected && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
                rb.gravityScale = 0f;
            else
                rb.gravityScale = defaultGravity;
        }
        else
        {
            col.sharedMaterial = noFriction;
            rb.gravityScale = defaultGravity;
        }
    }
    protected virtual void OnDrawGizmos()
    {
        if (groundCheck == null) return;

        // Ground check visualization
        Gizmos.color = Color.green;
        if (useEnemyEdgeDetection)
        {
            // Enemy: simple line down
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckRadius));
        }
        else
        {
            // Player: sphere
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Wall checks
        Gizmos.color = Color.blue;
        if (primaryWallCheck != null)
            Gizmos.DrawLine(primaryWallCheck.position, primaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));

        if (secondaryWallCheck != null)
            Gizmos.DrawLine(secondaryWallCheck.position, secondaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));

        // Ledge check (player only)
        Gizmos.color = Color.red;
        if (ledgeCheck != null && !useEnemyEdgeDetection)
            Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + new Vector3(wallCheckDistance * facingDir, 0));

        // Edge detection visualization
        if (useEnemyEdgeDetection)
        {
            Vector3 feetPos = groundCheck.position;
            Vector3 forwardProbe = feetPos + Vector3.right * facingDir * Mathf.Max(0.01f, edgeForwardOffset);
            float downDist = Mathf.Max(0f, groundCheckRadius + edgeDownDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(forwardProbe, forwardProbe + Vector3.down * downDist);
        }
    }
}