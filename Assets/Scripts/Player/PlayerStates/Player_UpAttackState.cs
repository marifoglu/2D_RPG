using UnityEngine;

public class Player_UpAttackState : PlayerState
{
    private float attackVelocityTimer;
    private bool hasReachedPeak;

    public Player_UpAttackState(Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        hasReachedPeak = false;

        // Stamina check (reuse jump attack cost or create a new one)
        if (!player.stamina.HasEnoughStamina(player.stamina.GetJumpAttackCost()))
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }

        // Consume stamina
        player.stamina.TryUseStamina(player.stamina.GetJumpAttackCost());

        // Apply upward attack velocity
        attackVelocityTimer = player.upAttackVelocityDuration;
        player.SetVelocity(player.upAttackVelocity.x * player.facingDir, player.upAttackVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        // Handle velocity duration
        attackVelocityTimer -= Time.deltaTime;

        if (attackVelocityTimer < 0 && !hasReachedPeak)
        {
            // Slow down horizontal movement but let gravity take over vertical
            player.SetVelocity(rb.linearVelocity.x * 0.5f, rb.linearVelocity.y);
            hasReachedPeak = true;
        }

        // Transition to fall state when animation triggers or when falling
        if (triggerCalled)
        {
            if (player.groundDetected)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.fallState);
        }

        // Safety: if we're falling and animation hasn't triggered, go to fall state
        if (rb.linearVelocity.y < -2f && hasReachedPeak)
        {
            stateMachine.ChangeState(player.fallState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}