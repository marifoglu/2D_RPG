using UnityEngine;

public class Enemy_Slime : Enemy, ICounterable
{
    [Header("Slime Specific Settings")]
    [SerializeField] private bool hasRecoveryAnimation = false;

    protected override void Awake()
    {
        base.Awake();
        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        battleState = new Enemy_BattleState(this, stateMachine, "battle");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");

        deadState = new Enemy_DeadState(this, stateMachine, "dead");

        anim.SetBool("hasStunRecovery", hasRecoveryAnimation);
    }


    override protected void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }
}