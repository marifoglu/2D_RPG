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
        base.Enter();

        vfx.EnableAttackAlert(false);
        enemy.EnableCounterWindow(false);

        stateTimer = enemy.stunnedDuration;
        rb.linearVelocity = new Vector2(enemy.stunnedVelocity.x * -enemy.facingDir, enemy.stunnedVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            // Flip toward player on exit (optional but fixes facing issue)
            Transform p = enemy.GetPlayerDetection();
            if (p != null)
            {
                if ((p.position.x > enemy.transform.position.x && enemy.facingDir < 0) ||
                    (p.position.x < enemy.transform.position.x && enemy.facingDir > 0))
                {
                    enemy.Flip();
                }
            }

            stateMachine.ChangeState(enemy.idleState);
        }
    }

}
