using UnityEngine;

public class Skill_ObjectShard : Skill_ObjectBase
{
    [SerializeField] private GameObject vfxPrefab;

    public void SetupShard(float destinationTime)
    {
        Invoke(nameof(Explode), destinationTime);
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
