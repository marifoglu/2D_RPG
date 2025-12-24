using UnityEngine;

public class Player_StaggerState : PlayerState
{
    private float normalStaggerDuration = 1f;
    private float staminaDepletedStaggerDuration = 2f; // Longer stagger when stamina runs out
    private bool isStaminaDepleted;

    public Player_StaggerState(Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // Disable input during stagger
        input.Disable();

        // Stop all movement
        player.SetVelocity(0, rb.linearVelocity.y);

        // Set stagger duration based on cause
        stateTimer = isStaminaDepleted ? staminaDepletedStaggerDuration : normalStaggerDuration;

        // PLAYER CAN TAKE DAMAGE DURING STAGGER - They're vulnerable!
        player.health.SetCanTakeDamage(true);

        // Make sure blocking is disabled
        player.SetBlocking(false);
        player.SetInParryWindow(false);

        // Visual feedback
        Debug.Log(isStaminaDepleted ? "STAMINA DEPLETED - Long Stagger! VULNERABLE!" : "Normal Stagger - Vulnerable!");
    }

    public override void Update()
    {
        base.Update();

        // Keep player stationary (but they can still be hit)
        player.SetVelocity(0, rb.linearVelocity.y);

        // If player takes damage during stagger, we might want to reset the timer (optional)
        // This makes consecutive hits keep them staggered longer
        // Uncomment the following if you want this behavior:
        /*
        if (triggerCalled) // If hit during stagger
        {
            stateTimer = 0.5f; // Reset to small duration
        }
        */

        // Exit when timer runs out
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        // Re-enable input
        input.Enable();

        // Make sure damage is enabled (should already be)
        player.health.SetCanTakeDamage(true);

        // Reset stamina depleted flag
        isStaminaDepleted = false;
    }

    // Set whether this is a stamina-depleted stagger
    public void SetStaminaDepleted(bool depleted)
    {
        isStaminaDepleted = depleted;
    }
}