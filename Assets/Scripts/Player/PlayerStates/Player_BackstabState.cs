using UnityEngine;

public class Player_BackstabState : PlayerState
{
    private enum BackstabPhase
    {
        Disappearing,
        Teleporting,
        Appearing,
        Attacking,
        Recovering
    }

    private BackstabPhase currentPhase;
    private Skill_Backstab backstabSkill;

    // Phase timings
    private float disappearDuration = 0.15f;
    private float appearDuration = 0.1f;
    private float attackRecoveryDuration = 0.3f;

    public Player_BackstabState(Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        backstabSkill = skillManager.backstab;

        if (backstabSkill == null || backstabSkill.GetCurrentTarget() == null)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        // Make player invulnerable during backstab
        player.health.SetCanTakeDamage(false);

        // Stop movement
        player.SetVelocity(0, 0);

        // Start disappear phase
        currentPhase = BackstabPhase.Disappearing;
        stateTimer = disappearDuration;

        // Play disappear animation
        anim.SetTrigger("BackstabDisappear");
    }

    public override void Update()
    {
        base.Update();

        // Keep player stationary
        player.SetVelocity(0, rb.linearVelocity.y);

        switch (currentPhase)
        {
            case BackstabPhase.Disappearing:
                HandleDisappearPhase();
                break;

            case BackstabPhase.Teleporting:
                HandleTeleportPhase();
                break;

            case BackstabPhase.Appearing:
                HandleAppearPhase();
                break;

            case BackstabPhase.Attacking:
                HandleAttackPhase();
                break;

            case BackstabPhase.Recovering:
                HandleRecoveryPhase();
                break;
        }
    }

    private void HandleDisappearPhase()
    {
        if (stateTimer <= 0)
        {
            currentPhase = BackstabPhase.Teleporting;
        }
    }

    private void HandleTeleportPhase()
    {
        // Execute the teleport
        backstabSkill.ExecuteTeleport();

        // Move to appear phase
        currentPhase = BackstabPhase.Appearing;
        stateTimer = appearDuration;

        // Play appear/attack animation
        anim.SetTrigger("BackstabAppear");
    }

    private void HandleAppearPhase()
    {
        if (stateTimer <= 0)
        {
            currentPhase = BackstabPhase.Attacking;
            anim.SetTrigger("BackstabAttack");
        }
    }

    private void HandleAttackPhase()
    {
        // Wait for animation trigger to execute the attack
        if (triggerCalled)
        {
            backstabSkill.ExecuteBackstabAttack();

            currentPhase = BackstabPhase.Recovering;
            stateTimer = attackRecoveryDuration;
            triggerCalled = false;
        }
    }

    private void HandleRecoveryPhase()
    {
        if (stateTimer <= 0 || triggerCalled)
        {
            // Exit to appropriate state
            if (player.groundDetected)
            {
                stateMachine.ChangeState(player.idleState);
            }
            else
            {
                stateMachine.ChangeState(player.fallState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        // Re-enable damage
        player.health.SetCanTakeDamage(true);

        // Clear target reference
        backstabSkill?.ClearTarget();

        // Reset animation triggers
        anim.ResetTrigger("BackstabDisappear");
        anim.ResetTrigger("BackstabAppear");
        anim.ResetTrigger("BackstabAttack");
    }

    // Called from animation event when attack should deal damage
    public void OnBackstabHit()
    {
        if (currentPhase == BackstabPhase.Attacking)
        {
            backstabSkill?.ExecuteBackstabAttack();
        }
    }
}