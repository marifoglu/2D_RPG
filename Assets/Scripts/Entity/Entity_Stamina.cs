// Assets/Scripts/Entity/Entity_Stamina.cs
using System;
using UnityEngine;
using UnityEngine.UI;

public class Entity_Stamina : MonoBehaviour
{
    private Entity_Stats entityStats;
    private Slider staminaBar; // Local mini-bar (optional)

    [Header("Stamina Settings")]
    [SerializeField] protected float currentStamina;
    [SerializeField] private float regenInterval = 0.1f;
    [SerializeField] private float regenDelay = 1f;
    private float lastStaminaUseTime;

    [Header("Stamina Costs")]
    [SerializeField] private float basicAttackCost = 10f;
    [SerializeField] private float heavyAttackCost = 25f;
    [SerializeField] private float dashCost = 20f;
    [SerializeField] private float jumpAttackCost = 15f;

    public event Action OnStaminaChanged;

    private void Awake()
    {
        entityStats = GetComponent<Entity_Stats>();

        // Optional: Try to find local slider (for mini-bars on entities)
        Slider[] sliders = GetComponentsInChildren<Slider>(true);
        if (sliders.Length > 1)
        {
            staminaBar = sliders[1];
        }
        // If only 1 slider found, it's the health bar - no stamina bar on this entity
        // This is NORMAL for the player since stamina is shown in UI_StaminaBar instead

        SetupStamina();
    }

    private void Start()
    {
        InvokeRepeating(nameof(RegenerateStamina), 0, regenInterval);
    }

    private void SetupStamina()
    {
        if (entityStats == null)
            return;

        currentStamina = entityStats.GetMaxStamina();
        UpdateStaminaBar();
    }

    public bool HasEnoughStamina(float cost)
    {
        return currentStamina >= cost;
    }

    public bool TryUseStamina(float cost)
    {
        if (!HasEnoughStamina(cost))
            return false;

        currentStamina -= cost;
        currentStamina = Mathf.Max(0, currentStamina);
        lastStaminaUseTime = Time.time;

        UpdateStaminaBar();
        OnStaminaChanged?.Invoke(); // This triggers UI_StaminaBar to update

        return true;
    }

    public void RegenerateStamina()
    {
        if (Time.time < lastStaminaUseTime + regenDelay)
            return;

        if (currentStamina >= entityStats.GetMaxStamina())
            return;

        float regenAmount = (entityStats.resources.staminaRegen.GetValue() * regenInterval);
        IncreaseStamina(regenAmount);
    }

    public void IncreaseStamina(float amount)
    {
        float maxStamina = entityStats.GetMaxStamina();
        currentStamina = Mathf.Min(currentStamina + amount, maxStamina);

        UpdateStaminaBar();
        OnStaminaChanged?.Invoke(); // This triggers UI_StaminaBar to update
    }

    private void UpdateStaminaBar()
    {
        // Update local mini-bar if it exists (optional for enemies)
        if (staminaBar != null && entityStats != null)
        {
            staminaBar.value = currentStamina / entityStats.GetMaxStamina();
        }
        // For player, the main UI is handled by UI_StaminaBar component
    }

    public float GetStaminaPercentage()
    {
        return currentStamina / entityStats.GetMaxStamina();
    }

    public float GetBasicAttackCost() => basicAttackCost;
    public float GetHeavyAttackCost() => heavyAttackCost;
    public float GetDashCost() => dashCost;
    public float GetJumpAttackCost() => jumpAttackCost;

    private void OnValidate()
    {
        if (Application.isPlaying && entityStats != null)
            currentStamina = Mathf.Clamp(currentStamina, 0, entityStats.GetMaxStamina());
    }
}