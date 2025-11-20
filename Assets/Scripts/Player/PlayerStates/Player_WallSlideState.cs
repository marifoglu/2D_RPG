using UnityEngine;

public class Player_WallSlideState : PlayerState
{
    private float wallCheckBuffer = 0.1f;
    private float wallLostTimer;
    private float wallJumpGraceTimer = 0f;
    private float wallJumpGracePeriod = 0.2f; // Grace period when entering from wall jump

    public Player_WallSlideState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        wallLostTimer = 0f;

        // Check if we entered from a wall jump (velocity opposite to facing direction)
        bool fromWallJump = Mathf.Abs(rb.linearVelocity.x) > 1f && Mathf.Sign(rb.linearVelocity.x) != player.facingDir;

        if (fromWallJump)
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

        // If grounded, exit immediately
        if (player.groundDetected)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        // Apply wall slide physics
        player.SetVelocity(0, rb.linearVelocity.y * player.wallSlideSlowMultiplier);

        // WALL JUMP - Highest priority
        if (input.PlayerCharacter.Jump.WasPerformedThisFrame())
        {
            stateMachine.ChangeState(player.wallJumpState);
            return;
        }

        // LEDGE CLIMB - Only when specifically pressing up WITHOUT opposing horizontal input
        if (player.ledgeDetected && player.moveInput.y > 0.5f)
        {
            // Allow ledge climb if pure up input OR up + correct wall direction
            bool pureUpInput = Mathf.Abs(player.moveInput.x) < 0.1f;
            bool upWithWallDirection = player.moveInput.x != 0 && Mathf.Sign(player.moveInput.x) == player.facingDir;

            if (pureUpInput || upWithWallDirection)
            {
                stateMachine.ChangeState(player.ledgeClimbState);
                return;
            }
        }

        // Check for valid wall slide inputs
        bool holdingWallDirection = player.moveInput.x != 0 && Mathf.Sign(player.moveInput.x) == player.facingDir;
        bool holdingUpOnly = player.moveInput.y > 0.1f && Mathf.Abs(player.moveInput.x) < 0.1f;
        bool pushingAwayFromWall = player.moveInput.x != 0 && Mathf.Sign(player.moveInput.x) != player.facingDir;

        // Stay on wall if holding correct direction OR just holding up
        bool shouldStayOnWall = holdingWallDirection || holdingUpOnly || wallJumpGraceTimer > 0f;

        // ONLY exit if actively pushing away OR completely no input (and not in grace period)
        if (pushingAwayFromWall)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
        else if (!shouldStayOnWall)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }

        // Decrease grace timer
        if (wallJumpGraceTimer > 0f)
        {
            wallJumpGraceTimer -= Time.deltaTime;
        }

        // Wall lost detection
        if (!player.wallDetected)
        {
            wallLostTimer += Time.deltaTime;
            if (wallLostTimer >= wallCheckBuffer)
            {
                stateMachine.ChangeState(player.fallState);
            }
        }
        else
        {
            wallLostTimer = 0f;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}