using UnityEngine;

public class Enemy_IdleState : Enemy_GroundedState
{
    public Enemy_IdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;
        Debug.Log($"[{enemy.name}] ===== IDLE STATE ENTERED ===== Timer: {stateTimer}s");
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            Debug.Log($"[{enemy.name}] Idle complete - changing to MOVE");
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void Exit()
    {
        Debug.Log($"[{enemy.name}] ===== IDLE STATE EXITING =====");
        base.Exit();
        Debug.Log($"[{enemy.name}] ===== IDLE STATE EXITED =====");
    }
}