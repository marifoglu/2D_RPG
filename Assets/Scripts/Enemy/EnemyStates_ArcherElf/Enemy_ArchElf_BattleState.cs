//using UnityEngine;

//public class Enemy_ArchElf_BattleState : Enemy_BattleState
//{
//    private bool canFlip;
//    private bool reachedDeadEnd;

//    public Enemy_ArchElf_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
//    {
//    }
//    public override void Enter()
//    {
//        base.Enter();
//        reachedDeadEnd = false;
//    }
//    override public void Update()
//    {
//        stateTimer -= Time.deltaTime;
//        UpdateAnimationParameters();

//        // Stop if at edge or no ground
//        if (enemy.edgeDetected || !enemy.groundDetected)
//        {
//            enemy.SetVelocity(0f, rb.linearVelocity.y);
//            stateMachine.ChangeState(enemy.idleState);
//            return;
//        }

//        if (enemy.PlayerDetected())
//        {
//            UpdateTargetIfNeeded();
//            UpdateBattleTimer();
//        }
//        else
//        {
//            if (player != null && enemy.IsPlayerBehind() && DistanceToPlayer() < enemy.attackDistance * 2f)
//            {
//                enemy.FlipTowardTarget(player);
//                UpdateBattleTimer();
//            }
//            else if (player != null && DistanceToPlayer() > MAX_CHASE_DISTANCE)
//            {
//                stateMachine.ChangeState(enemy.idleState);
//                return;
//            }
//            else if (BattleTimeOver())
//            {
//                stateMachine.ChangeState(enemy.idleState);
//                return;
//            }
//        }

//        // REMOVED: Duplicate BattleTimeOver() check that was here

//        if (CanAttack())
//        {
//            if (enemy.PlayerDetected() == false && canFlip)
//            {
//                enemy.HandleFlip(DirectionToPlayer());
//                canFlip = false;
//            }

//            enemy.SetVelocity(0, rb.linearVelocity.y);

//            if (WithinAttackRange() && enemy.PlayerDetected())
//            {
//                canFlip = true;
//                lastTimeAttacked = Time.time;
//                stateMachine.ChangeState(enemy.attackState);
//                return;
//            }
//        }
//        else
//        {
//            // Archer Elf specific behavior: keep moving even if can't chase player
//            float xVelocity = enemy.GetBattleMoveSpeed() * -1;
//            if (enemy.groundDetected)
//                enemy.SetVelocity(xVelocity * DirectionToPlayer(), rb.linearVelocity.y);
//        }
//    }
//}

using UnityEngine;


public class Enemy_ArchElf_BattleState : Enemy_BattleState
{
    private bool canFlip;
    private bool reachedDeadEnd;

    public Enemy_ArchElf_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        reachedDeadEnd = false;
    }
    override public void Update()
    {
        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();

        // Stop if at edge or no ground
        if (enemy.edgeDetected || !enemy.groundDetected)
        {
            enemy.SetVelocity(0f, rb.linearVelocity.y);
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        if (enemy.PlayerDetected())
        {
            UpdateTargetIfNeeded();
            UpdateBattleTimer();
        }
        else
        {
            if (player != null && enemy.IsPlayerBehind() && DistanceToPlayer() < enemy.attackDistance * 2f)
            {
                enemy.FlipTowardTarget(player);
                UpdateBattleTimer();
            }
            else if (player != null && DistanceToPlayer() > MAX_CHASE_DISTANCE)
            {
                stateMachine.ChangeState(enemy.idleState);
                return;
            }
            else if (BattleTimeOver())
            {
                stateMachine.ChangeState(enemy.idleState);
                return;
            }
        }

        // Determine whether player is currently detected by raycast
        RaycastHit2D hit = enemy.PlayerDetected();
        bool playerDetectedNow = hit.collider != null;

        // If we previously set canFlip (after an attack) and now can't see player, do the flip once
        if (!playerDetectedNow && canFlip)
        {
            enemy.HandleFlip(DirectionToPlayer());
            canFlip = false;
        }

        // If within attack range, visible and attack cooldown expired -> attack
        if (WithinAttackRange() && playerDetectedNow && CanAttack())
        {
            enemy.SetVelocity(0, rb.linearVelocity.y);
            canFlip = true;
            lastTimeAttacked = Time.time;
            stateMachine.ChangeState(enemy.attackState);
            return;
        }

        // Otherwise chase the player (allow chasing while waiting for cooldown or not yet in range)
        float speed = enemy.canChasePlayer ? enemy.GetBattleMoveSpeed() : 0.0001f;
        int dir = DirectionToPlayer();

        // Fallback: if DirectionToPlayer() returned 0 due to FLIP_DEAD_ZONE but player exists and is further than tiny epsilon,
        // force a direction so the enemy actually approaches the player.
        if (dir == 0 && player != null && DistanceToPlayer() > 0.15f)
            dir = player.position.x > enemy.transform.position.x ? 1 : -1;

        if (enemy.groundDetected)
            enemy.SetVelocity(speed * dir, rb.linearVelocity.y);
    }
}