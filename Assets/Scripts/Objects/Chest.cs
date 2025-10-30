using UnityEngine;

public class Chest : MonoBehaviour, IDamageable
{

    private Animator animator => GetComponentInChildren<Animator>();
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    private Entity_VFX entityVFX => GetComponent<Entity_VFX>();

    [Header("Chest Settings")]
    [SerializeField] private Vector2 knockback;

    public void TakeDamage(float damage, Transform damageDealer)
    {
        entityVFX.PlayOnDamageVfx();
        animator.SetBool("chestOpen", true);
        rb.linearVelocity = knockback;
        rb.angularVelocity = Random.Range(-200f, 200f);

        // Drop Items

    }

}
