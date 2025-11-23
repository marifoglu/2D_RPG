using System;
using UnityEngine;

public class Skill_ObjectShard : Skill_ObjectBase
{
    public event Action onExplode;
    private Skill_Shard shardManager;

    [SerializeField] private GameObject vfxPrefab;

    private Transform target;
    private float speed;

    private void Update()
    {
        if (target == null)
            return;

        transform.position = Vector3 .MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }
    public void SetupShard(Skill_Shard shardManager)
    {
        this.shardManager = shardManager;
        entityStats = shardManager.player.stats;
        damageScaleData = shardManager.damageScaleData;

        float detotnationTime = shardManager.GetDetonateTime();

        Invoke(nameof(Explode), detotnationTime);
    }

    public void SetupShard(Skill_Shard shardManager, float detotnationTime, bool canMove, float shardSpeed)
    {
        this.shardManager = shardManager;
        entityStats = shardManager.player.stats;
        damageScaleData = shardManager.damageScaleData;

        
        Invoke(nameof(Explode), detotnationTime);

        if(canMove)
            MoveTowerdsClosestTarget(shardSpeed);
    }

    public void MoveTowerdsClosestTarget(float moveSpeed)
    {
        target = FindClosestTarget();
        this.speed = moveSpeed;
    }

    public void Explode()
    {
        DamageEnemiesInRadius(targetCheck, checkRadius);
        GameObject vfx = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = shardManager.player.vfx.GetElementColor(usedElement);

        onExplode?.Invoke();

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() == null)
            return;

        Explode();
    }
}
