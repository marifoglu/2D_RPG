using UnityEngine;

public class Player_FallState : Player_AiredState
{
    private float wallJumpGraceTimer = 0f;
    private float wallJumpGracePeriod = 0.15f;
    private bool justFromWallJump = false;

    public Player_FallState(Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName) { }

    public override void Enter()
    {
        base.Enter();

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

        // Check for ladder grab while falling - WITH POSITION CHECK
        if (player.CanGrabLadder())
        {
            Ladder currentLadder = player.GetCurrentLadder();
            if (currentLadder != null)
            {
                Vector2 ladderTop = currentLadder.GetTopPosition();

                // Only allow grab if player is BELOW ladder top
                if (player.transform.position.y <= ladderTop.y + 0.5f)
                {
                    // Player is at or below ladder top, allow grab
                    if (player.moveInput.y > 0.1f || Mathf.Abs(player.moveInput.y) < 0.1f)
                    {
                        player.TryGrabLadder();
                        return;
                    }
                }
            }
        }

        if (player.ledgeDetected && player.moveInput.y > 0.2f)
        {
            stateMachine.ChangeState(player.ledgeClimbState);
            return;
        }

        // ADDITIONAL: Allow ledge climb when pressing UP + RIGHT/LEFT toward the wall
        if (player.ledgeDetected && player.moveInput.y > 0.2f && player.moveInput.x != 0 )
        {
            // Check if moving toward the wall (same direction as facing)
            if (Mathf.Sign(player.moveInput.x) == player.facingDir)
            {
                player.SetLedgeClimbCooldown();
                stateMachine.ChangeState(player.ledgeClimbState);
                return;
            }
        }

        if (player.groundDetected)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }


        // Wall detection logic
        if (player.wallDetected && !player.groundDetected)
        {
            bool holdingWallDirection = player.moveInput.x != 0 && Mathf.Sign(player.moveInput.x) == player.facingDir;

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