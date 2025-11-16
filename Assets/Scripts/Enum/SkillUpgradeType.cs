using UnityEngine;

public enum SkillUpgradeType
{
    // Dash Tree ---
    Dash,   // Dash to avoid damage
    Dash_CloneStart, // Creates a clone at when dash starts
    Dash_CloneOnStartAndArrival, // Creates a clone at when dash starts an when dash ends
    Dash_ShardOnStart, // Creates a shard at when dash starts
    Dash_ShardOnStartAndArrival // Creates a shard at when dash starts an when dash ends
}
