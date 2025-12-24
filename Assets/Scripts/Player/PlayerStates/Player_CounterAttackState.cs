//using UnityEngine;

//public class Player_CounterAttackState : PlayerState
//{
//    private Player_Combat combat;
//    private bool counteredSomebody;
//    public Player_CounterAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
//    {
//        combat = player.GetComponent<Player_Combat>();
//    }

//    override public void Enter()
//    {
//        base.Enter();

//        stateTimer = combat.GetCounterRecoveryDuration(); // Duration of the counter-attack state
//        counteredSomebody = combat.CounterAttackPerformed();
//        anim.SetBool("CounterAttackPerformed", counteredSomebody);
//    }

//    override public void Update()
//    {
//        base.Update();

//        //if (combat.CounterAttackPerformed())
//        //{
//        //    counteredSomebody = true;
//        //    anim.SetBool("CounterAttackPerformed", true);
//        //}

//        player.SetVelocity(0, rb.linearVelocity.y);

//        if (triggerCalled)
//            stateMachine.ChangeState(player.idleState);


//        if (stateTimer < 0f && counteredSomebody == false)
//            stateMachine.ChangeState(player.idleState);

//    }
//}

using UnityEngine;

public class Player_CounterAttackState : PlayerState
{
    private Player_Combat combat;
    private bool counteredSomebody;

    // Parry window - only this brief moment allows counter attacks
    private float parryWindowDuration = 0.3f;
    private float parryWindowTimer;
    private bool isInParryWindow;
    private bool hasTriggeredCounter; // Prevent double-triggering

    // Block settings
    private float blockStaminaDrainRate = 5f;

    public Player_CounterAttackState(Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
        combat = player.GetComponent<Player_Combat>();
    }

    public override void Enter()
    {
        base.Enter();

        // Initial stamina check
        if (!player.stamina.HasEnoughStamina(1f))
        {
            player.EnterStaggerState(true);
            return;
        }

        counteredSomebody = false;
        hasTriggeredCounter = false;

        // Start parry window
        parryWindowTimer = parryWindowDuration;
        isInParryWindow = true;

        // Set blocking flags
        player.SetBlocking(true);
        player.SetInParryWindow(true);

        stateTimer = combat.GetCounterRecoveryDuration();
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(0, rb.linearVelocity.y);

        // Update parry window timer
        if (isInParryWindow && !counteredSomebody)
        {
            parryWindowTimer -= Time.deltaTime;

            if (parryWindowTimer <= 0)
            {
                isInParryWindow = false;
                player.SetInParryWindow(false);
                Debug.Log("Parry window expired - now regular blocking");
            }
        }

        // If counter was performed, wait for animation to finish
        if (counteredSomebody)
        {
            player.SetVelocity(0, rb.linearVelocity.y);

            if (triggerCalled)
            {
                stateMachine.ChangeState(player.idleState);
            }
            return; // Don't process button checks during counter animation
        }

        // Check if still holding block button
        if (input.PlayerCharacter.CounterAttack.IsPressed())
        {
            // Drain stamina while blocking
            float staminaDrain = blockStaminaDrainRate * Time.deltaTime;

            if (!player.stamina.TryUseStamina(staminaDrain))
            {
                // Out of stamina!
                player.SetBlocking(false);
                player.SetInParryWindow(false);
                player.EnterStaggerState(true);
                return;
            }

            // Continue blocking - STAY IN THIS STATE
            // Don't exit!
        }
        else
        {
            // Button released - exit block
            player.SetBlocking(false);
            player.SetInParryWindow(false);
            stateMachine.ChangeState(player.idleState);
        }

        // REMOVED THE stateTimer CHECK - WE HOLD UNTIL BUTTON RELEASED OR STAMINA DEPLETED!
    }

    public override void Exit()
    {
        base.Exit();
        player.SetBlocking(false);
        player.SetInParryWindow(false);
        anim.SetBool("CounterAttackPerformed", false);
    }

    // Called when hit during parry window - THE ORIGINAL WORKING LOGIC
    public void TriggerCounterAttack()
    {
        if (!isInParryWindow || counteredSomebody || hasTriggeredCounter)
            return;

        hasTriggeredCounter = true;

        // THIS IS THE ORIGINAL WORKING CODE - IT IMMEDIATELY DAMAGES ENEMIES
        counteredSomebody = combat.CounterAttackPerformed();
        anim.SetBool("CounterAttackPerformed", counteredSomebody);

        if (counteredSomebody)
        {
            Debug.Log("PERFECT PARRY - Enemy damaged!");
            player.SetBlocking(false);
            isInParryWindow = false;
            player.SetInParryWindow(false);
        }
    }

    public bool IsInParryWindow()
    {
        return isInParryWindow;
    }
}