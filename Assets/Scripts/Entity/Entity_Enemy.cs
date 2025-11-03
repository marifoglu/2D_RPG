using System;
using System.Collections;
using UnityEngine;

public class Entity_Enemy : MonoBehaviour
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

    [Header("Edge Detection")]
    [Tooltip("Horizontal look-ahead from feet to check for ground ahead.")]
    [SerializeField] private float edgeForwardOffset = 0.5f;
    [Tooltip("Extra downward distance from the look-ahead point.")]
    [SerializeField] private float edgeDownDistance = 0.25f;
    public bool edgeDetected { get; private set; }

    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }

    // Condition Variables
    private bool isKnocked = false;
    private Coroutine knockbackCo;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine();
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
    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnocked)
            return;

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
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
        // Ground under feet
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        // Wall in front
        if (secondaryWallCheck != null)
        {
            wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround)
                           && Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        }
        else
        {
            wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        }

        // Ground ahead (front-foot probe)
        Vector2 feetPos = groundCheck != null ? (Vector2)groundCheck.position : (Vector2)transform.position;
        Vector2 frontFoot = feetPos + Vector2.right * facingDir * Mathf.Max(0.01f, edgeForwardOffset);
        float downDist = Mathf.Max(0f, groundCheckDistance + edgeDownDistance);
        bool groundAhead = Physics2D.Raycast(frontFoot, Vector2.down, downDist, whatIsGround);

        // Consider an edge only if currently grounded and no ground at front
        edgeDetected = groundDetected && !groundAhead;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (groundCheck != null)
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance));

        Gizmos.color = Color.blue;
        if (primaryWallCheck != null)
            Gizmos.DrawLine(primaryWallCheck.position, primaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));

        if (secondaryWallCheck != null)
            Gizmos.DrawLine(secondaryWallCheck.position, secondaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));

        // Visualize edge probe
        if (groundCheck != null)
        {
            Vector3 feetPos = groundCheck.position;
            Vector3 frontFoot = feetPos + Vector3.right * facingDir * Mathf.Max(0.01f, edgeForwardOffset);
            float downDist = Mathf.Max(0f, groundCheckDistance + edgeDownDistance);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(frontFoot, frontFoot + Vector3.down * downDist);
        }
    }
}