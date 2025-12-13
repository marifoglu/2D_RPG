using UnityEngine;

public class Object_Chest : MonoBehaviour, IDamageable
{
    private Animator animator => GetComponentInChildren<Animator>();
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    private Entity_VFX entityVFX => GetComponent<Entity_VFX>();
    private Entity_DropManager dropManager => GetComponent<Entity_DropManager>();

    [Header("Chest Settings")]
    [SerializeField] private Vector2 knockback;
    [SerializeField] private bool canDropItem = true;

    public bool TakeDamage(float damage, float elementalDamage, ElementType elementType, Transform damageDealer)
    {
        if(canDropItem == false)
            return false;

        canDropItem = false;
        dropManager?.DropItems();
        entityVFX.PlayOnDamageVfx();
        animator.SetBool("chestOpen", true);
        rb.linearVelocity = knockback;
        rb.angularVelocity = Random.Range(-200f, 200f);

        return true;
    }
}