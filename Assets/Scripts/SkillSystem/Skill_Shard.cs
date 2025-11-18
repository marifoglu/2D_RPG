using System;
using System.Collections;
using UnityEngine;

public class Skill_Shard : Skill_Base
{
    Skill_ObjectShard currentShard;
    [SerializeField] private GameObject shardPrefab;
    [SerializeField] private float detonateTime = 2f;

    [Header("Moving Shard Upgrades")]
    [SerializeField] private float shardSpeed = 10f;

    [Header("MutliCast Shard Upgrades")]
    [SerializeField] private int maxCharges = 3;
    [SerializeField] private int currentCharges;
    [SerializeField] private bool isCharging;

    protected override void Awake()
    {
        base.Awake();
        currentCharges = maxCharges;
    }

    public override void TryUseSkill()
    {
        if (CanUseSkill() == false)
            return;

        if (Unlocked(SkillUpgradeType.Shard))
            HandleShardRegular();

        if(Unlocked(SkillUpgradeType.Shard_MoveToEnemy))
            HandleShardMoving();

        if(Unlocked(SkillUpgradeType.Shard_MultiCast))
            HandleShardMultiCast();
    }
    public void CreateShard()
    {
        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        currentShard = shard.GetComponent<Skill_ObjectShard>();

        currentShard.SetupShard(detonateTime);

    }
    private void HandleShardRegular()
    {
        CreateShard();
        SetSkillOnCooldown();
    }

    private void HandleShardMoving()
    {
        CreateShard();
        currentShard.MoveTowerdsClosestTarget(shardSpeed);
        SetSkillOnCooldown();
    }

    private void HandleShardMultiCast()
    {
        if (currentCharges <= 0)
            return;

        CreateShard();
        currentShard.MoveTowerdsClosestTarget(shardSpeed);
        currentCharges--;

        if(isCharging == false)
            StartCoroutine(ShardMutliChargeCo());
    }

    private IEnumerator ShardMutliChargeCo()
    {
        isCharging = true;

        while(currentCharges < maxCharges)
        {
            yield return new WaitForSeconds(cooldown);
            currentCharges++;
        }
    isCharging = false;
    }
}
