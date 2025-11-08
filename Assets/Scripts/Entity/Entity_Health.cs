using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class Entity_Health : MonoBehaviour, IDamageable
{
    private Entity_VFX entityVFX;
    private Entity entity;
    private Entity_Stats stats;
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


    private void Awake()
    {
        entityVFX = GetComponent<Entity_VFX>();
        entity = GetComponent<Entity>();
        stats = GetComponent<Entity_Stats>();
        healthBar = GetComponentInChildren<Slider>();
        impulseSource = GetComponentInParent<CinemachineImpulseSource>();

        // Check if entityStats exists before using it
        if (stats == null)
            return;   

        currentHealth = stats.GetMaxHealth();
        UpdateHealthBar();
    }

    public virtual bool TakeDamage(float damage, float elementalDamage, ElementType elementType, Transform damageDealer)
    {
        if (isDead)
            return false;

        if (AttackEvaded())
            return false;

        // Calculate armor mitigation
        Entity_Stats attackerStats = damageDealer.GetComponent<Entity_Stats>();
        float armorReduction = attackerStats != null ? attackerStats.GetArmorReduction() : 0f;

        float mitigation = stats.GetArmorMitigation(armorReduction);
        float finalDamage = damage * (1 - mitigation);


        // Calculate elemental resistance
        float elementalResistance = stats.GetElementalResistance(elementType);
        float elementalDamageTaken = elementalDamage * (1 - elementalResistance);   

        // Knockback
        Vector2 knockback = CalculateKnockback(damageDealer, finalDamage);
        float duration = CalculateDuration(finalDamage);

        entity?.ReceiveKnockback(knockback, duration);
        entityVFX?.PlayOnDamageVfx();

        ReduceHp(finalDamage + elementalDamageTaken);
        Debug.Log($"Took {elementalDamageTaken} elemental damage from the {elementType} Current HP: {currentHealth}");

        // Trigger camera shake when this entity takes damage
        TriggerCameraShake();

        return true;
    }

    private bool AttackEvaded()
    {
        return Random.Range(0,100) < stats.GetEvasion();
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
        if (healthBar == null || stats == null)
            return;

        healthBar.value = currentHealth / stats.GetMaxHealth();
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
        if (stats == null)
            return false;

        return damage / stats.GetMaxHealth() > heavyDamageTreshold;
    }
    private void TriggerCameraShake()
    {
        // Check if all required components exist
        if (CameraShakeManager.Instance == null)
        {
            Debug.LogWarning($"[{gameObject.name}] CameraShakeManager.Instance is null!");
            return;
        }

        if (screenShakeProfile == null)
        {
            Debug.LogWarning($"[{gameObject.name}] ScreenShakeProfile is not assigned!");
            return;
        }

        if (impulseSource == null)
        {
            Debug.LogWarning($"[{gameObject.name}] CinemachineImpulseSource not found! Make sure it's on a parent GameObject.");
            return;
        }

        CameraShakeManager.Instance.ScreenShakeFromProfile(screenShakeProfile, impulseSource);
    }

}