//using UnityEngine;

//public class Player_AiredState : PlayerState
//{
//    public Player_AiredState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
//    {
//    }

//    public override void Update()
//    {
//        base.Update();

//        if (player.moveInput.x != 0)
//            player.SetVelocity(player.moveInput.x * (player.moveSpeed * player.inAirMoveMultiplier), rb.linearVelocity.y);

//        if (input.PlayerCharacter.Attack.WasPressedThisFrame())
//            stateMachine.ChangeState(player.jumpAttackState);
//    }
//}
using UnityEngine;

public class Player_AiredState : PlayerState
{
    public Player_AiredState(Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player.moveInput.x != 0)
            player.SetVelocity(player.moveInput.x * (player.moveSpeed * player.inAirMoveMultiplier), rb.linearVelocity.y);

        if (input.PlayerCharacter.Attack.WasPressedThisFrame())
        {
            // Check if pressing UP - do up attack
            if (player.moveInput.y > 0.5f)
            {
                stateMachine.ChangeState(player.upAttackState);
            }
            // Otherwise do normal jump attack (downward)
            else
            {
                stateMachine.ChangeState(player.jumpAttackState);
            }
        }
    }
}