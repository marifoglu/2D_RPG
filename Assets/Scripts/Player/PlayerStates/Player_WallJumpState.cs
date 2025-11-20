using UnityEngine;

public class Player_WallJumpState : PlayerState
{
    private float wallJumpTimer = 0f;

    public Player_WallJumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        wallJumpTimer = 0f;
        player.SetVelocity(player.wallJumpForce.x * -player.facingDir, player.wallJumpForce.y);
    }

    public override void Update()
    {
        base.Update();
        wallJumpTimer += Time.deltaTime;

        // Always go to fall state when moving downward
        if (rb.linearVelocity.y < 0)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}