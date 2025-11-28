using UnityEngine;

public class Player_GroundedState : PlayerState
{
    public Player_GroundedState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        rb.gravityScale = 0f;
    }

    public override void Update()
    {
        base.Update();

        // If the state was changed during base.Update(), bail out
        if (stateMachine.currentState != this)
            return;

        bool jumpPressed = input.PlayerCharacter.Jump.WasPressedThisFrame();
        float x = player.moveInput.x;
        float y = player.moveInput.y;

        // Check for ladder climbing - ONLY when pressing UP and can grab
        if (y > 0.5f && player.CanGrabLadder())
        {
            player.TryGrabLadder();
            return;
        }

        // Zero Y velocity when grounded and not trying to jump
        if (player.groundDetected && rb.linearVelocity.y != 0f && !jumpPressed && stateMachine.currentState != player.jumpState)
        {
            player.SetVelocity(rb.linearVelocity.x, 0f);
        }

        // if no longer grounded, fall
        if (!player.groundDetected)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }

        // Jump while grounded
        if (jumpPressed)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }

        // handle wall interaction
        if (player.wallDetected && !player.groundDetected)
            return;

        // Heavy Attack - MUST BE BEFORE BASIC ATTACK
        if (input.PlayerCharacter.HeavyAttack.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.heavyAttackState);
            return;
        }

        // Basic attack
        if (input.PlayerCharacter.Attack.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.basicAttackState);
            return;
        }

        // Counter
        if (input.PlayerCharacter.CounterAttack.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.counterAttackState);
            return;
        }

        // If there IS movement input, transition to MoveState
        if (x != 0f)
        {
            stateMachine.ChangeState(player.moveState);
        }

        if (input.PlayerCharacter.RangeAttack.WasPressedThisFrame() && skillManager.swordThrow.CanUseSkill())
            stateMachine.ChangeState(player.swordThrowState);
    }
}