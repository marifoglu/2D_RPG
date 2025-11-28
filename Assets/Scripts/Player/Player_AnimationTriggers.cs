using UnityEngine;

public class Player_AnimationTriggers : Entity_AnimationTriggers
{
    private Player player;
    private Player_HeavyCombat heavyCombat;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<Player>();
        heavyCombat = GetComponentInParent<Player_HeavyCombat>();
    }

    // Animation Event
    private void ThrowSword() => player.skillManager.swordThrow.ThrowSword();

    // Animation Event - Heavy Attack
    private void HeavyAttackTrigger()
    {
        if (heavyCombat != null)
            heavyCombat.PerformHeavyAttack();
    }
}