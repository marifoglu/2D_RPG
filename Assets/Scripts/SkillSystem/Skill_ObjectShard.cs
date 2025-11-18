using UnityEngine;

public class Skill_ObjectShard : Skill_ObjectBase
{
    [SerializeField] private GameObject vfxPrefab;
    public Transform target;
    private float speed;

    private void Update()
    {
        if (target == null)
            return;

        transform.position = Vector3 .MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }
    public void SetupShard(float destinationTime)
    {
        Invoke(nameof(Explode), destinationTime);
    }

    public void MoveTowerdsClosestTarget(float moveSpeed)
    {
        target = FindClosestTarget();
        this.speed = moveSpeed;
    }

    private void Explode()
    {
        DamageEnemiesInRadius(targetCheck, checkRadius);
        Instantiate(vfxPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() == null)
            return;

        Explode();
    }
}
