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

        Debug.Log($"[{enemy.name}] ENTERED BattleState - Player: {(enemy.player != null ? enemy.player.name : "NULL")}");

        UpdateBattleTimer();

        // Use the Enemy's player reference (set by TryEnterBattleState) as the source of truth.
        if (enemy.player == null)
        {
            Debug.LogWarning($"[{enemy.name}] Player reference is NULL on Enter! Attempting to detect...");
            var hit = enemy.PlayerDetected();
            if (hit.collider != null)
            {
                Debug.Log($"[{enemy.name}] Found player via raycast: {hit.transform.name}");
                enemy.TryEnterBattleState(hit.transform); // use method (setter is inaccessible)
            }
            else
            {
                Debug.LogWarning($"[{enemy.name}] No player detected via raycast!");
            }
        }

        if (ShouldRetreat())
        {
            Debug.Log($"[{enemy.name}] Retreating! Distance: {DistanceToPlayer()}, Min: {enemy.minRetreatDistance}");
            rb.linearVelocity = new Vector2(enemy.retreatVelocity.x * -DirectToPlayer(), enemy.retreatVelocity.y);
            enemy.HandleFlip(DirectToPlayer());
        }
    }

    public override void Update()
    {
        base.Update();

        bool playerDetected = enemy.PlayerDetected();

        // Refresh battle timer when the player is detected in front of the enemy
        if (playerDetected)
        {
            Debug.Log($"[{enemy.name}] Player DETECTED - Refreshing battle timer");
            UpdateBattleTimer();
        }
        else
        {
            Debug.Log($"[{enemy.name}] Player NOT DETECTED - Last known player: {(enemy.player != null ? "EXISTS" : "NULL")}");

            // Player is no longer in line of sight - try to turn around to find them
            if (enemy.player != null)
            {
                int dirToLastKnownPlayer = DirectToPlayer();

                Debug.Log($"[{enemy.name}] Direction to last known player: {dirToLastKnownPlayer}, Current facing: {enemy.facingDir}");

                // If player is behind us, flip to face them
                if (dirToLastKnownPlayer != 0 && dirToLastKnownPlayer != enemy.facingDir)
                {
                    Debug.Log($"[{enemy.name}] FLIPPING to face last known player position!");
                    enemy.Flip();
                }
            }
        }

        if (BattleTimeOver())
        {
            Debug.Log($"[{enemy.name}] Battle time OVER! Returning to idle. Time elapsed: {Time.time - lastTimeWasInBattle}");
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        // Only enter attack when player is actually detected by the raycast (line-of-sight).
        if (WithinAttackRange() && playerDetected)
        {
            Debug.Log($"[{enemy.name}] Player in attack range AND detected! Attacking... Distance: {DistanceToPlayer()}");
            stateMachine.ChangeState(enemy.attackState);
            return;
        }

        // Otherwise move toward the (known) player position if we have one.
        if (enemy.player != null)
        {
            int dir = DirectToPlayer();
            Debug.Log($"[{enemy.name}] Moving toward player - Direction: {dir}, Speed: {enemy.battleMoveSpeed * dir}");
            enemy.SetVelocity(enemy.battleMoveSpeed * dir, rb.linearVelocity.y);
        }
        else
        {
            Debug.LogWarning($"[{enemy.name}] No player reference - STOPPING!");
            enemy.SetVelocity(0f, rb.linearVelocity.y);
        }
    }

    private void UpdateBattleTimer() => lastTimeWasInBattle = Time.time;
    private bool BattleTimeOver() => Time.time >= lastTimeWasInBattle + enemy.battleTimeDuration;
    private bool WithinAttackRange() => DistanceToPlayer() < enemy.attackDistance;

    private bool ShouldRetreat() => DistanceToPlayer() < enemy.minRetreatDistance;

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