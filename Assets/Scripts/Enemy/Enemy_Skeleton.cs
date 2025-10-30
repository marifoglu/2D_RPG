using UnityEngine;

public class Enemy_Skeleton : Enemy, ICounterable
{
    public bool CanBeCountered { get => canBeStunned;  }

    protected override void Awake()
    {
        base.Awake();
        idleState = new Enemy_IdleState(this, stateMachine, "idle"); 
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        battleState = new Enemy_BattleState(this, stateMachine, "battle");
        deadState = new Enemy_DeadState(this, stateMachine, "dead");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");
    }

    override protected void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    public void HandleCounter()
    {
        if(CanBeCountered == false)
            return;

        stateMachine.ChangeState(stunnedState);
    }
}
