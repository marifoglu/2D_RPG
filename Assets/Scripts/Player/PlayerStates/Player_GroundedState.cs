using UnityEngine;

public class Player_GroundedState : PlayerState
{
    public Player_GroundedState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        rb.gravityScale = 0f;  // Grounded gravity is controlled by Entity
    }

    public override void Update()
    {
        base.Update();


        bool jumpPressed = input.Player.Jump.WasPressedThisFrame();
        float x = player.moveInput.x;
        float y = player.moveInput.y;

        // ✅ ONLY zero Y velocity when grounded and not trying to jump
        if (player.groundDetected && rb.linearVelocity.y != 0f && !jumpPressed && stateMachine.currentState != player.jumpState)
        {
            player.SetVelocity(rb.linearVelocity.x, 0f);
        }

        // ✅ If no longer grounded, fall
        if (!player.groundDetected)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }

        // ✅ Jump while grounded
        if (jumpPressed)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }

        // ✅ Handle wall interaction — block ONLY while airborne and hugging wall
        if (player.wallDetected && !player.groundDetected)
            return;

        // ✅ Basic attack
        if (input.Player.Attack.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.basicAttackState);
            return;
        }

        // ✅ Counter
        if (input.Player.CounterAttack.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.counterAttackState);
            return;
        }

        // ✅ If there IS movement input, transition to MoveState
        if (x != 0f)
        {
            stateMachine.ChangeState(player.moveState);
        }
    }
}
