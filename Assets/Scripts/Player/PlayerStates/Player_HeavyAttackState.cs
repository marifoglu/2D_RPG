using UnityEngine;

public class Player_HeavyAttackState : PlayerState
{
    private float attackVelocityTimer;
    private float animationDuration = 1.0f; // Adjust to match your animation length

    public Player_HeavyAttackState(Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // Set state timer to animation duration as fallback
        stateTimer = animationDuration;

        SyncAttackSpeed();

        // Stop all movement when entering attack state
        player.SetVelocity(0f, 0f);

        // Define attack direction according to input
        int attackDir = player.moveInput.x != 0 ? ((int)player.moveInput.x) : player.facingDir;

        // Apply attack movement
        attackVelocityTimer = player.heavyAttackVelocityDuration;
        player.SetVelocity(player.heavyAttackVelocity.x * attackDir, player.heavyAttackVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        // Keep the player mostly stationary during attack
        attackVelocityTimer -= Time.deltaTime;

        if (attackVelocityTimer < 0)
        {
            player.SetVelocity(0, rb.linearVelocity.y);
        }

        // Fallback: Force exit if animation time runs out
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        // Primary exit: Animation event calls CurrentStateTrigger
        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        // Make sure player is stopped when exiting
        player.SetVelocity(0, rb.linearVelocity.y);
    }
}