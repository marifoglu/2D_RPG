using UnityEngine;

public class Enemy_Slime_DeadState : Enemy_DeadState
{
    private Enemy_Slime enemySlime;

    public Enemy_Slime_DeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemySlime = enemy as Enemy_Slime;
    }

    public override void Enter()
    {
        base.Enter();

        enemySlime.CreateSlimeOnDeath();
    }
}
