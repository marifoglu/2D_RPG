// Assets/Scripts/Entity/Entity_Stamina.cs
using System;
using UnityEngine;
using UnityEngine.UI;

public class Entity_Stamina : MonoBehaviour
{
    private Entity_Stats entityStats;
    private Slider staminaBar;

    [Header("Stamina Settings")]
    [SerializeField] protected float currentStamina;
    [SerializeField] private float regenInterval = 0.1f; // Regen every 0.1 seconds
    [SerializeField] private float regenDelay = 1f; // Wait 1 second after using stamina before regen starts
    private float lastStaminaUseTime;

    [Header("Stamina Costs")]
    [SerializeField] private float basicAttackCost = 10f;
    [SerializeField] private float heavyAttackCost = 25f;
    [SerializeField] private float dashCost = 20f;
    [SerializeField] private float jumpAttackCost = 15f;

    // Events for UI updates
    public event Action OnStaminaChanged;

    private void Awake()
    {
        entityStats = GetComponent<Entity_Stats>();

        // FIXED: Safe slider retrieval with bounds checking
        Slider[] sliders = GetComponentsInChildren<Slider>(true); // Include inactive
        if (sliders.Length > 1)
        {
            staminaBar = sliders[1]; // Get second slider (first is health)
        }
        else if (sliders.Length == 1)
        {
            Debug.LogWarning($"{gameObject.name}: Only one slider found. Stamina bar may not be set up correctly.");
            staminaBar = sliders[0]; // Fallback to first slider
        }
        else
        {
            Debug.LogError($"{gameObject.name}: No sliders found in children! Please add stamina slider to the prefab.");
        }

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
        OnStaminaChanged?.Invoke();

        return true;
    }

    public void RegenerateStamina()
    {
        // Wait for delay after using stamina
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
        OnStaminaChanged?.Invoke();
    }

    private void UpdateStaminaBar()
    {
        if (staminaBar == null || entityStats == null)
            return;

        staminaBar.value = currentStamina / entityStats.GetMaxStamina();
    }

    public float GetStaminaPercentage()
    {
        return currentStamina / entityStats.GetMaxStamina();
    }

    // Stamina cost getters
    public float GetBasicAttackCost() => basicAttackCost;
    public float GetHeavyAttackCost() => heavyAttackCost;
    public float GetDashCost() => dashCost;
    public float GetJumpAttackCost() => jumpAttackCost;

    // For debugging
    private void OnValidate()
    {
        if (Application.isPlaying && entityStats != null)
            currentStamina = Mathf.Clamp(currentStamina, 0, entityStats.GetMaxStamina());
    }
}