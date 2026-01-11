using UnityEngine;

public class Enemy_AttackState : EnemyState
{
    private const float CONTINUE_CHASE_DISTANCE = 10f; // Keep chasing if player within this distance after attack

    public Enemy_AttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        SyncAttackSpeed();

        // Stop all movement when entering attack state
        enemy.SetVelocity(0f, 0f);

        // If edge or wall, block before animation triggers
        if (enemy.edgeDetected || enemy.wallDetected)
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }
    }

    public override void Update()
    {
        base.Update();

        // Keep the enemy stationary during attack
        enemy.SetVelocity(0f, rb.linearVelocity.y);

        // Cancel attack if player reference is completely gone, or we are at edge/wall
        if (enemy.player == null || enemy.edgeDetected || enemy.wallDetected)
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        if (triggerCalled)
        {
            // After attack completes, check conditions before deciding next state
            if (enemy.edgeDetected || enemy.wallDetected || !enemy.groundDetected)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
            // If we can still see player via raycast, definitely continue battle
            else if (enemy.PlayerDetected())
            {
                stateMachine.ChangeState(enemy.battleState);
            }
            // Player not in raycast, but check if they're still nearby (dashed/jumped)
            else if (enemy.player != null && IsPlayerNearby())
            {
                // Player escaped but is still close - continue chasing
                stateMachine.ChangeState(enemy.battleState);
            }
            else
            {
                // Player is truly gone - give up
                stateMachine.ChangeState(enemy.idleState);
            }
        }
    }


    private bool IsPlayerNearby()
    {
        if (enemy.player == null)
            return false;

        float distance = Mathf.Abs(enemy.player.position.x - enemy.transform.position.x);
        return distance < CONTINUE_CHASE_DISTANCE;
    }
}