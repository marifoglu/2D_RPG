using System;
using System.Collections;
using UnityEngine;

public class Skill_Shard : Skill_Base
{
    private SkillObject_Shard currentShard;
    private Entity_Health playerHealth;

    [SerializeField] private GameObject shardPrefab;
    [SerializeField] private float detonateTime = 2f;

    [Header("Moving Shard Upgrades")]
    [SerializeField] private float shardSpeed = 10f;

    [Header("MutliCast Shard Upgrades")]
    [SerializeField] private int maxCharges = 3;
    [SerializeField] private int currentCharges;
    [SerializeField] private bool isCharging;

    [Header("Teleport Shard Upgrades")]
    [SerializeField] private float shardExistDuration = 10;
        
    [Header("Health Rewind Shard Upgrades")]
    [SerializeField] private float savedHealthPercent;

    protected override void Awake()
    {
        base.Awake();
        currentCharges = maxCharges;
        playerHealth = player.GetComponentInParent<Entity_Health>();
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

        if (Unlocked(SkillUpgradeType.Shard_Teleport))
            HandleShardTeleport();

        if (Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
            HandleShardHealthRewind();


    }
    public void CreateShard()
    {
        float detonateTime = GetDetonateTime();

        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        currentShard = shard.GetComponent<SkillObject_Shard>();

        currentShard.SetupShard(this);

        if (Unlocked(SkillUpgradeType.Shard_Teleport) || Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
        {
            currentShard.onExplode += ForceCooldown;
        }
    }

    public void CreateRawShard(Transform target = null, bool shardsCanMove = false)
    {
        bool canMove = shardsCanMove != false ? shardsCanMove : Unlocked(SkillUpgradeType.Shard_MoveToEnemy) || Unlocked(SkillUpgradeType.Shard_MultiCast); 

        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        shard.GetComponent<SkillObject_Shard>().SetupShard(this, detonateTime, canMove, shardSpeed, target);
    }

    public void CreateDomainShard(Transform target)
    {

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

    private void HandleShardTeleport()
    {
        if (currentShard == null)
        {
            CreateShard();
        }
        else
        {
            SwapPlayerWithShard();
            SetSkillOnCooldown();
        }
    }

    private void SwapPlayerWithShard()
    {
        Vector3 shardPosition = currentShard.transform.position;
        Vector3 playerPosition = player.transform.position;

        currentShard.transform.position = playerPosition;
        currentShard.Explode();

        player.TeleportPlayer(shardPosition);
    }

    public float GetDetonateTime()
    {
        if(Unlocked(SkillUpgradeType.Shard_Teleport) || Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
        {
            return  shardExistDuration;
        }
        return detonateTime;
    }

    private void ForceCooldown()
    {
        if (OnCoolDown() == false)
        {
            SetSkillOnCooldown();
            currentShard.onExplode -= ForceCooldown;
        }
    }

    private void HandleShardHealthRewind()
    {
        if(currentShard == null)
        {
            CreateShard();
            savedHealthPercent = playerHealth.GetHealthPercentage();
        }
        else
        {
            SwapPlayerWithShard();
            playerHealth.SetHealthToPercentage(savedHealthPercent);
            SetSkillOnCooldown();
        }
    }
}