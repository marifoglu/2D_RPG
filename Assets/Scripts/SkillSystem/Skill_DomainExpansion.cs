using System.Collections.Generic;
using UnityEngine;

public class Skill_DomainExpansion : Skill_Base
{

    [SerializeField] private GameObject domainPrefab;

    [Header("Slowing Down Upgrade")]
    [SerializeField] public float slowDownPercent = 0.8f;
    [SerializeField] public float slowDownDuration = 5f;

    [Header("Shard Casting Upgrade")]
    [SerializeField] public int shardToCast = 10;
    [SerializeField] public float shardCastDomainSlowdown = 1f;
    [SerializeField] public float shardCastDomainDuration = 8f;
    private float spellsCasterTime;
    private float spellsPerSecond;

    [Header("Time Echo Casting Upgrade")]
    [SerializeField] public int echoToCast = 8;
    [SerializeField] private float echoCastDomainSlow = 1f;
    [SerializeField] private float echoCastDomainDuration = 8f;
    [SerializeField] private float healthToRestoreWithEcho = .05f;

    [Header("Domain Details")]
    [SerializeField] public float maxDomainSize = 10f;
    [SerializeField] public float expandSpeed = 5f;

    private List<Enemy> trappedTargets = new List<Enemy>();
    private Transform currentTargets;

    public void CreateDomain()
    {
        spellsPerSecond = GetSpellsToCast() / GetDomainDuration();

        GameObject domainObject = Instantiate(domainPrefab, player.transform.position, Quaternion.identity);
        domainObject.GetComponent<SkillObject_DomainExpansion>().SetupDomain(this);
    }

    public float GetDomainDuration()
    {
        if (upgradeType == SkillUpgradeType.Domain_SlowingDown)
            return slowDownDuration;
        else if (upgradeType == SkillUpgradeType.Domain_ShardSpam)
            return shardCastDomainDuration;
        else if (upgradeType == SkillUpgradeType.Domain_EchoSpam)
            return echoCastDomainDuration;

        return 0f;
    }

    public float GetSlowdownPercent()
    {
        if (upgradeType == SkillUpgradeType.Domain_SlowingDown)
            return slowDownPercent;
        else if (upgradeType == SkillUpgradeType.Domain_ShardSpam)
            return shardCastDomainSlowdown;
        else if (upgradeType == SkillUpgradeType.Domain_EchoSpam)
            return echoCastDomainSlow;

        return 0f;
    }

    private int GetSpellsToCast()
    {
        if (upgradeType == SkillUpgradeType.Domain_ShardSpam)
            return shardToCast;
        else if (upgradeType == SkillUpgradeType.Domain_EchoSpam)
            return echoToCast;
        return 0;
    }

    public bool InstantDomain()
    {
        return upgradeType != SkillUpgradeType.Domain_EchoSpam
            && upgradeType != SkillUpgradeType.Domain_ShardSpam;
    }


    public void AddTarget(Enemy addTarrget)
    {
        if (addTarrget != null && !trappedTargets.Contains(addTarrget))
            trappedTargets.Add(addTarrget);
    }

    public void DoSpellCasting()
    {
        bool shouldCastEchoes = upgradeType == SkillUpgradeType.Domain_EchoSpam;
        bool shouldCastShards = upgradeType == SkillUpgradeType.Domain_ShardSpam;

        if (!shouldCastEchoes && !shouldCastShards)
            return;

        spellsCasterTime -= Time.deltaTime;

        if (spellsCasterTime <= 0)
        {
            if (currentTargets == null)
                currentTargets = FindTargetInDomain();

            if (currentTargets != null)
            {
                CastSpell(currentTargets);
                spellsCasterTime = spellsPerSecond > 0f ? 1f / spellsPerSecond : float.MaxValue;
                currentTargets = null;
            }
            else
            {
                spellsCasterTime = 0.5f;
            }
        }
    }
    private void CastSpell(Transform target)
    {
        if (upgradeType == SkillUpgradeType.Domain_EchoSpam)
        {
            Vector3 offset = Random.value < .5f ? new Vector2(1, 0) : new Vector2(-1, 0);
            skillManager.timeEcho.CreateTimeEcho(target.position + offset);
        }

        if (upgradeType == SkillUpgradeType.Domain_ShardSpam)
        {
            skillManager.shard.CreateRawShard(target, true);
        }

    }

    private Transform FindTargetInDomain()
    {
        trappedTargets.RemoveAll(target => target == null || target.enemyHealth.isDead);

        if(trappedTargets.Count == 0)
            return null;

        int randomIndex = Random.Range(0, trappedTargets.Count);    
        return trappedTargets[randomIndex].transform;
    }

    public void ClearTargets()
    {
        foreach (var enemy in trappedTargets)
        {
            if (enemy != null)
                enemy.StopSlowDown();
        }

        trappedTargets.Clear();
    }
}