using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using System;

public class Entity_Health : MonoBehaviour, IDamageable
{
    private Entity entity;
    private Entity_Stats entityStats;
    private Entity_VFX entityVFX;
    private Entity_DropManager dropManager;
    private Slider healthBar;

    public event Action OnTakingDamage;
    public event Action OnHealthUpdate;

    //private bool miniHealthBarActive;   
    [SerializeField] protected float currentHealth;
    public float CurrentHealth => currentHealth;
    public bool isDead { get; private set; }
    protected bool canTakeDamage = true;

    [Header("Health Regeneration")]
    [SerializeField] private float regenInterval = 1f;
    [SerializeField] private bool canRegenarteHealth = true;
    public float lastDamageTaken { get; private set; }

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
        entityStats = GetComponent<Entity_Stats>();
        dropManager = GetComponent<Entity_DropManager>();
        healthBar = GetComponentInChildren<Slider>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        if (impulseSource == null)
        {
            impulseSource = GetComponentInParent<CinemachineImpulseSource>();
        }
        // Check if entityStats exists before using it
        SetupHealth();
    }

    private void SetupHealth()
    {
        if (entityStats == null)
            return;

        currentHealth = entityStats.GetMaxHealth();
        OnHealthUpdate += UpdateHealthBar;

        UpdateHealthBar();
        InvokeRepeating(nameof(RegenerateHealth), 0, regenInterval);
    }

    public float GetCurrentHealth() => currentHealth;
    public virtual bool TakeDamage(float damage, float elementalDamage, ElementType elementType, Transform damageDealer)
    {
        if (isDead || canTakeDamage == false)
            return false;

        if (AttackEvaded())
            return false;

        // Calculate armor mitigation
        Entity_Stats attackerStats = damageDealer.GetComponent<Entity_Stats>();
        float armorReduction = attackerStats != null ? attackerStats.GetArmorReduction() : 0f;

        float mitigation = entityStats != null ? entityStats.GetArmorMitigation(armorReduction) : 0;
        float elementalResistance = entityStats != null ? entityStats.GetElementalResistance(elementType) : 0;

        // Calculate elemental resistance
        float elementalDamageTaken = elementalDamage * (1 - elementalResistance);
        float physicalDamageTaken = damage * (1 - mitigation);

        // Knockback
        TakeKnockback(damageDealer, physicalDamageTaken);
        ReduceHealth(physicalDamageTaken + elementalDamageTaken);


        // Trigger camera shake when this entity takes damage
        TriggerCameraShake();

        OnTakingDamage?.Invoke();

        return true;
    }
    public void RegenerateHealth()
    {
        if (canRegenarteHealth == false)
            return;

        float regenAmount = entityStats.resources.healthRegen.GetValue();
        increaseHealth(regenAmount);
    }
    public void increaseHealth(float healthAmount)
    {
        if(isDead)
            return;

        float newHealth = currentHealth + healthAmount;
        float maxHealth = entityStats.GetMaxHealth();
        currentHealth = Mathf.Min(newHealth, maxHealth);

        OnHealthUpdate?.Invoke();
    }
    public void ReduceHealth(float takenDamage)
    {
        currentHealth -= takenDamage;

        entityVFX?.PlayOnDamageVfx();
        OnHealthUpdate?.Invoke();

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
        }
        dropManager?.DropItems();
    }
    public void SetCanTakeDamage(bool canTakeDamage) => this.canTakeDamage = canTakeDamage;
    private bool AttackEvaded()
    {
        if (entityStats == null)
            return false;
        else
            return UnityEngine.Random.Range(0, 100) < entityStats.GetEvasion();
    }

    public float GetHealthPercentage() => currentHealth / entityStats.GetMaxHealth();

    public void SetHealthToPercentage(float percentage) // Use it in Skill_Shard Teleport HP Rewind upgrade
    {
        currentHealth = entityStats.GetMaxHealth() * Mathf.Clamp01(percentage);
        OnHealthUpdate?.Invoke();
    }

    private void TakeKnockback(Transform damageDealer, float finalDamage)
    {
        Vector2 knockback = CalculateKnockback(damageDealer, finalDamage);
        float duration = CalculateDuration(finalDamage);

        entity?.ReceiveKnockback(knockback, duration);
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
        else
            return damage / entityStats.GetMaxHealth() > heavyDamageTreshold;
    }
    private void TriggerCameraShake()
    {
        if (CameraShakeManager.Instance == null)
        {
            Debug.LogWarning("CameraShakeManager.Instance is null!");
            return;
        }

        if (screenShakeProfile == null)
        {
            Debug.LogWarning($"ScreenShakeProfile is not assigned on {gameObject.name}!");
            return;
        }

        // Always get the impulse source from the player, not from this entity
        Player player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            Debug.LogWarning("Player not found in scene!");
            return;
        }

        CinemachineImpulseSource playerImpulseSource = player.GetComponent<CinemachineImpulseSource>();
        if (playerImpulseSource == null)
        {
            Debug.LogWarning("CinemachineImpulseSource is missing on Player!");
            return;
        }

        CameraShakeManager.Instance.ScreenShakeFromProfile(screenShakeProfile, playerImpulseSource);
    }

}
