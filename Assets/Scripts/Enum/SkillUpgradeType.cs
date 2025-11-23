using UnityEngine;

public enum SkillUpgradeType
{
    None,

    // Dash Tree ---
    Dash,   // Dash to avoid damage
    Dash_CloneStart, // Creates a clone at when dash starts
    Dash_CloneOnStartAndArrival, // Creates a clone at when dash starts an when dash ends
    Dash_ShardOnStart, // Creates a shard at when dash starts
    Dash_ShardOnStartAndArrival, // Creates a shard at when dash starts an when dash ends

    // Shard Tree ---
    Shard, // Shard explose when touches by enemy or when time goes up.
    Shard_MoveToEnemy, // Shard moves to nearest enemy.
    Shard_MultiCast, // Shard ability can have up to N charges. You can cast them al in row.
    Shard_Teleport, // You can Teleport to the places with the last shard position.
    Shard_TeleportHpRewind, // When you Teleport to the your HP % is restored.

    // Shard Tree ---
    SwordThrow, // Throw a sword that damages enemies from distance.
    SwordThrow_Spin, // Thrown sword spins, damaging all enemies in its path.
    SwordThrow_Pierce, // Thrown sword pierces through multiple enemies.
    SwordThrow_Bounce// Thrown sword bounces between enemies.
}

