using System;
using System.Collections;
using UnityEngine;

public class Entitiy : MonoBehaviour
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

    [Header("Ledge Climb Details")]
    [SerializeField] private Vector2 ledgeClimbOffset;

    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }
    public bool ledgeDetected { get; private set; }

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
        if(knockbackCo != null)
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

        if(isKnocked)
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
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

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

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance));

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(primaryWallCheck.position, primaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));

        if (secondaryWallCheck != null)
            Gizmos.DrawLine(secondaryWallCheck.position, secondaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));

        Gizmos.color = Color.red;
        if (ledgeCheck != null)
            Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + new Vector3(wallCheckDistance * facingDir, 0));
    }
}