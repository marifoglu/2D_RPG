using System.Collections.Generic;
using UnityEngine;

public class SkillObject_SwordBounce : SkillObject_Sword
{
    [SerializeField] private float bounceSpeed = 15f;
    private int bounceCount;

    private Collider2D[] enemyTargets;
    private Transform nextTarget;
    private List<Transform> selectedBefore = new List<Transform>();

    public override void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        anim.SetTrigger("Spin");

        base.SetupSword(swordManager, direction);

        bounceSpeed = swordManager.bounceSpeed;
        bounceCount = swordManager.bounceCount;

        // Initialize enemy list and try to pick a first target immediately
        enemyTargets = GetEnemiesAround(transform, 10f);
        if (enemyTargets != null && enemyTargets.Length > 0)
        {
            rb.simulated = false; // control movement manually
            nextTarget = GetNextTarget();
        }
    }

    protected override void Update()
    {
        HandleComeBack();
        HandleBounce();
    }

    private void HandleBounce()
    {
        if (nextTarget == null)
            return;

        transform.position = Vector2.MoveTowards(transform.position, nextTarget.position, bounceSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, nextTarget.position) < 0.75f)
        {
            DamageEnemiesInRadius(transform, 1f);
            BounceToNextTarget();

            if (bounceCount == 0 || nextTarget == null)
            {
                nextTarget = null;
                GetSwordBackToPlayer();
            }
        }
    }

    private void BounceToNextTarget()
    {
        nextTarget = GetNextTarget();
        bounceCount--;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyTargets == null)
        {
            enemyTargets = GetEnemiesAround(transform, 10f);
            rb.simulated = false;
        }

        DamageEnemiesInRadius(transform, 1f);

        if (enemyTargets.Length <= 1 || bounceCount == 0)
            GetSwordBackToPlayer();
        else
            nextTarget = GetNextTarget();
    }

    private Transform GetNextTarget()
    {
        List<Transform> validTarget = GetValidTargets();

        if (validTarget == null || validTarget.Count == 0)
            return null;

        int randomIndex = Random.Range(0, validTarget.Count);

        Transform next = validTarget[randomIndex];
        selectedBefore.Add(next);

        return next;
    }

    private List<Transform> GetValidTargets()
    {
        List<Transform> validTargets = new List<Transform>();
        List<Transform> aliveTargets = GetAliveTargets();

        foreach (var enemy in aliveTargets)
        {
            if (enemy != null && !selectedBefore.Contains(enemy.transform))
                validTargets.Add(enemy.transform);
        }

        if (validTargets.Count > 0)
        {
            return validTargets; // <-- fixed: return filtered list, not full aliveTargets
        }
        else
        {
            selectedBefore.Clear();
            return validTargets;
        }
    }

    private List<Transform> GetAliveTargets()
    {
        List<Transform> aliveTargets = new List<Transform>();

        if (enemyTargets == null)
            return aliveTargets;

        foreach (var enemy in enemyTargets)
        {
            if (enemy != null)
                aliveTargets.Add(enemy.transform);
        }
        return aliveTargets;
    }
}