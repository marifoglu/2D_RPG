using UnityEngine;
public class Player_JumpState : Player_AiredState
{
    public Player_JumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, player.jumpForce);
            rb.gravityScale = 1f;
    }

    public override void Update()
    {
        base.Update();
        if (rb.linearVelocity.y <= 0f && stateMachine.currentState != player.jumpAttackState)
            stateMachine.ChangeState(player.fallState);
    }

}