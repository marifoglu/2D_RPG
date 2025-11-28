using Unity.VisualScripting;
using UnityEngine;

public class Player_IdleState : Player_GroundedState
{
    public Player_IdleState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0, 0f);
    }

    public override void Update()
    {
        base.Update();

        if (player.moveInput.x == player.facingDir && player.wallDetected && !player.groundDetected)
            return;

        if (player.moveInput.x != 0)
            stateMachine.ChangeState(player.moveState);
    }
}