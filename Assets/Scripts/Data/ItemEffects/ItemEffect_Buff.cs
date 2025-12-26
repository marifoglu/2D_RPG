using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item effect data - Buff", menuName = "RPG Setup/Item data/Item effect/Buff effect")]
public class ItemEffect_Buff : ItemEffectDataSO
{
    [SerializeField] private BuffEffectData[] buffsToApply;
    [SerializeField] private float buffDuration;
    [SerializeField] private string buffSource = Guid.NewGuid().ToString();

    public override bool CanBeUsed(Player player)
    {

        if (player.stats.CanApplyBuffOf(buffSource))
        {
            this.player = player;
            return true;
        }
        else
        {
            Debug.Log("Same buff effect can't apply twice!");
            return false;
        }
    }

    public override void ExecuteEffect()
    {
        player.stats.ApplyBuff(buffsToApply, buffDuration, buffSource);
        player = null;
    }
}
