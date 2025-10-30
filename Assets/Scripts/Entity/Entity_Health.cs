using UnityEngine;
using UnityEngine.UI;

public class Entity_Health : MonoBehaviour, IDamageable
{
    private Entity_VFX entityVFX;
    private Entitiy entity;
    private Slider healthBar;


    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float currentHealth;
    [SerializeField] public bool isDead { get; protected set; } = false;


    [Header("On Light Damage Knockback")]
    [SerializeField] private Vector2 knockbackPower = new Vector2(1.5f ,2.4f);
    [SerializeField] private Vector2 heavyKnockbackPower = new Vector2(7f ,7f);
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private float heavyKnockbackDuration = 0.5f;

    [Header("On Heavy Damage Knockback")]
    [SerializeField] private float heavyDamageTreshold = 0.3f;

    [Header("Stagger Settings")]
    public float staggerDuration = 1f; // set individually in Inspector
    [HideInInspector] public bool isStaggered;

    private void Awake()
    {
        entityVFX = GetComponent<Entity_VFX>();
        entity = GetComponent<Entitiy>();
        healthBar = GetComponentInChildren<Slider>();

        currentHealth = maxHealth;
        UpdateHealthBar();
    }
    public virtual void TakeDamage(float damage, Transform damageDealer)
    {
        if (isDead)
            return;

        Vector2 knockback = CalculateKnockback(damageDealer, damage);
        float duration = CalculateDuration(damage);

        entity?.ReceiveKnockback(knockback, duration);
        entityVFX?.PlayOnDamageVfx();

        ReduceHp(damage);
    }

    protected void ReduceHp(float takenDamage)
    {
        currentHealth -= takenDamage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null)
            return;

        healthBar.value = currentHealth / maxHealth;
    }
    protected virtual void Die()
    {
        isDead = true;
        entity.EntityDeath(); 
    }

    private Vector2 CalculateKnockback(Transform damageDealer, float damage)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;
        
        Vector2 knockback =  IsHeavyDamage(damage) ? heavyKnockbackPower : knockbackPower;    
        knockback.x *= direction;

        return knockback;
    }

    private float CalculateDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;
    private bool IsHeavyDamage(float damage) => damage / maxHealth > heavyDamageTreshold;
    
}
