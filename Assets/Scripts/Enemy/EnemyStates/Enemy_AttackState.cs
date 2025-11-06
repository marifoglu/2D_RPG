using UnityEngine;

public class Enemy_AttackState : EnemyState
{
    public Enemy_AttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName) { }

    public override void Enter()
    {
        base.Enter();

        // If edge or wall, block before  animation triggers
        if (enemy.edgeDetected || enemy.wallDetected)
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }
    }

    public override void Update()
    {
        base.Update();

        // cancel attack, if the player is gone, or we are at the edge or wall
        if (enemy.player == null || enemy.edgeDetected || enemy.wallDetected)
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        if (triggerCalled)
        {
            if (enemy.edgeDetected || enemy.wallDetected || !enemy.groundDetected)
            {
                Debug.LogWarning($"[{enemy.name}] AT EDGE/WALL after attack - Starting to roam!");
                stateMachine.ChangeState(enemy.idleState);
            }
            else
            {
                Debug.Log($"[{enemy.name}] Safe position - Back to battle.");
                stateMachine.ChangeState(enemy.battleState);
            }
        }
    }
}