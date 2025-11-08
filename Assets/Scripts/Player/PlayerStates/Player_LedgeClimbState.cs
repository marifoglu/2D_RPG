using UnityEngine;

public class Player_LedgeClimbState : PlayerState
{
    private Vector2 climbStartPosition;
    private Vector2 climbEndPosition;
    private float climbProgress = 0f;
    private float climbDuration = 0.45f;
    private float originalGravity;
    private bool isLedgeCached = false;
    private Vector2 cachedLedgePosition;

    public Player_LedgeClimbState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // Store original physics
        originalGravity = rb.gravityScale;

        // Stop all movement
        player.SetVelocity(0, 0);
        rb.gravityScale = 0;

        // Check and cache ledge position once
        if (!isLedgeCached)
        {
            cachedLedgePosition = player.DetermineLedgePosition();
            isLedgeCached = true;
        }

        // Calculate positions using cached ledge position
        climbStartPosition = player.transform.position;
        climbEndPosition = new Vector2(
            cachedLedgePosition.x + (1.2f * player.facingDir), // Horizontal offset
            cachedLedgePosition.y + player.GetComponent<Collider2D>().bounds.extents.y + 0.1f // Align with top of ledge
        );
        climbProgress = 0f;
    }

    public override void Update()
    {
        base.Update();

        // Animate the climb movement
        climbProgress += Time.deltaTime / climbDuration;

        if (climbProgress < 1f)
        {
            // Smooth movement from start to end position
            Vector2 newPosition = Vector2.Lerp(climbStartPosition, climbEndPosition, climbProgress);
            player.transform.position = newPosition;
        }
        else
        {
            // Climb finished
            player.transform.position = climbEndPosition;
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        // Restore physics and clear ledge cache
        rb.gravityScale = originalGravity;
        player.transform.position = new Vector2(climbEndPosition.x, climbEndPosition.y + 0.1f); // Small upward offset
        isLedgeCached = false; // Reset cache for next use
    }
}