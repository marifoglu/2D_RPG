using UnityEngine;

public class Player_CounterAttackState : PlayerState
{
    private Player_Combat combat;
    private bool counteredSomebody;
    public Player_CounterAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        combat = player.GetComponent<Player_Combat>();
    }

    override public void Enter()
    {
        base.Enter();

        stateTimer = combat.GetCounterRecoveryDuration(); // Duration of the counter-attack state
        counteredSomebody = combat.CounterAttackPerformed();
        anim.SetBool("CounterAttackPerformed", counteredSomebody);
    }

    override public void Update()
    {
        base.Update();

        //if (combat.CounterAttackPerformed())
        //{
        //    counteredSomebody = true;
        //    anim.SetBool("CounterAttackPerformed", true);
        //}

        player.SetVelocity(0, rb.linearVelocity.y);

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);


        if (stateTimer < 0f && counteredSomebody == false)
            stateMachine.ChangeState(player.idleState);

    }
}