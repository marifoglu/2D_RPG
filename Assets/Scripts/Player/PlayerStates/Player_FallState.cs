using UnityEngine;

public class Player_FallState : Player_AiredState
{
    private float wallJumpGraceTimer = 0f;
    private float wallJumpGracePeriod = 0.15f; // Short grace period for wall jump recovery
    private bool justFromWallJump = false;

    public Player_FallState(Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName) { }

    public override void Enter()
    {
        base.Enter();

        // Check if we just came from a wall jump by checking the previous state
        // We can detect this by seeing if we have horizontal velocity opposite to facing direction
        justFromWallJump = Mathf.Abs(rb.linearVelocity.x) > 1f && Mathf.Sign(rb.linearVelocity.x) != player.facingDir;

        if (justFromWallJump)
        {
            wallJumpGraceTimer = wallJumpGracePeriod;
        }
        else
        {
            wallJumpGraceTimer = 0f;
        }
    }

    public override void Update()
    {
        base.Update();

        if (player.ledgeDetected && player.moveInput.y > 0.2f)
        {
            stateMachine.ChangeState(player.ledgeClimbState);
            return;
        }

        if (player.groundDetected)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        // Wall detection logic
        if (player.wallDetected && !player.groundDetected)
        {
            // UPDATED: Check for wall direction input (including with up button)
            bool holdingWallDirection = player.moveInput.x != 0 && Mathf.Sign(player.moveInput.x) == player.facingDir;

            // Enter wall slide if:
            // 1. Holding correct direction (even with up button), OR
            // 2. Just from wall jump and still in grace period
            if (holdingWallDirection)
            {
                stateMachine.ChangeState(player.wallSlideState);
                return;
            }
            else if (justFromWallJump && wallJumpGraceTimer > 0f)
            {
                stateMachine.ChangeState(player.wallSlideState);
                return;
            }
            else
            {
            }
        }

        // Decrease grace timer
        if (wallJumpGraceTimer > 0f)
        {
            wallJumpGraceTimer -= Time.deltaTime;
            if (wallJumpGraceTimer <= 0f)
            {
                justFromWallJump = false;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}