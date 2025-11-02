using UnityEngine;

public class Enemy_StunnedState : EnemyState
{
    private Enemy_VFX vfx;

    public Enemy_StunnedState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        vfx = enemy.GetComponent<Enemy_VFX>();
    }

    public override void Enter()
    {
        Debug.Log($"[{enemy.name}] ===== STUNNED STATE ENTERING =====");
        Debug.Log($"[{enemy.name}] Animation bool '{animBoolName}' about to be set to TRUE");

        base.Enter();

        vfx.EnableAttackAlert(false);
        enemy.EnableCounterWindow(false);

        stateTimer = enemy.stunnedDuration;
        rb.linearVelocity = new Vector2(enemy.stunnedVelocity.x * -enemy.facingDir, enemy.stunnedVelocity.y);

        Debug.Log($"[{enemy.name}] STUNNED STATE ENTERED - Timer: {stateTimer}s, Velocity: {rb.linearVelocity}");
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            Debug.Log($"[{enemy.name}] Stunned timer expired - transitioning to idle");

            // Flip toward player on exit (optional but fixes facing issue)
            Transform p = enemy.GetPlayerDetection();
            if (p != null)
            {
                if ((p.position.x > enemy.transform.position.x && enemy.facingDir < 0) ||
                    (p.position.x < enemy.transform.position.x && enemy.facingDir > 0))
                {
                    Debug.Log($"[{enemy.name}] Flipping to face player on stunned exit");
                    enemy.Flip();
                }
            }

            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        Debug.Log($"[{enemy.name}] STUNNED STATE EXITING - Animation bool '{animBoolName}' set to FALSE");
        base.Exit();
        Debug.Log($"[{enemy.name}] STUNNED STATE EXITED");
    }

}