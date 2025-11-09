using UnityEngine;
using Unity.Cinemachine;

public class Entity_AnimationTriggers : MonoBehaviour
{
    private Entity entity;
    private Entity_Combat entityCombat;

    //private CinemachineImpulseSource impulseSource;

    //[Header("Camera Shake Force")]
    //[SerializeField] private float shakeForce = 1.0f; // Can be set per object from Inspector

    protected virtual void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityCombat = GetComponentInParent<Entity_Combat>();

        // Only look for shake source on Player
        //impulseSource = GetComponentInParent<CinemachineImpulseSource>();

        if (entity == null)
            Debug.LogError($"{name}: Entity_AnimationTriggers couldn't find Entity on parents.");
    }

    // Animation Event: called from clips
    private void CurrentStateTrigger()
    {
        if (entity != null)
        {
            entity.CurrentStateAnimationTrigger();
            return;
        }

        Debug.LogWarning($"{name}: CurrentStateTrigger called but no entity found.");
    }

    // Animation Event: called from clips
    private void AttackTrigger()
    {
        // If this is an enemy and the player has died, ignore attack hit application
        Enemy enemy = entity as Enemy;
        if (enemy != null && enemy.IsPlayerDead)
            return;

        if (entityCombat != null)
            entityCombat.PerformAttack();
    }
}