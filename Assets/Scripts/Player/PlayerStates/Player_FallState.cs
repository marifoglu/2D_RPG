using UnityEngine;

public class Player_FallState : Player_AiredState
{
    public Player_FallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        // LEDGE CLIMB from falling near a wall
        if (player.ledgeDetected && player.moveInput.y > 0.2f)
        {
            stateMachine.ChangeState(player.ledgeClimbState);
            return;
        }

        if (player.groundDetected)
        {
            stateMachine.ChangeState(player.idleState);
        }
        else if (player.wallDetected)
        {
            stateMachine.ChangeState(player.wallSlideState);
        }
    }
}