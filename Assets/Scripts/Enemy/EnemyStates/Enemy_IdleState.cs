using UnityEngine;

public class Enemy_IdleState : Enemy_GroundedState
{
    public Enemy_IdleState(Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.idleTime;
        Debug.Log($"[{enemy.name}] ENTER IdleState | Timer: {stateTimer:F2}");
    }

    public override void Update()
    {
        base.Update();

        if (enemy.edgeDetected)
        {
            Debug.LogWarning($"[{enemy.name}] STILL AT EDGE in IdleState. Flipping...");
            enemy.Flip(); // turn around
            return;
        }

        if (stateTimer <= 0)
        {
            Debug.Log($"[{enemy.name}] Idle timer finished. Switching to MoveState.");
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void Exit()
    {
        Debug.Log($"[{enemy.name}] EXIT IdleState");
        base.Exit();
    }
}
