using UnityEngine;

public class Entity_AnimationTriggers : MonoBehaviour
{
    // Support both hierarchies
    private Entity playerEntity;          // Player path (Player : Entity)
    private Entity_Enemy enemyEntity;     // Enemy path (Enemy : Entity_Enemy)
    private Entity_Combat entityCombat;   // Shared combat component

    protected virtual void Awake()
    {
        playerEntity = GetComponentInParent<Entity>();
        enemyEntity = GetComponentInParent<Entity_Enemy>();
        entityCombat = GetComponentInParent<Entity_Combat>();

        if (playerEntity == null && enemyEntity == null)
            Debug.LogError($"{name}: Entity_AnimationTriggers couldn't find Entity or Entity_Enemy on parents.");
    }

    // Animation Event: called from clips
    private void CurrentStateTrigger()
    {
        if (playerEntity != null) { playerEntity.CurrentStateAnimationTrigger(); return; }
        if (enemyEntity != null) { enemyEntity.CurrentStateAnimationTrigger(); return; }

        Debug.LogWarning($"{name}: CurrentStateTrigger called but no entity found.");
    }

    // Animation Event: called from clips
    private void AttackTrigger()
    {
        if (entityCombat != null)
            entityCombat.PerformAttack();
        // else silently ignore to avoid NRE on non-combat clips
    }
}
