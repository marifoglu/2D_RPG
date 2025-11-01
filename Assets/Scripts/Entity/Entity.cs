using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public event Action OnFlipped;
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    protected StateMachine stateMachine;
    private bool facingRight = true;
    public int facingDir { get; private set; } = 1;
    [Header("Collision Detection")]
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform primaryWallCheck;
    [SerializeField] private Transform secondaryWallCheck;
    [SerializeField] private Transform ledgeCheck;
    [Header("Slope Detection")]
    [SerializeField] private float slopeCheckDistance = 1.25f;
    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private float slopeOverlapRadius = 0.1f;
    [SerializeField] private PhysicsMaterial2D noFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;
    [Header("Ledge Climb Details")]
    [SerializeField] private Vector2 ledgeClimbOffset;
    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }
    public bool ledgeDetected { get; private set; }
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
    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine();
        cc = GetComponent<CapsuleCollider2D>();
        if (cc != null)
            capsuleColliderSize = cc.size;
        defaultGravity = rb.gravityScale; // store original gravity

    }

    protected virtual void Start()
    {
    }
    protected virtual void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }
    public virtual void EntityDeath()
    {
        Debug.Log("Entity Dead");
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
    public void SetVelocity(float xInput, float yVelocity)
    {
        if (isKnocked) return;

        bool touchingSlopeButNotDetectedYet =
        groundDetected &&
        Mathf.Abs(xInput) > 0.01f &&
        rb.linearVelocity.x == 0f &&     // trying to move but blocked
                    !isOnSlope &&
        IsTouchingSlope();               // <-- NEW overlap check
        if (touchingSlopeButNotDetectedYet)
        {
            // Force player onto slope
            rb.linearVelocity = new Vector2(xInput * 2f, -2f);
            return;
        }
        // Slope movement (when detected)
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
        groundDetected =
        Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround)
        || Physics2D.Raycast(transform.position, Vector2.down, capsuleColliderSize.y * 0.6f, whatIsGround);
        if (secondaryWallCheck != null)
        {
            wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround)
            && Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        }
        else
        {
            wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        }
        // Proper ledge detection: Wall at body level but NO wall at head level
        bool wallAtBody = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        bool wallAtHead = Physics2D.Raycast(ledgeCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        ledgeDetected = wallAtBody && !wallAtHead;
    }
    public Vector2 DetermineLedgePosition()
    {
        // Get the top of the wall by checking from above
        Vector2 wallTopCheck = new Vector2(primaryWallCheck.position.x, ledgeCheck.position.y);
        RaycastHit2D topHit = Physics2D.Raycast(wallTopCheck, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        if (topHit.collider != null)
        {
            // We hit the top of the wall, move to the other side
            Vector2 ledgePos = new Vector2(
topHit.point.x + (ledgeClimbOffset.x * -facingDir),
topHit.point.y + ledgeClimbOffset.y);
            return ledgePos;
        }
        else
        {
            // Fallback: use the original method
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
        if (cc == null) return;
        float yOffset = capsuleColliderSize.y * 0.5f + 0.05f;
        Vector2 checkPos = (Vector2)transform.position + Vector2.down * yOffset;
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);
        if (hit)
        {
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            isOnSlope = slopeDownAngle > 0.1f;
        }
        else
        {
            slopeDownAngle = 0f;
            isOnSlope = false;
        }
        canWalkOnSlope = slopeDownAngle > 0.1f && slopeDownAngle <= maxSlopeAngle;
        Debug.DrawRay(hit.point, hit.normal * 0.5f, Color.green);
    }
    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, Vector2.down + Vector2.right * facingDir, slopeCheckDistance, whatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, Vector2.down - Vector2.right * facingDir, slopeCheckDistance, whatIsGround);
        if (slopeHitFront)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }
    }
    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);
        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeDownAngle != lastSlopeAngle)
            {
                isOnSlope = true;
            }
            lastSlopeAngle = slopeDownAngle;
            Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }
        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            canWalkOnSlope = false;
        }
        else
        {
            canWalkOnSlope = true;
        }
        if (isOnSlope && canWalkOnSlope && rb.linearVelocity.x >= 0.0f)
        {
            rb.sharedMaterial = fullFriction;
        }
        else
        {
            rb.sharedMaterial = noFriction;
        }
    }
    public void ApplySlopeFriction()
    {
        if (noFriction == null || fullFriction == null) return;
        var col = GetComponent<Collider2D>();
        float vy = rb.linearVelocity.y;
        // Conditions for applying slope friction
        bool slopeWalk =
        isOnSlope &&
        canWalkOnSlope &&
        groundDetected &&
        (rb.linearVelocity.y > -0.6f && rb.linearVelocity.y < 0.6f);

        bool movingDownSlope =
        isOnSlope &&
        canWalkOnSlope &&
        rb.linearVelocity.y < 0f;
        if (slopeWalk)
        {
            col.sharedMaterial = fullFriction;

            // Only kill gravity when grounded on a gentle slope and not jumping
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
        Gizmos.color = Color.green;
        //Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance));
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(primaryWallCheck.position, primaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));
        if (secondaryWallCheck != null)
            Gizmos.DrawLine(secondaryWallCheck.position, secondaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));
        Gizmos.color = Color.red;
        if (ledgeCheck != null)
            Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + new Vector3(wallCheckDistance * facingDir, 0));
        Gizmos.color = Color.magenta;
        Vector2 feetPos = (Vector2)transform.position + Vector2.down * (capsuleColliderSize.y * 0.5f + 0.05f);
        Gizmos.DrawWireSphere(feetPos, slopeOverlapRadius);
    }
}