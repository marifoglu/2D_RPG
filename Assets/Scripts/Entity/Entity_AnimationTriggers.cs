using UnityEngine;
using Unity.Cinemachine;

public class Entity_AnimationTriggers : MonoBehaviour
{
    // Support both hierarchies
    private Entity playerEntity;          // Player path (Player : Entity)
    private Entity_Enemy enemyEntity;     // Enemy path (Enemy : Entity_Enemy)
    private Entity_Combat entityCombat;   // Shared combat component

    //private CinemachineImpulseSource impulseSource;

    //[Header("Camera Shake Force")]
    //[SerializeField] private float shakeForce = 1.0f; // Can be set per object from Inspector

    protected virtual void Awake()
    {
        playerEntity = GetComponentInParent<Entity>();
        enemyEntity = GetComponentInParent<Entity_Enemy>();
        entityCombat = GetComponentInParent<Entity_Combat>();

        // Only look for shake source on Player
        //impulseSource = GetComponentInParent<CinemachineImpulseSource>();

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
        // If this is an enemy and the player has died, ignore attack hit application
        var enemy = enemyEntity as Enemy;
        if (enemy != null && enemy.IsPlayerDead)
            return;

        if (entityCombat != null)
            entityCombat.PerformAttack();

        //TriggerCameraShake();
    }

    //private void TriggerCameraShake()
    //{
    //    if (impulseSource != null)
    //    {
    //        impulseSource.GenerateImpulseWithForce(shakeForce);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("[Entity_AnimationTriggers] No CinemachineImpulseSource found for camera shake.");
    //    }
    //}
}
