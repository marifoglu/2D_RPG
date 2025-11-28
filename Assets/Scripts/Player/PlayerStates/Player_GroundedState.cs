//using UnityEngine;

//public class Player_GroundedState : PlayerState
//{
//    public Player_GroundedState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
//    {
//    }

//    public override void Enter()
//    {
//        base.Enter();
//        rb.gravityScale = 0f;  // Grounded gravity is controlled by Entity
//    }

//    public override void Update()
//    {
//        base.Update();

//        // If the state was changed during base.Update() (for example PlayerState handled an input
//        // and called ChangeState), bail out to avoid the previous state's logic overriding the new state.
//        if (stateMachine.currentState != this)
//            return;

//        bool jumpPressed = input.PlayerCharacter.Jump.WasPressedThisFrame();
//        float x = player.moveInput.x;
//        float y = player.moveInput.y;

//        // Zero Y velocity when grounded and not trying to jump
//        if (player.groundDetected && rb.linearVelocity.y != 0f && !jumpPressed && stateMachine.currentState != player.jumpState)
//        {
//            player.SetVelocity(rb.linearVelocity.x, 0f);
//        }

//        // if no longer grounded, fall
//        if (!player.groundDetected)
//        {
//            stateMachine.ChangeState(player.fallState);
//            return;
//        }

//        // Jump while grounded
//        if (jumpPressed)
//        {
//            stateMachine.ChangeState(player.jumpState);
//            return;
//        }

//        // handle wall interaction
//        if (player.wallDetected && !player.groundDetected)
//            return;

//        // Basic attack
//        if (input.PlayerCharacter.Attack.WasPressedThisFrame())
//        {
//            stateMachine.ChangeState(player.basicAttackState);
//            return;
//        }

//        // Counter
//        if (input.PlayerCharacter.CounterAttack.WasPressedThisFrame())
//        {
//            stateMachine.ChangeState(player.counterAttackState);
//            return;
//        }

//        // If there IS movement input, transition to MoveState
//        if (x != 0f)
//        {
//            stateMachine.ChangeState(player.moveState);
//        }

//        if (input.PlayerCharacter.RangeAttack.WasPressedThisFrame() && skillManager.swordThrow.CanUseSkill())
//            stateMachine.ChangeState(player.swordThrowState);

//    }
//}
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

        // If the state was changed during base.Update(), bail out
        if (stateMachine.currentState != this)
            return;

        bool jumpPressed = input.PlayerCharacter.Jump.WasPressedThisFrame();
        float x = player.moveInput.x;
        float y = player.moveInput.y;

        // Check for ladder climbing - ONLY when pressing UP and can grab
        // This prevents accidentally grabbing ladder when walking past it
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