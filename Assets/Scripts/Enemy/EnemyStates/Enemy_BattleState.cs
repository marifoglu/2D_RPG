//using UnityEngine;

//public class Enemy_BattleState : EnemyState
//{
//    private Transform player;
//    private Transform lastTarget;
//    private float lastTimeWasInBattle;
//    private const float FLIP_DEAD_ZONE = 0.1f;

//    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
//    {
//    }

//    public override void Enter()
//    {
//        base.Enter();

//        UpdateBattleTimer();

//        if (player == null)
//            player = enemy.GetPlayerDetection();

//        if (ShouldRetreat())
//        {
//            rb.linearVelocity = new Vector2((enemy.retreatVelocity.x * enemy.activeSlowMultiplier) * -DirectToPlayer(), enemy.retreatVelocity.y);
//            enemy.HandleFlip(DirectToPlayer());
//        }
//    }

//    public override void Update()
//    {
//        base.Update();

//        // stop there mate, if edge or no ground
//        if (enemy.edgeDetected || !enemy.groundDetected)
//        {
//            enemy.SetVelocity(0f, rb.linearVelocity.y);
//            stateMachine.ChangeState(enemy.idleState);
//            return;
//        }

//        // Update target tracking
//        if (enemy.PlayerDetected())
//        {
//            UpdateTargetIfNeeded();
//            UpdateBattleTimer();
//        }
//        else if (BattleTimeOver())
//        {
//            stateMachine.ChangeState(enemy.idleState);
//            return;
//        }

//        if (WithinAttackRange() && enemy.PlayerDetected())
//        {
//            stateMachine.ChangeState(enemy.attackState);
//            return;
//        }

//        int dir = DirectToPlayer();

//        enemy.SetVelocity(enemy.GetBattleMoveSpeed() * dir, rb.linearVelocity.y);
//    }

//    private void UpdateTargetIfNeeded()
//    {
//        if (enemy.PlayerDetected() == false)
//            return;

//        Transform newTarget = enemy.PlayerDetected().transform;

//        if (newTarget != lastTarget)
//        {
//            lastTarget = newTarget;
//            player = newTarget;
//        }
//    }

//    private void UpdateBattleTimer() => lastTimeWasInBattle = Time.time;
//    private bool BattleTimeOver() => Time.time >= lastTimeWasInBattle + enemy.battleTimeDuration;
//    private bool WithinAttackRange() => DistanceToPlayer() < enemy.attackDistance;

//    private bool ShouldRetreat() => DistanceToPlayer() < enemy.minRetreatDistance;

//    private float DistanceToPlayer()
//    {
//        if (player == null)
//            return float.MaxValue;

//        return Mathf.Abs(player.position.x - enemy.transform.position.x);
//    }

//    private int DirectToPlayer()
//    {
//        if (player == null)
//            return 0;

//        float distance = player.position.x - enemy.transform.position.x;

//        if (Mathf.Abs(distance) < FLIP_DEAD_ZONE)
//            return 0; // too close - don't flip

//        return distance > 0 ? 1 : -1;
//    }
//}

using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private Transform player;
    private Transform lastTarget;
    private float lastTimeWasInBattle;
    private const float FLIP_DEAD_ZONE = 0.1f;

    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        UpdateBattleTimer();

        if (player == null)
            player = enemy.GetPlayerDetection();

        // Ensure we're facing the player when entering battle
        if (player != null)
        {
            enemy.FlipTowardTarget(player);
        }

        if (ShouldRetreat())
        {
            rb.linearVelocity = new Vector2((enemy.retreatVelocity.x * enemy.activeSlowMultiplier) * -DirectToPlayer(), enemy.retreatVelocity.y);
            enemy.HandleFlip(DirectToPlayer());
        }
    }

    public override void Update()
    {
        base.Update();

        // stop there mate, if edge or no ground
        if (enemy.edgeDetected || !enemy.groundDetected)
        {
            enemy.SetVelocity(0f, rb.linearVelocity.y);
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        // Update target tracking
        if (enemy.PlayerDetected())
        {
            UpdateTargetIfNeeded();
            UpdateBattleTimer();
        }
        else
        {
            // Player not detected in front - check if player is behind us
            if (player != null && enemy.IsPlayerBehind())
            {
                // Player is behind us, flip to face them
                enemy.FlipTowardTarget(player);
                UpdateBattleTimer();
            }
            else if (player != null && IsPlayerStillValid())
            {
                // Player reference exists and is valid, keep tracking
                UpdateBattleTimer();
            }
            else if (BattleTimeOver())
            {
                stateMachine.ChangeState(enemy.idleState);
                return;
            }
        }

        if (WithinAttackRange() && CanSeePlayer())
        {
            stateMachine.ChangeState(enemy.attackState);
            return;
        }

        int dir = DirectToPlayer();

        enemy.SetVelocity(enemy.GetBattleMoveSpeed() * dir, rb.linearVelocity.y);
    }

    private void UpdateTargetIfNeeded()
    {
        if (enemy.PlayerDetected() == false)
            return;

        Transform newTarget = enemy.PlayerDetected().transform;

        if (newTarget != lastTarget)
        {
            lastTarget = newTarget;
            player = newTarget;
        }
    }

    /// <summary>
    /// Checks if the player reference is still valid (not null and player is alive)
    /// </summary>
    private bool IsPlayerStillValid()
    {
        if (player == null) return false;

        // Check if player object still exists and is within reasonable distance
        float distance = DistanceToPlayer();
        return distance < 20f; // Arbitrary max tracking distance
    }

    /// <summary>
    /// Checks if enemy can currently see the player (either via raycast or player is in front)
    /// </summary>
    private bool CanSeePlayer()
    {
        if (enemy.PlayerDetected()) return true;

        // If we have a player reference and they're in front of us
        if (player != null)
        {
            float dirToPlayer = player.position.x - enemy.transform.position.x;
            bool playerInFront = (dirToPlayer > 0 && enemy.facingDir > 0) || (dirToPlayer < 0 && enemy.facingDir < 0);
            return playerInFront;
        }

        return false;
    }

    private void UpdateBattleTimer() => lastTimeWasInBattle = Time.time;
    private bool BattleTimeOver() => Time.time >= lastTimeWasInBattle + enemy.battleTimeDuration;
    private bool WithinAttackRange() => DistanceToPlayer() < enemy.attackDistance;

    private bool ShouldRetreat() => DistanceToPlayer() < enemy.minRetreatDistance;

    private float DistanceToPlayer()
    {
        if (player == null)
            return float.MaxValue;

        return Mathf.Abs(player.position.x - enemy.transform.position.x);
    }

    private int DirectToPlayer()
    {
        if (player == null)
            return 0;

        float distance = player.position.x - enemy.transform.position.x;

        if (Mathf.Abs(distance) < FLIP_DEAD_ZONE)
            return 0; // too close - don't flip

        return distance > 0 ? 1 : -1;
    }
}