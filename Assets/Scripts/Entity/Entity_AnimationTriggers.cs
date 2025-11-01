using UnityEngine;

public class Entity_AnimationTriggers : MonoBehaviour
{
    private Entity entity;
    private Entity_Combat entitiyCombat;

    protected virtual void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entitiyCombat = GetComponentInParent<Entity_Combat>();
    }

    private void CurrentStateTrigger()
    {
        entity.CurrentStateAnimationTrigger(); 
    }

    private void AttackTrigger()
    {
        entitiyCombat.PerformAttack();
    }
}
