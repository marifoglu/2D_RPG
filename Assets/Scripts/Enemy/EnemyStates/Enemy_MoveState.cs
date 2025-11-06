using UnityEngine;

public class Enemy_MoveState : Enemy_GroundedState
{
    public Enemy_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        // No immediate flip here; edge detection will trigger a flip
    }

    public override void Update()
    {
        base.Update();

        // ✅ Stop if enemy is no longer grounded
        if (!enemy.groundDetected)
        {
            enemy.SetVelocity(0f, rb.linearVelocity.y);
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        // ✅ Stop and flip if facing edge or wall
        if (enemy.wallDetected || enemy.edgeDetected)
        {
            enemy.SetVelocity(0f, rb.linearVelocity.y);
            enemy.Flip();
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        // ✅ Move safely forward (apply horizontal motion only)
        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.linearVelocity.y);
    }
}
