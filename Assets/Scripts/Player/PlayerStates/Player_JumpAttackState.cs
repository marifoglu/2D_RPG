using UnityEngine;

public class Player_JumpAttackState : PlayerState
{
    private bool touchedGround;

    public Player_JumpAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // Stamina check
        if (!player.stamina.HasEnoughStamina(player.stamina.GetJumpAttackCost()))
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }

        // Consume stamina
        player.stamina.TryUseStamina(player.stamina.GetJumpAttackCost());


        touchedGround = false;

        player.SetVelocity(player.jumpAttackVelocity.x * player.facingDir, player.jumpAttackVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        if (player.groundDetected && touchedGround == false)
        {
            touchedGround = true;
            anim.SetTrigger("JumpAttackTrigger");
            player.SetVelocity(0, rb.linearVelocity.y);
        }

        if (triggerCalled && player.groundDetected)
            stateMachine.ChangeState(player.idleState);
    }

}
