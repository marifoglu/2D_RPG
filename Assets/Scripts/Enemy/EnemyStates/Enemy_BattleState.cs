using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private float lastTimeWasInBattle;
    private const float FLIP_DEAD_ZONE = 0.1f;

    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        UpdateBattleTimer();

        // Use the Enemy's player reference (set by TryEnterBattleState) as the source of truth.
        if (enemy.player == null)
        {
            var hit = enemy.PlayerDetected();
            if (hit.collider != null)
            {
                enemy.TryEnterBattleState(hit.transform); // use method (setter is inaccessible)
            }
        }

        // Optional: small retreat if too close on enter
        if (ShouldRetreat())
        {
            rb.linearVelocity = new Vector2(enemy.retreatVelocity.x * -DirectToPlayer(), enemy.retreatVelocity.y);
            enemy.HandleFlip(DirectToPlayer());
        }
    }

    public override void Update()
    {
        base.Update();

        // 🚫 STOP if edge or no ground
        if (enemy.edgeDetected || !enemy.groundDetected)
        {
            Debug.LogWarning($"[{enemy.name}] Stopped chasing due to edge/no ground. Roaming.");
            enemy.SetVelocity(0f, rb.linearVelocity.y);
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        // ✅ Normal battle logic below — only runs when we're safe
        bool playerDetected = enemy.PlayerDetected();

        if (playerDetected)
            UpdateBattleTimer();
        else if (BattleTimeOver())
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        if (WithinAttackRange() && playerDetected)
        {
            stateMachine.ChangeState(enemy.attackState);
            return;
        }

        int dir = DirectToPlayer();
        enemy.SetVelocity(enemy.battleMoveSpeed * dir, rb.linearVelocity.y);
    }


    private void UpdateBattleTimer() => lastTimeWasInBattle = Time.time;
    private bool BattleTimeOver() => Time.time >= lastTimeWasInBattle + enemy.battleTimeDuration;
    private bool WithinAttackRange() => DistanceToPlayer() < enemy.attackDistance;

    private bool ShouldRetreat() => DistanceToPlayer() < enemy.minRetreatDistance;

    private bool CanSafelyAttack()
    {
        if (enemy.player == null)
            return false;

        if (enemy.edgeDetected)
            return false;

        if (enemy.wallDetected)
            return false;

        if (!enemy.groundDetected)
            return false;

        return true;
    }

    private float DistanceToPlayer()
    {
        if (enemy.player == null)
            return float.MaxValue;

        return Mathf.Abs(enemy.player.position.x - enemy.transform.position.x);
    }

    private int DirectToPlayer()
    {
        if (enemy.player == null)
            return 0;

        float distance = enemy.player.position.x - enemy.transform.position.x;

        if (Mathf.Abs(distance) < FLIP_DEAD_ZONE)
            return 0; // too close — don't flip

        return distance > 0 ? 1 : -1;
    }
}
