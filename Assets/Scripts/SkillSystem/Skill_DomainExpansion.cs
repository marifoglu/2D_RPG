using System.Collections.Generic;
using UnityEngine;

public class Skill_DomainExpansion : Skill_Base
{

    [SerializeField] private GameObject domainPrefab;

    [Header("Slowing Down Upgrade")]
    [SerializeField] public float slowDownPercent = 0.8f;
    [SerializeField] public float slowDownDuration = 5f;

    [Header("Spell Casting Upgrade")]
    [SerializeField] public int spellToCast = 10;
    [SerializeField] public float spellCastDomainSlowdown = 1f;
    [SerializeField] public float spellCastDomainDuration = 8f;

    [Header("Domain Details")]
    [SerializeField] public float maxDomainSize = 10f;
    [SerializeField] public float expandSpeed = 10f;
    private float spellCasterTime;
    private float spellPerSecond;

    private List<Enemy> trappedTargets = new List<Enemy>();
    private Transform currentTargets;



    public float GetDomainDuration()
    {
        // Compute duration first (no recursive call)
        float duration = (upgradeType == SkillUpgradeType.Domain_SlowingDown)
            ? slowDownDuration
            : spellCastDomainDuration;

        // Guard against division by zero
        if (duration <= 0f)
        {
            Debug.LogWarning("Domain duration is 0 or negative!");
            spellPerSecond = 0f;
            return 1f; // Return at least 1 second to prevent division by zero
        }

        // Calculate spells per second based on chosen duration
        spellPerSecond = (float)spellToCast / duration;

        return duration;
    }

    public float GetSlowdownPercent()
    {
        if (upgradeType == SkillUpgradeType.Domain_SlowingDown)
            return slowDownPercent;
        else
            return spellCastDomainSlowdown;
    }

    public bool InstantDomain()
    {
        return upgradeType != SkillUpgradeType.Domain_EchoSpam
            && upgradeType != SkillUpgradeType.Domain_ShardSpam;
    }

    public void CreateDomain()
    {
        GameObject domainObject = Instantiate(domainPrefab, player.transform.position, Quaternion.identity);
        domainObject.GetComponent<SkillObject_DomainExpansion>().SetupDomain(this);

        // Initialize spell casting timer properly
        if (spellPerSecond > 0f)
            spellCasterTime = 1f / spellPerSecond;
        else
            spellCasterTime = float.MaxValue;

        Debug.Log($"Domain created. Upgrade type: {upgradeType}, SpellPerSecond: {spellPerSecond}, Initial timer: {spellCasterTime}");
    }

    public void AddTarget(Enemy addTarrget)
    {
        if (addTarrget != null && !trappedTargets.Contains(addTarrget))
        {
            trappedTargets.Add(addTarrget);
            Debug.Log($"Target added to domain: {addTarrget.name}. Total targets: {trappedTargets.Count}");
        }
    }

    public void DoSpellCasting()
    {
        // CHECK: Only specific upgrade types should cast spells
        // Domain_EchoSpam and Domain_ShardSpam should spawn clones
        // But if upgradeType is something like TimeEcho_SingleAttack, it means
        // you're trying to use a TimeEcho upgrade on the Domain skill, which is wrong

        bool shouldCastEchoes = upgradeType == SkillUpgradeType.Domain_EchoSpam;
        bool shouldCastShards = upgradeType == SkillUpgradeType.Domain_ShardSpam;

        if (!shouldCastEchoes && !shouldCastShards)
        {
            Debug.LogWarning($"Domain upgrade type '{upgradeType}' does not support spell casting. Use Domain_EchoSpam or Domain_ShardSpam.");
            return;
        }

        spellCasterTime -= Time.deltaTime;

        if (spellCasterTime <= 0)
        {
            if (currentTargets == null)
                currentTargets = FindTargetInDomain();

            if (currentTargets != null)
            {
                Debug.Log($"Casting spell at target: {currentTargets.name}");
                CastSpell(currentTargets);
                spellCasterTime = spellPerSecond > 0f ? 1f / spellPerSecond : float.MaxValue;
                currentTargets = null;
            }
            else
            {
                Debug.LogWarning("No valid targets found in domain!");
                // Reset timer even if no target found to prevent rapid checking
                spellCasterTime = 0.5f;
            }
        }
    }
    private void CastSpell(Transform target)
    {
        if (upgradeType == SkillUpgradeType.Domain_EchoSpam)
        {
            Vector3 offset = Random.value < .5f ? new Vector2(1, 0) : new Vector2(-1, 0);
            Debug.Log($"Creating Time Echo at position: {target.position + offset}");
            skillManager.timeEcho.CreateTimeEcho(target.position + offset);
        }

        if (upgradeType == SkillUpgradeType.Domain_ShardSpam)
        {
            Debug.Log($"Creating Shard targeting: {target.name}");
            skillManager.shard.CreateRawShard(target, true);
        }

    }

    private Transform FindTargetInDomain()
    {
        // Remove any destroyed/null entries first
        for (int i = trappedTargets.Count - 1; i >= 0; i--)
        {
            if (trappedTargets[i] == null)
                trappedTargets.RemoveAt(i);
        }

        if (trappedTargets.Count == 0)
        {
            Debug.Log("No targets in domain!");
            return null;
        }

        int randomIndex = Random.Range(0, trappedTargets.Count);
        Enemy chosen = trappedTargets[randomIndex];

        // Extra null-check (defensive)
        if (chosen == null)
        {
            trappedTargets.RemoveAt(randomIndex);
            return null;
        }

        return chosen.transform;
    }

    public void ClearTargets()
    {
        Debug.Log($"Clearing {trappedTargets.Count} targets from domain");

        // Defensive: only call StopSlowDown on still-alive references
        foreach (var enemy in trappedTargets)
        {
            if (enemy != null)
                enemy.StopSlowDown();
        }

        trappedTargets.Clear();
    }
}