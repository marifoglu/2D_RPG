using UnityEngine;

public class Entity_AnimationTriggers : MonoBehaviour
{
    private Entitiy entity;
    private Entity_Combat entitiyCombat;

    protected virtual void Awake()
    {
        entity = GetComponentInParent<Entitiy>();
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
