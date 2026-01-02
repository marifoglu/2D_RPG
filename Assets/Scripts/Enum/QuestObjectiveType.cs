using UnityEngine;

public enum QuestObjectiveType
{
    Kill,           // Kill X enemies of type Y
    Talk,           // Talk to specific NPC
    Collect,        // Collect X items (from drops/world)
    Deliver,        // Deliver item to NPC
    Visit,          // Visit a location/trigger
    Interact,       // Interact with object
    Escort,         // Escort NPC (future)
    Defend,         // Defend location (future)
    Custom          // For special objectives
}