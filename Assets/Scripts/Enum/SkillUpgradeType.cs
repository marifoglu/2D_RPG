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
    SwordThrow_Bounce,// Thrown sword bounces between enemies.

    // Shard Tree ---
    TimeEcho, // Create clone of player, it can take damage from enemies.
    TimeEcho_SingleAttack, // The clone attacks a single time before disappearing.
    TimeEcho_MultiAttack, // The clone attacks multiple times before disappearing.
    TimeEcho_ChanceToMultiply, // There is a chance that after disappearing, the clone creates another clone.
    TimeEcho_HealWisp, // When the clone dies it creates a wisp that flies to the player and heals him. Heal amount is based on clones recieved damage before die
    TimeEcho_CleanseWisp, // wisp will remove negative status effects from the player.
    TimeEcho_CoolDownWisp, // wisp will reduce all skills cooldown.

    // Backstab Tree ---
    Backstab, // Teleport behind the closest enemy and perform a backstab attack.
    Backstab_ChainStrike, // After backstab, can chain to another nearby enemy within a short window.
    Backstab_ShadowMark, // Backstabbed enemies take increased damage for a duration.
    Backstab_ExecutionStrike, // Backstab deals massively increased damage to enemies below 30% HP.
    Backstab_PhantomStep, // Leaves a damaging shadow clone at original position when teleporting.

    // Domain Expansion ---
    Domain_SlowingDown, // Create an area that slows down time for enemies inside.
    Domain_EchoSpam, // You can no longer move, but your Time Echo skill can be used multiple times rapidly.
    Domain_ShardSpam // You can no longer move, but your Time Shard skill can be used multiple times rapidly.

}

