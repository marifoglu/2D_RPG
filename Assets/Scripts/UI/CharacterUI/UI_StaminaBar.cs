// Assets/Scripts/UI/UI_StaminaBar.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StaminaBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private TextMeshProUGUI staminaText; // Optional text display

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color lowStaminaColor = Color.yellow;
    [SerializeField] private float lowStaminaThreshold = 0.25f;

    private Entity_Stamina playerStamina;
    private Entity_Stats playerStats;
    private Image fillImage;

    private void Awake()
    {
        if (staminaSlider == null)
            staminaSlider = GetComponent<Slider>();

        if (staminaSlider != null && staminaSlider.fillRect != null)
            fillImage = staminaSlider.fillRect.GetComponent<Image>();
    }

    private void Start()
    {
        // Find player stamina
        Player player = FindAnyObjectByType<Player>();
        if (player != null)
        {
            playerStamina = player.stamina;
            playerStats = player.stats;

            if (playerStamina != null)
            {
                playerStamina.OnStaminaChanged += UpdateStaminaUI;
                UpdateStaminaUI(); // Initial update
            }
        }
    }

    private void UpdateStaminaUI()
    {
        if (playerStamina == null || playerStats == null)
            return;

        float maxStamina = playerStats.GetMaxStamina();
        float currentStamina = playerStamina.GetStaminaPercentage() * maxStamina;
        float percentage = currentStamina / maxStamina;

        // Update slider
        if (staminaSlider != null)
            staminaSlider.value = percentage;

        // Update color based on stamina level
        if (fillImage != null)
        {
            fillImage.color = percentage <= lowStaminaThreshold ? lowStaminaColor : normalColor;
        }

        // Update text (optional)
        if (staminaText != null)
        {
            staminaText.text = $"{currentStamina:F0}/{maxStamina:F0}";
        }
    }

    private void OnDestroy()
    {
        if (playerStamina != null)
            playerStamina.OnStaminaChanged -= UpdateStaminaUI;
    }
}