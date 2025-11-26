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
            spellPerSecond = 0f;
            return duration;
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
    }

    public void AddTarget(Enemy addTarrget)
    {
        trappedTargets.Add(addTarrget);
    }

    public void DoSpellCasting()
    {
        spellCasterTime -= Time.deltaTime;
        if (currentTargets == null)
            currentTargets = FindTargetInDomain();

        if (currentTargets != null && spellCasterTime < 0)
        {
            CastSpell(currentTargets);
            spellCasterTime = spellPerSecond > 0f ? 1f / spellPerSecond : float.MaxValue;
            currentTargets = null;
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
        // Remove any destroyed/null entries first
        for (int i = trappedTargets.Count - 1; i >= 0; i--)
        {
            if (trappedTargets[i] == null)
                trappedTargets.RemoveAt(i);
        }

        if (trappedTargets.Count == 0)
            return null;

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
        // Defensive: only call StopSlowDown on still-alive references
        foreach (var enemy in trappedTargets)
        {
            if (enemy != null)
                enemy.StopSlowDown();
        }

        trappedTargets = new List<Enemy>();
    }
}