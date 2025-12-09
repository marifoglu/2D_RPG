using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item effect data - Buff", menuName = "RPG Setup/Item data/Item effect/Buff effect")]

public class ItemEffect_Buff : ItemEffectDataSO
{
    [SerializeField] private BuffEffectData[] buffsToApply;
    [SerializeField] private float buffDuration;
    [SerializeField] private string buffSource = Guid.NewGuid().ToString();

    private Player_Stats playerStats;

    public override bool CanBeUsed()
    {
        if(playerStats == null)
            playerStats = FindFirstObjectByType<Player_Stats>();

        if (playerStats.CanApplyBuffOf(buffSource))
            return true;
        else
            Debug.Log("Same buff effect can't apply twice!");
            return false;
    }

    public override void ExecuteEffect()
    {
        playerStats.ApplyBuff(buffsToApply, buffDuration, buffSource);  
    }
}
