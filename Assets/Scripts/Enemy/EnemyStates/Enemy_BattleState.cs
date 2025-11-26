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
        else if (BattleTimeOver())
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        if (WithinAttackRange() && enemy.PlayerDetected())
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