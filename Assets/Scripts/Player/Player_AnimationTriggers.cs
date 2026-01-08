using UnityEngine;

public class Player_AnimationTriggers : Entity_AnimationTriggers
{
    private Player player;
    private Player_HeavyCombat heavyCombat;
    private Player_UpCombat upCombat;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<Player>();
        heavyCombat = GetComponentInParent<Player_HeavyCombat>();
        upCombat = GetComponentInParent<Player_UpCombat>();
    }

    // Animation Event for Sword Throw
    private void ThrowSword() => player.skillManager.swordThrow.ThrowSword();

    // Animation Event for Heavy Attack
    private void HeavyAttackTrigger()
    {
        if (heavyCombat != null)
            heavyCombat.PerformHeavyAttack();
    }

    // Animation Event for Up Attack
    private void UpAttackTrigger()
    {
        if (upCombat != null)
            upCombat.PerformUpAttack();
    }

    // backstab attack animation when damage applied
    private void BackstabHitTrigger()
    {
        if (player != null && player.skillManager.backstab != null)
        {
            player.skillManager.backstab.ExecuteBackstabAttack();
        }
    }

    private void BackstabCompleteTrigger()
    {
        if (player != null)
        {
            player.CurrentStateAnimationTrigger();
        }
    }
}