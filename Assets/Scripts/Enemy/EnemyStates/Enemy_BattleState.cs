using UnityEngine;


public class Enemy_BattleState : EnemyState
{
    protected Transform player;
    protected Transform lastTarget;
    protected float lastTimeWasInBattle;
    protected float lastTimeAttacked = float.NegativeInfinity;


    // reduced dead-zone so quick passes are detected
    protected const float FLIP_DEAD_ZONE = 0.02f;
    protected const float MAX_CHASE_DISTANCE = 15f; // Only give up if player is REALLY far

    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        UpdateBattleTimer();

        if (player == null)
            player = enemy.player ?? enemy.GetPlayerDetection();


        if (ShouldRetreat())
        {
            ShortRetreat();
        }
    }
    protected void ShortRetreat()
    {
        float x = (enemy.retreatVelocity.x * enemy.activeSlowMultiplier) * -DirectionToPlayer();
        float y = enemy.retreatVelocity.y;

        rb.linearVelocity = new Vector2(x, y);
        enemy.HandleFlip(DirectionToPlayer());
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

        // Debug snapshot
        float dist = (player != null) ? Mathf.Abs(player.position.x - enemy.transform.position.x) : float.PositiveInfinity;
        bool rayHit = enemy.PlayerDetected().collider != null;

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
            // Player not detected in front/ check if player is behind us
            // Flip toward player when they are behind even if raycast misses (helps when player dashes past)
            if (player != null && enemy.IsPlayerBehind())
            {
                enemy.FlipTowardTarget(player);
                UpdateBattleTimer();
            }
            // Player is too far away - give up immediately
            else if (player != null && DistanceToPlayer() > MAX_CHASE_DISTANCE)
            {
                stateMachine.ChangeState(enemy.idleState);
                return;
            }
            // Battle timer expired  / give up chasing
            else if (BattleTimeOver())
            {
                stateMachine.ChangeState(enemy.idleState);
                return;
            }
        }

        // Only attack if we can actually see the player via raycast AND they're in range
        if (WithinAttackRange() && enemy.PlayerDetected() && CanAttack())
        {
            lastTimeAttacked = Time.time;
            stateMachine.ChangeState(enemy.attackState);
            return;
        }
        else
        {
            float baseSpeed = enemy.canChasePlayer ? enemy.GetBattleMoveSpeed() : 0.0001f;
            int dir = DirectionToPlayer();

            // if DirectionToPlayer() returned 0 due to tiny dead-zone but player exists and is further than a tiny epsilon,
            // force a direction so the enemy actually approaches the player.
            if (dir == 0 && player != null && DistanceToPlayer() > 0.01f)
            {
                dir = player.position.x > enemy.transform.position.x ? 1 : -1;
            }
            enemy.SetVelocity(baseSpeed * dir, rb.linearVelocity.y);
        }
    }

    protected bool CanAttack() => Time.time > lastTimeAttacked + enemy.attackCooldown;

    protected void UpdateTargetIfNeeded()
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

    protected void UpdateBattleTimer() => lastTimeWasInBattle = Time.time;
    protected bool BattleTimeOver() => Time.time >= lastTimeWasInBattle + enemy.battleTimeDuration;
    protected bool WithinAttackRange() => DistanceToPlayer() < enemy.attackDistance;

    protected bool ShouldRetreat() => DistanceToPlayer() < enemy.minRetreatDistance;

    protected float DistanceToPlayer()
    {
        if (player == null)
            return float.MaxValue;

        return Mathf.Abs(player.position.x - enemy.transform.position.x);
    }

    protected int DirectionToPlayer()
    {
        if (player == null)
            return 0;

        float distance = player.position.x - enemy.transform.position.x;

        // Use a very small dead zone; treat anything non-zero as a direction so fast passes flip
        if (Mathf.Abs(distance) < FLIP_DEAD_ZONE)
            return 0;

        return distance > 0 ? 1 : -1;
    }
}