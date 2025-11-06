using UnityEngine;

public class Enemy_GroundedState : EnemyState
{
    public Enemy_GroundedState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        // Edge/wall safety takes precedence over player detection
        if (enemy.edgeDetected || !enemy.groundDetected || enemy.wallDetected)
        {
            // Stay roaming state, don't chase
            return;
        }

        // check for player if we're in a safe position
        if (enemy.PlayerDetected() == true)
            stateMachine.ChangeState(enemy.battleState);
    }
}