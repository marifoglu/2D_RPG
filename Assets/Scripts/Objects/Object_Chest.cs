using UnityEngine;

public class Object_Chest : MonoBehaviour, IDamageable
{
    private Animator animator => GetComponentInChildren<Animator>();
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    private Entity_VFX entityVFX => GetComponent<Entity_VFX>();
    private Collider2D col;
    private bool isOpened = false;

    [Header("Chest Settings")]
    [SerializeField] private Vector2 knockback;

    private void Awake()
    {
        col = GetComponent<Collider2D>();

        // collider a trigger so enemies can pass through
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Keep rigidbody kinematic to prevent physics
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public bool TakeDamage(float damage, float elementalDamage, ElementType elementType, Transform damageDealer)
    {
        // Only allow opening once
        if (isOpened)
            return false;

        isOpened = true;

        entityVFX.PlayOnDamageVfx();
        animator.SetBool("chestOpen", true);

        StartCoroutine(VisualKnockbackEffect());

        // Drop Items
        return true;
    }

    private System.Collections.IEnumerator VisualKnockbackEffect()
    {
        Vector3 originalPosition = transform.position;
        Vector3 knockbackPosition = originalPosition + new Vector3(knockback.x * 0.1f, knockback.y * 0.1f, 0);

        float duration = 0.3f;
        float elapsed = 0f;

        // Move to knockback position
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2);
            transform.position = Vector3.Lerp(originalPosition, knockbackPosition, t);
            yield return null;
        }

        // Return to original position
        elapsed = 0f;
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2);
            transform.position = Vector3.Lerp(knockbackPosition, originalPosition, t);
            yield return null;
        }

        transform.position = originalPosition;
    }
}