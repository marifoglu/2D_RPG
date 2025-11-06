using UnityEngine;

public class Enemy_MoveState : Enemy_GroundedState
{
    // small grace after flipping
    private float edgeGraceTimer;

    public Enemy_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        // allow a brief step away from the edge after entering move
        edgeGraceTimer = 0.18f; 
    }

    public override void Update()
    {
        base.Update();

        // Always fall back to Idle if lose ground
        if (!enemy.groundDetected)
        {
            enemy.SetVelocity(0f, rb.linearVelocity.y);
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        // While grace timer is active, ignore all edge/wall checks,
        if (edgeGraceTimer > 0f)
        {
            edgeGraceTimer -= Time.deltaTime;
            enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.linearVelocity.y);
            return;
        }

        // use your normal safe guards
        if (enemy.wallDetected || enemy.edgeDetected)
        {
            enemy.SetVelocity(0f, rb.linearVelocity.y);
            enemy.Flip();
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        // Normal roam
        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.linearVelocity.y);
    }
}
