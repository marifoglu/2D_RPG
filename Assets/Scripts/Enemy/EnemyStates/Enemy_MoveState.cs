using UnityEngine;

public class Enemy_MoveState : Enemy_GroundedState
{
    public Enemy_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    override public void Enter()
    {
        base.Enter();
        // Do not flip on Enter; wait for a confirmed edge/wall in Update
    }

    override public void Update()
    {
        base.Update();

        // If we left the ground, stop and idle
        if (!enemy.groundDetected)
        {
            enemy.SetVelocity(0f, rb.linearVelocity.y);
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        // If a wall or edge is ahead, stop, flip once, and idle
        if (enemy.wallDetected || enemy.edgeDetected)
        {
            enemy.SetVelocity(0f, rb.linearVelocity.y);
            enemy.Flip();
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        // Walk forward safely
        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.linearVelocity.y);
    }
}