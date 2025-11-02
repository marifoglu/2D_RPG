using UnityEngine;

public class Enemy_AttackState : EnemyState
{
    public Enemy_AttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log($"[{enemy.name}] ===== ATTACK STATE ENTERED ===== Animation: '{animBoolName}' = TRUE");
    }


    public override void Update()
    {
        base.Update();

        if (triggerCalled)
        {
            Debug.Log($"[{enemy.name}] Attack trigger called - returning to battle");
            stateMachine.ChangeState(enemy.battleState);
        }
    }

    public override void Exit()
    {
        Debug.Log($"[{enemy.name}] ===== ATTACK STATE EXITING ===== Animation: '{animBoolName}' = FALSE");
        base.Exit();
        Debug.Log($"[{enemy.name}] ===== ATTACK STATE EXITED =====");
    }

}