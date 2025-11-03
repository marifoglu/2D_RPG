using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class Entity_Health : MonoBehaviour, IDamageable
{
    private Entity_VFX entityVFX;
    private Entity entity;
    private Entity_Stats entityStats;
    private Slider healthBar;

    [SerializeField] protected float currentHealth;
    [SerializeField] public bool isDead { get; protected set; } = false;

    [Header("On Light Damage Knockback")]
    [SerializeField] private Vector2 knockbackPower = new Vector2(1.5f, 2.4f);
    [SerializeField] private Vector2 heavyKnockbackPower = new Vector2(7f, 7f);
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private float heavyKnockbackDuration = 0.5f;

    [Header("On Heavy Damage Knockback")]
    [SerializeField] private float heavyDamageTreshold = 0.3f;

    [Header("Camera Shake on Hit")]
    private CinemachineImpulseSource impulseSource;
    [SerializeField] private ScreenShakeProfile screenShakeProfile;
    [SerializeField] private float shakeForce = 1.0f; // Can be set per object from Inspector


    private void Awake()
    {
        entityVFX = GetComponent<Entity_VFX>();
        entity = GetComponent<Entity>();
        entityStats = GetComponent<Entity_Stats>();
        healthBar = GetComponentInChildren<Slider>();
        impulseSource = GetComponentInParent<CinemachineImpulseSource>();

        // Check if entityStats exists before using it
        if (entityStats == null)
        {
            return;
        }

        currentHealth = entityStats.GetMaxHealth();
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

        // Trigger camera shake when this entity takes damage
        TriggerCameraShake();

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
        if (healthBar == null || entityStats == null)
            return;

        healthBar.value = currentHealth / entityStats.GetMaxHealth();
    }

    protected virtual void Die()
    {
        isDead = true;

        if (entity != null)
        {
            entity.EntityDeath();
            return;
        }

        var enemyEntity = GetComponent<Entity_Enemy>();
        if (enemyEntity != null)
        {
            enemyEntity.EntityDeath();
            return;
        }
    }

    private Vector2 CalculateKnockback(Transform damageDealer, float damage)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;

        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockbackPower : knockbackPower;
        knockback.x *= direction;

        return knockback;
    }

    private float CalculateDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;

    private bool IsHeavyDamage(float damage)
    {
        if (entityStats == null)
            return false;

        return damage / entityStats.GetMaxHealth() > heavyDamageTreshold;
    }
    private void TriggerCameraShake()
    {

        CameraShakeManager.Instance.ScreenShakeFromProfile(screenShakeProfile, impulseSource);
    }

}