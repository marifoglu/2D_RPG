//using System;
//using System.Collections;
//using UnityEngine;

//public class Entity : MonoBehaviour
//{
//    public event Action OnFlipped;
//    public Animator anim { get; private set; }
//    public Rigidbody2D rb { get; private set; }
//    public Entity_Stats stats { get; private set; }

//    protected StateMachine stateMachine;


//    private bool facingRight = true;
//    public int facingDir { get; private set; } = 1;


//    [Header("Collision Detection")]
//    public LayerMask whatIsGround;
//    [SerializeField] private float groundCheckRadius = 0.1f;
//    [SerializeField] private float wallCheckDistance;
//    [SerializeField] private Transform groundCheck;
//    [SerializeField] private Transform primaryWallCheck;
//    [SerializeField] private Transform secondaryWallCheck;
//    [SerializeField] private Transform ledgeCheck;

//    [Header("Edge Detection")]
//    [Tooltip("Horizontal look-ahead from feet to check for ground ahead.")]
//    [SerializeField] protected float edgeForwardOffset = 0.5f;
//    [Tooltip("Extra downward distance from the look-ahead point.")]
//    [SerializeField] protected float edgeDownDistance = 0.25f;
//    [Tooltip("Use enemy-style edge detection (simpler raycast forward from feet)")]
//    [SerializeField] protected bool useEnemyEdgeDetection = false;

//    [Header("Slope Detection")]
//    [SerializeField] private float slopeCheckDistance = 1.25f;
//    [SerializeField] private float maxSlopeAngle = 45f;
//    [SerializeField] private float slopeOverlapRadius = 0.1f;
//    [SerializeField] private PhysicsMaterial2D noFriction;
//    [SerializeField] private PhysicsMaterial2D fullFriction;

//    [Header("Ledge Climb Details")]
//    [SerializeField] private Vector2 ledgeClimbOffset;

//    public bool groundDetected { get; private set; }
//    public bool wallDetected { get; private set; }
//    public bool ledgeDetected { get; private set; }
//    public bool edgeDetected { get; private set; }

//    // Slope variables
//    public bool isOnSlope { get; private set; }
//    public bool canWalkOnSlope { get; private set; }
//    public float slopeDownAngle { get; private set; }
//    public Vector2 slopeNormalPerp { get; private set; }
//    private float slopeSideAngle;
//    private float lastSlopeAngle;
//    protected float defaultGravity;
//    private Vector2 capsuleColliderSize;
//    private CapsuleCollider2D cc;

//    // Condition Variables
//    private bool isKnocked = false;
//    private Coroutine knockbackCo;
//    private Coroutine slowDownCo;

//    protected virtual void Awake()
//    {
//        anim = GetComponentInChildren<Animator>();
//        rb = GetComponent<Rigidbody2D>();
//        stateMachine = new StateMachine();
//        stats = GetComponent<Entity_Stats>();
//        cc = GetComponent<CapsuleCollider2D>();
//        if (cc != null)
//        {
//            capsuleColliderSize = cc.size;
//        }
//        else
//        {
//            // Fallback for enemies without CapsuleCollider2D
//            var boxCol = GetComponent<BoxCollider2D>();
//            if (boxCol != null)
//                capsuleColliderSize = boxCol.size;
//            else
//                capsuleColliderSize = new Vector2(1f, 2f); // Default size
//        }
//        defaultGravity = rb.gravityScale;
//    }

//    protected virtual void Start()
//    {
//    }

//    protected virtual void Update()
//    {
//        HandleCollisionDetection();
//        if (stateMachine != null && stateMachine.currentState != null)
//        {
//            stateMachine.UpdateActiveState();
//        }
//    }

//    public virtual void StopSlowDown()
//    {
//        slowDownCo = null;

//    }
//    public virtual void SlowDownEntity(float duration, float slowMultiplier, bool canOverrideSlowDownEffect = false)
//    {
//        if(slowDownCo != null)
//        {
//            if(canOverrideSlowDownEffect)
//                StopCoroutine(slowDownCo);
//            else
//                return; 

//        }

//        slowDownCo = StartCoroutine(SlowDownEntityCo(duration, slowMultiplier));
//    }

//    protected virtual IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
//    {
//        yield return null;
//    }
//    public virtual void EntityDeath()
//    {
//        //Debug.Log("Entity Dead");
//    }

//    public void EnterStaggerState(EntityState staggerState)
//    {
//        stateMachine.ChangeState(staggerState);
//    }

//    public void CurrentStateAnimationTrigger()
//    {
//        stateMachine.currentState.AnimationTrigger();
//    }

//    public void ReceiveKnockback(Vector2 konckback, float duration)
//    {
//        if (knockbackCo != null)
//            StopCoroutine(knockbackCo);
//        knockbackCo = StartCoroutine(KnockbackCo(konckback, duration));
//    }

//    private IEnumerator KnockbackCo(Vector2 knockback, float duration)
//    {
//        isKnocked = true;
//        rb.linearVelocity = knockback;
//        yield return new WaitForSeconds(duration);
//        rb.linearVelocity = Vector2.zero;
//        isKnocked = false;
//    }
//    public void SetVelocityRaw(float xVelocity, float yVelocity)
//    {
//        if (isKnocked)
//            return;

//        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
//        HandleFlip(xVelocity);
//    }
//    public void SetVelocity(float xInput, float yVelocity)
//    {
//        if (isKnocked) return;

//        // Enemy uses simplified slope logic
//        if (useEnemyEdgeDetection)
//        {
//            // If on a slope and grounded, move along the slope direction
//            if (groundDetected && isOnSlope && canWalkOnSlope && Mathf.Abs(xInput) > 0.01f)
//            {
//                Vector2 slopeDir = slopeNormalPerp.normalized;
//                if (Mathf.Sign(slopeDir.x) != Mathf.Sign(xInput))
//                    slopeDir *= -1;
//                rb.linearVelocity = new Vector2(slopeDir.x * Mathf.Abs(xInput), slopeDir.y * Mathf.Abs(xInput));
//            }
//            else
//            {
//                rb.linearVelocity = new Vector2(xInput, yVelocity);
//            }
//            HandleFlip(xInput);
//            return;
//        }

//        // Player slope logic
//        bool touchingSlopeButNotDetectedYet =
//        groundDetected && Mathf.Abs(xInput) > 0.01f && rb.linearVelocity.x == 0f && !isOnSlope && IsTouchingSlope();

//        if (touchingSlopeButNotDetectedYet)
//        {
//            rb.linearVelocity = new Vector2(xInput * 2f, -2f);
//            return;
//        }
//        if (isOnSlope && canWalkOnSlope && groundDetected)
//        {
//            Vector2 slopeDir = slopeNormalPerp.normalized;
//            if (Mathf.Sign(slopeDir.x) != Mathf.Sign(xInput))
//                slopeDir *= -1;
//            rb.linearVelocity = slopeDir * Mathf.Abs(xInput);
//        }
//        else
//        {
//            rb.linearVelocity = new Vector2(xInput, yVelocity);
//            if (!isOnSlope && groundDetected)
//                rb.gravityScale = defaultGravity;
//        }
//        HandleFlip(xInput);
//    }

//    public void HandleFlip(float xVelcoity)
//    {
//        if (xVelcoity > 0 && facingRight == false)
//            Flip();
//        else if (xVelcoity < 0 && facingRight)
//            Flip();
//    }

//    public void Flip()
//    {
//        transform.Rotate(0, 180, 0);
//        facingRight = !facingRight;
//        facingDir = facingDir * -1;
//        OnFlipped?.Invoke();
//    }

//    //private void HandleCollisionDetection()
//    //{
//    //    // Ground detection for simple raycast from groundCheck
//    //    if (useEnemyEdgeDetection)
//    //    {
//    //        // Enemy uses simple ground check
//    //        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius, whatIsGround);
//    //    }
//    //    else
//    //    {
//    //        // Player uses more complex ground check
//    //        groundDetected =
//    //        Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround)
//    //        || Physics2D.Raycast(transform.position, Vector2.down, capsuleColliderSize.y * 0.6f, whatIsGround);
//    //    }

//    //    // Wall detection (same for both)
//    //    if (secondaryWallCheck != null)
//    //    {
//    //        wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround)
//    //        && Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
//    //    }
//    //    else
//    //    {
//    //        wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
//    //    }

//    //    // Ledge detection (for player only)
//    //    if (ledgeCheck != null && !useEnemyEdgeDetection)
//    //    {
//    //        bool wallAtBody = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
//    //        bool wallAtHead = Physics2D.Raycast(ledgeCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
//    //        ledgeDetected = wallAtBody && !wallAtHead;
//    //    }
//    //    else
//    //    {
//    //        ledgeDetected = false;
//    //    }

//    //    // Edge detection - different method based on entity type
//    //    if (useEnemyEdgeDetection)
//    //    {
//    //        // Enemy-style: check if ground exists ahead at feet level
//    //        Vector2 feetPos = (Vector2)groundCheck.position;
//    //        Vector2 forwardProbe = feetPos + Vector2.right * facingDir * edgeForwardOffset;
//    //        float rayLength = groundCheckRadius + edgeDownDistance;

//    //        bool groundAhead = Physics2D.Raycast(forwardProbe, Vector2.down, rayLength, whatIsGround);
//    //        edgeDetected = groundDetected && !groundAhead;
//    //    }
//    //    else
//    //    {
//    //        // Player-style edge detection
//    //        Vector2 frontPos = (Vector2)transform.position + new Vector2(facingDir * 0.5f, -0.5f);
//    //        edgeDetected = !Physics2D.Raycast(frontPos, Vector2.down, 0.6f, whatIsGround);
//    //    }
//    //}





//    private void HandleCollisionDetection()
//    {
//        // Ground detection for simple raycast from groundCheck
//        if (useEnemyEdgeDetection)
//        {
//            // Enemy uses simple ground check
//            groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius, whatIsGround);
//        }
//        else
//        {
//            // Player uses more complex ground check
//            // **FIXED: Use only the circle check, remove the raycast that causes issues with one-way platforms**
//            groundDetected = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
//        }

//        // Wall detection (same for both)
//        if (secondaryWallCheck != null)
//        {
//            wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround)
//            && Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
//        }
//        else
//        {
//            wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
//        }

//        // Ledge detection (for player only)
//        if (ledgeCheck != null && !useEnemyEdgeDetection)
//        {
//            bool wallAtBody = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
//            bool wallAtHead = Physics2D.Raycast(ledgeCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
//            ledgeDetected = wallAtBody && !wallAtHead;
//        }
//        else
//        {
//            ledgeDetected = false;
//        }

//        // Edge detection - different method based on entity type
//        if (useEnemyEdgeDetection)
//        {
//            // Enemy-style: check if ground exists ahead at feet level
//            Vector2 feetPos = (Vector2)groundCheck.position;
//            Vector2 forwardProbe = feetPos + Vector2.right * facingDir * edgeForwardOffset;
//            float rayLength = groundCheckRadius + edgeDownDistance;

//            bool groundAhead = Physics2D.Raycast(forwardProbe, Vector2.down, rayLength, whatIsGround);
//            edgeDetected = groundDetected && !groundAhead;
//        }
//        else
//        {
//            // Player-style edge detection
//            Vector2 frontPos = (Vector2)transform.position + new Vector2(facingDir * 0.5f, -0.5f);
//            edgeDetected = !Physics2D.Raycast(frontPos, Vector2.down, 0.6f, whatIsGround);
//        }
//    }



//    public Vector2 DetermineLedgePosition()
//    {
//        // Only allow ledge climbing if not using enemy detection
//        if (useEnemyEdgeDetection)
//            return transform.position;

//        Vector2 wallTopCheck = new Vector2(primaryWallCheck.position.x, ledgeCheck.position.y);
//        RaycastHit2D topHit = Physics2D.Raycast(wallTopCheck, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
//        if (topHit.collider != null)
//        {
//            Vector2 ledgePos = new Vector2(
//                            topHit.point.x + (ledgeClimbOffset.x * -facingDir),
//                            topHit.point.y + ledgeClimbOffset.y);
//            return ledgePos;
//        }
//        else
//        {
//            RaycastHit2D hit = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
//            if (hit.collider != null)
//            {
//                Vector2 ledgePos = new Vector2(
//                hit.point.x + (ledgeClimbOffset.x * -facingDir),
//                hit.point.y + ledgeClimbOffset.y);
//                return ledgePos;
//            }
//        }
//        return transform.position;
//    }

//    protected virtual void FixedUpdate()
//    {
//        // Both player and enemy need slope calculations for proper movement
//        SlopeCheck();
//        ApplySlopeFriction();
//    }

//    private bool IsTouchingSlope()
//    {
//        Vector2 feetPos = (Vector2)transform.position + Vector2.down * (capsuleColliderSize.y * 0.5f + 0.05f);
//        return Physics2D.OverlapCircle(feetPos, slopeOverlapRadius, whatIsGround);
//    }

//    private void SlopeCheck()
//    {
//        // Use groundCheck position for enemies, capsule collider for player
//        Vector2 checkPos;

//        if (useEnemyEdgeDetection && groundCheck != null)
//        {
//            // Enemy: check from groundCheck position
//            checkPos = groundCheck.position;
//        }
//        else if (cc != null)
//        {
//            // Player: check from capsule collider bottom
//            float yOffset = capsuleColliderSize.y * 0.5f + 0.05f;
//            checkPos = (Vector2)transform.position + Vector2.down * yOffset;
//        }
//        else
//        {
//            // Fallback: use transform position
//            checkPos = (Vector2)transform.position + Vector2.down * 0.5f;
//        }

//        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);
//        if (hit)
//        {
//            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
//            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
//            isOnSlope = slopeDownAngle > 0.1f;
//            Debug.DrawRay(hit.point, hit.normal * 0.5f, Color.green);
//        }
//        else
//        {
//            slopeDownAngle = 0f;
//            isOnSlope = false;
//        }
//        canWalkOnSlope = slopeDownAngle > 0.1f && slopeDownAngle <= maxSlopeAngle;
//    }

//    //public void ApplySlopeFriction()
//    //{
//    //    if (noFriction == null || fullFriction == null) return;

//    //    var col = GetComponent<Collider2D>();
//    //    if (col == null) return;

//    //    // Check if this is a Player dropping through a platform
//    //    var player = this as Player;
//    //    if (player != null && player.isDroppingThroughPlatform)
//    //    {
//    //        // Use no friction and full gravity when dropping through platform
//    //        col.sharedMaterial = noFriction;
//    //        rb.gravityScale = defaultGravity;
//    //        return;
//    //    }

//    //    // Don't apply slope friction or gravity changes during Domain Expansion
//    //    if (player != null && stateMachine.currentState == player.domainExpansionState)
//    //    {
//    //        col.sharedMaterial = noFriction;
//    //        return;
//    //    }

//    //    // Both enemy and player use similar friction logic
//    //    bool slopeWalk = isOnSlope && canWalkOnSlope && groundDetected && (rb.linearVelocity.y > -0.6f && rb.linearVelocity.y < 0.6f);

//    //    if (slopeWalk)
//    //    {
//    //        col.sharedMaterial = fullFriction;

//    //        if (groundDetected && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
//    //            rb.gravityScale = 0f;
//    //        else
//    //            rb.gravityScale = defaultGravity;
//    //    }
//    //    else
//    //    {
//    //        col.sharedMaterial = noFriction;
//    //        rb.gravityScale = defaultGravity;
//    //    }
//    //}
//    // Replace your ApplySlopeFriction method in Entity.cs with this updated version:

//    public void ApplySlopeFriction()
//    {
//        if (noFriction == null || fullFriction == null) return;

//        var col = GetComponent<Collider2D>();
//        if (col == null) return;

//        // Check if this is a Player on a ladder
//        var player = this as Player;
//        if (player != null && player.IsOnLadder())
//        {
//            // Disable friction completely while on ladder
//            col.sharedMaterial = noFriction;
//            rb.gravityScale = 0f; // Keep gravity disabled
//            return;
//        }

//        // Check if this is a Player dropping through a platform
//        if (player != null && player.isDroppingThroughPlatform)
//        {
//            // Use no friction and full gravity when dropping through platform
//            col.sharedMaterial = noFriction;
//            rb.gravityScale = defaultGravity;
//            return;
//        }

//        // Don't apply slope friction or gravity changes during Domain Expansion
//        if (player != null && stateMachine.currentState == player.domainExpansionState)
//        {
//            col.sharedMaterial = noFriction;
//            return;
//        }

//        // Both enemy and player use similar friction logic
//        bool slopeWalk = isOnSlope && canWalkOnSlope && groundDetected && (rb.linearVelocity.y > -0.6f && rb.linearVelocity.y < 0.6f);

//        if (slopeWalk)
//        {
//            col.sharedMaterial = fullFriction;

//            if (groundDetected && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
//                rb.gravityScale = 0f;
//            else
//                rb.gravityScale = defaultGravity;
//        }
//        else
//        {
//            col.sharedMaterial = noFriction;
//            rb.gravityScale = defaultGravity;
//        }
//    }
//    protected virtual void OnDrawGizmos()
//    {
//        if (groundCheck == null) return;

//        // Ground check visualization
//        Gizmos.color = Color.green;
//        if (useEnemyEdgeDetection)
//        {
//            // Enemy: simple line down
//            Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckRadius));
//        }
//        else
//        {
//            // Player: sphere
//            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
//        }

//        // Wall checks
//        Gizmos.color = Color.blue;
//        if (primaryWallCheck != null)
//            Gizmos.DrawLine(primaryWallCheck.position, primaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));

//        if (secondaryWallCheck != null)
//            Gizmos.DrawLine(secondaryWallCheck.position, secondaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));

//        // Ledge check (player only)
//        Gizmos.color = Color.red;
//        if (ledgeCheck != null && !useEnemyEdgeDetection)
//            Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + new Vector3(wallCheckDistance * facingDir, 0));

//        // Edge detection visualization
//        if (useEnemyEdgeDetection)
//        {
//            Vector3 feetPos = groundCheck.position;
//            Vector3 forwardProbe = feetPos + Vector3.right * facingDir * Mathf.Max(0.01f, edgeForwardOffset);
//            float downDist = Mathf.Max(0f, groundCheckRadius + edgeDownDistance);

//            Gizmos.color = Color.yellow;
//            Gizmos.DrawLine(forwardProbe, forwardProbe + Vector3.down * downDist);
//        }
//    }
//}

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
    [SerializeField] private LayerMask whatIsOneWayPlatform;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform primaryWallCheck;
    [SerializeField] private Transform secondaryWallCheck;
    [SerializeField] private Transform ledgeCheck;

    [Header("Edge Detection")]
    [Tooltip("Horizontal look-ahead from feet to check for ground ahead.")]
    [SerializeField] protected float edgeForwardOffset = 0.5f;
    [Tooltip("Extra downward distance from the look-ahead point.")]
    [SerializeField] protected float edgeDownDistance = 0.25f;
    [Tooltip("Use enemy-style edge detection (simpler raycast forward from feet)")]
    [SerializeField] protected bool useEnemyEdgeDetection = false;

    [Header("Slope Detection")]
    [SerializeField] private float slopeCheckDistance = 1.25f;
    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private float slopeOverlapRadius = 0.1f;
    [SerializeField] private PhysicsMaterial2D noFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;

    [Header("Ledge Climb Details")]
    [SerializeField] private Vector2 ledgeClimbOffset;
    [Tooltip("How close (world units) the player must be to a platform's horizontal edge to consider it a climbable ledge.")]
    [SerializeField] private float ledgeEdgeTolerance = 0.18f;
    [Tooltip("Draw debug gizmos for ledge checks in the Scene view.")]
    [SerializeField] private bool debugLedgeChecks = false;

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

        // If the one-way platform mask hasn't been set in inspector, try to auto-assign the "OneWayPlatform" layer.
        if (whatIsOneWayPlatform == 0)
        {
            int layerIndex = LayerMask.NameToLayer("OneWayPlatform");
            if (layerIndex != -1)
                whatIsOneWayPlatform = 1 << layerIndex;
        }
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
        // Combined mask used for ground/edge/slope checks (includes one-way platforms)
        int combinedMask = whatIsGround | whatIsOneWayPlatform;

        // Ground detection
        if (useEnemyEdgeDetection)
        {
            groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius, combinedMask);
        }
        else
        {
            bool regularGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

            bool oneWayGround = false;
            var player = this as Player;
            if (player != null && !player.isDroppingThroughPlatform)
            {
                oneWayGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsOneWayPlatform);
            }

            groundDetected = regularGround || oneWayGround;
        }

        // Wall detection: exclude one-way platforms (platforms shouldn't act as walls)
        if (secondaryWallCheck != null)
        {
            wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround)
                         && Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        }
        else
        {
            wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        }

        if (ledgeCheck != null && !useEnemyEdgeDetection)
        {
            RaycastHit2D bodyHit = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, combinedMask);
            RaycastHit2D headHit = Physics2D.Raycast(ledgeCheck.position, Vector2.right * facingDir, wallCheckDistance, combinedMask);

            bool validBodyWall = false;
            bool validHeadWall = false;

            // bodyHit handling
            if (bodyHit.collider != null)
            {
                int hitLayerMask = 1 << bodyHit.collider.gameObject.layer;
                bool hitIsSolid = (whatIsGround.value & hitLayerMask) != 0;
                bool hitIsOneWay = (whatIsOneWayPlatform.value & hitLayerMask) != 0;

                // Regular solid vertical wall
                if (hitIsSolid && Mathf.Abs(bodyHit.normal.x) > 0.5f)
                {
                    validBodyWall = true;
                }
                else if (hitIsOneWay)
                {
                    // For one-way: require proximity to horizontal edge and that the hit is actually on top of that platform
                    var platformBounds = bodyHit.collider.bounds;
                    float edgeX = facingDir > 0 ? platformBounds.max.x : platformBounds.min.x;
                    float distToEdge = Mathf.Abs(bodyHit.point.x - edgeX);

                    if (distToEdge <= ledgeEdgeTolerance)
                    {
                        // Probe down from slightly above the platform edge to confirm same collider top
                        Vector2 probeOrigin = new Vector2(edgeX - facingDir * 0.01f, platformBounds.max.y + 0.1f);
                        RaycastHit2D topProbe = Physics2D.Raycast(probeOrigin, Vector2.down, platformBounds.size.y + 0.5f, whatIsOneWayPlatform);
                        if (topProbe.collider == bodyHit.collider)
                            validBodyWall = true;
                    }
                }
            }

            // headHit handling: if head area is blocked by a vertical surface (solid or platform near edge), disallow climb
            if (headHit.collider != null)
            {
                int hitLayerMaskH = 1 << headHit.collider.gameObject.layer;
                bool headIsSolid = (whatIsGround.value & hitLayerMaskH) != 0;
                bool headIsOneWay = (whatIsOneWayPlatform.value & hitLayerMaskH) != 0;

                if (headIsSolid && Mathf.Abs(headHit.normal.x) > 0.5f)
                {
                    validHeadWall = true;
                }
                else if (headIsOneWay)
                {
                    var platformBoundsH = headHit.collider.bounds;
                    float edgeXH = facingDir > 0 ? platformBoundsH.max.x : platformBoundsH.min.x;
                    float distToEdgeH = Mathf.Abs(headHit.point.x - edgeXH);
                    if (distToEdgeH <= ledgeEdgeTolerance)
                    {
                        // If head hits same platform and is close to edge, consider head blocked
                        RaycastHit2D topProbeH = Physics2D.Raycast(new Vector2(edgeXH - facingDir * 0.01f, platformBoundsH.max.y + 0.1f), Vector2.down, platformBoundsH.size.y + 0.5f, whatIsOneWayPlatform);
                        if (topProbeH.collider == headHit.collider)
                            validHeadWall = true;
                    }
                }
            }

            ledgeDetected = validBodyWall && !validHeadWall;

            if (debugLedgeChecks)
            {
                if (bodyHit.collider != null)
                {
                    Debug.DrawLine(primaryWallCheck.position, bodyHit.point, validBodyWall ? Color.green : Color.yellow);
                    Debug.DrawRay(bodyHit.point, bodyHit.normal * 0.2f, Color.cyan);
                }
                if (headHit.collider != null)
                {
                    Debug.DrawLine(ledgeCheck.position, headHit.point, validHeadWall ? Color.red : Color.magenta);
                    Debug.DrawRay(headHit.point, headHit.normal * 0.2f, Color.gray);
                }
            }
        }
        else
        {
            ledgeDetected = false;
        }

        // Edge detection:
        if (useEnemyEdgeDetection)
        {
            Vector2 feetPos = (Vector2)groundCheck.position;
            Vector2 forwardProbe = feetPos + Vector2.right * facingDir * edgeForwardOffset;
            float rayLength = groundCheckRadius + edgeDownDistance;

            bool groundAhead = Physics2D.Raycast(forwardProbe, Vector2.down, rayLength, combinedMask);
            edgeDetected = groundDetected && !groundAhead;
        }
        else
        {
            Vector2 frontPos = (Vector2)transform.position + new Vector2(facingDir * edgeForwardOffset, -groundCheckRadius);
            bool groundAhead = Physics2D.OverlapCircle(frontPos, groundCheckRadius, combinedMask);
            edgeDetected = groundDetected && !groundAhead;
        }
    }

    public Vector2 DetermineLedgePosition()
    {
        // Only allow ledge climbing if not using enemy detection
        if (useEnemyEdgeDetection)
            return transform.position;

        // First try regular solid ground ledge as before
        Vector2 wallTopCheck = new Vector2(primaryWallCheck.position.x, ledgeCheck.position.y);
        RaycastHit2D topHit = Physics2D.Raycast(wallTopCheck, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        if (topHit.collider != null)
        {
            return new Vector2(
                topHit.point.x + (ledgeClimbOffset.x * -facingDir),
                topHit.point.y + ledgeClimbOffset.y);
        }

        // If no solid hit, check one-way platform close to edge
        RaycastHit2D bodyHit = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsOneWayPlatform);
        if (bodyHit.collider != null)
        {
            var platformBounds = bodyHit.collider.bounds;
            float edgeX = facingDir > 0 ? platformBounds.max.x : platformBounds.min.x;
            float topY = platformBounds.max.y;

            return new Vector2(
                edgeX + (ledgeClimbOffset.x * -facingDir),
                topY + ledgeClimbOffset.y);
        }

        // Fallback: previous behavior
        RaycastHit2D hit = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        if (hit.collider != null)
        {
            return new Vector2(
                hit.point.x + (ledgeClimbOffset.x * -facingDir),
                hit.point.y + ledgeClimbOffset.y);
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
        return Physics2D.OverlapCircle(feetPos, slopeOverlapRadius, whatIsGround | whatIsOneWayPlatform);
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

        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround | whatIsOneWayPlatform);
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

        // CRITICAL Fix: Don't apply slope friction when jumping or falling in air
        if (player != null)
        {
            if (stateMachine.currentState == player.jumpState ||
                stateMachine.currentState == player.wallJumpState ||
                (!groundDetected && stateMachine.currentState == player.fallState))
            {
                col.sharedMaterial = noFriction;
                rb.gravityScale = defaultGravity;
                return;
            }
        }

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