using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    private Player player;

    [SerializeField] private RectTransform healthRect;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText; 

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        player.health.OnHealthUpdate += UpdateHealthBar;
    }

    private void UpdateHealthBar()
    {
        float currentHealth = Mathf.RoundToInt(player.health.GetCurrentHealth());
        float maxHealth = player.stats.GetMaxHealth();

        // Change size the health rect and slider
        float sizeDifference = Mathf.Abs(maxHealth - healthRect.sizeDelta.x);
        if(sizeDifference > 0.1f)
            healthRect.sizeDelta = new Vector2(maxHealth, healthRect.sizeDelta.y); // For reduceing health bar size, can multiply by .2f

        healthText.text = currentHealth + " / " + maxHealth;
        healthSlider.value = player.health.GetHealthPercentage();
    }
}
