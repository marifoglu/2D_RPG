using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private Transform player;
    private Transform lastTarget;
    private float lastTimeWasInBattle;
    private const float FLIP_DEAD_ZONE = 0.1f;
    private const float MAX_CHASE_DISTANCE = 15f; // Only give up if player is REALLY far

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

    public override void Exit()
    {
        base.Exit();
        // Clear player reference when leaving battle state
        player = null;
        lastTarget = null;
    }

    public override void Update()
    {
        base.Update();

        // Stop if at edge or no ground
        if (enemy.edgeDetected || !enemy.groundDetected)
        {
            enemy.SetVelocity(0f, rb.linearVelocity.y);
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        // Update target tracking - ONLY reset timer when we actually SEE the player
        if (enemy.PlayerDetected())
        {
            UpdateTargetIfNeeded();
            UpdateBattleTimer(); // Only reset timer when raycast sees player
        }
        else
        {
            // Player not detected in front - check if player is behind us
            if (player != null && enemy.IsPlayerBehind() && DistanceToPlayer() < enemy.attackDistance * 2f)
            {
                // Player is close behind us, flip to face them
                enemy.FlipTowardTarget(player);
                UpdateBattleTimer();
            }
            // Player is too far away - give up immediately
            else if (player != null && DistanceToPlayer() > MAX_CHASE_DISTANCE)
            {
                stateMachine.ChangeState(enemy.idleState);
                return;
            }
            // Battle timer expired - give up chasing
            else if (BattleTimeOver())
            {
                stateMachine.ChangeState(enemy.idleState);
                return;
            }
            // Otherwise keep chasing (player dashed/jumped but still in range)
        }

        // Only attack if we can actually see the player via raycast AND they're in range
        if (WithinAttackRange() && enemy.PlayerDetected())
        {
            stateMachine.ChangeState(enemy.attackState);
            return;
        }

        int dir = DirectToPlayer();

        // Keep chasing toward last known player position
        if (dir != 0 && player != null)
        {
            enemy.SetVelocity(enemy.GetBattleMoveSpeed() * dir, rb.linearVelocity.y);
        }
        else
        {
            enemy.SetVelocity(0f, rb.linearVelocity.y);
        }
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