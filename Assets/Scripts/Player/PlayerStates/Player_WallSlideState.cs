using UnityEngine;
public class Player_WallSlideState : PlayerState
{
    public Player_WallSlideState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        base.Update();
        // Apply wall slide physics (slower falling)
        player.SetVelocity(0, rb.linearVelocity.y * player.wallSlideSlowMultiplier);
        // LEDGE CLIMB - Check for up input and ledge detection
        if (player.ledgeDetected && player.moveInput.y > 0.5f)
        {
            stateMachine.ChangeState(player.ledgeClimbState);
            return;
        }
        // WALL JUMP
        if (input.PlayerCharacter.Jump.WasPerformedThisFrame())
        {
            stateMachine.ChangeState(player.wallJumpState);
            return;
        }
        // CANCEL WALL SLIDE - Move in opposite direction to fall
        if (player.moveInput.x != 0 && Mathf.Sign(player.moveInput.x) != player.facingDir)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
        // Fall off wall or reach ground
        if (player.groundDetected)
        {
            stateMachine.ChangeState(player.idleState);
        }
        else if (!player.wallDetected)
        {
            stateMachine.ChangeState(player.fallState);
        }
    }
    public override void Exit()
    {
        base.Exit();
    }
}