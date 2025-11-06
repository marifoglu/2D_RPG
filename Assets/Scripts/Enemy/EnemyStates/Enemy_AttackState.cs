using UnityEngine;

public class Enemy_AttackState : EnemyState
{
    public Enemy_AttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName) { }

    public override void Enter()
    {
        base.Enter();

        // If edge or wall — block BEFORE animation triggers the attack event
        if (enemy.edgeDetected || enemy.wallDetected)
        {
            Debug.LogWarning($"[{enemy.name}] BLOCKED AttackState - unsafe position, going back to idle");
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        // Any normal initialization logic here...
    }


    public override void Update()
    {
        base.Update();

        // If the player is gone, or we are at the edge or wall — cancel attack
        if (enemy.player == null || enemy.edgeDetected || enemy.wallDetected)
        {
            Debug.LogWarning($"[{enemy.name}] EXIT AttackState (unsafe or no player)");
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        if (triggerCalled)
        {
            Debug.Log($"[{enemy.name}] Attack animation finished.");

            // ✅ FIXED: Check if safe to continue battle, otherwise start roaming
            if (enemy.edgeDetected || enemy.wallDetected || !enemy.groundDetected)
            {
                Debug.LogWarning($"[{enemy.name}] AT EDGE/WALL after attack - Starting to roam!");
                stateMachine.ChangeState(enemy.idleState); // Start roaming
            }
            else
            {
                Debug.Log($"[{enemy.name}] Safe position - Back to battle.");
                stateMachine.ChangeState(enemy.battleState);
            }
        }
    }
}

//Working