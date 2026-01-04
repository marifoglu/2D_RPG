using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Visual indicator for enemy rarity - shows colored outline or glow
/// </summary>
public class Enemy_RarityIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer enemySprite;
    [SerializeField] private Image rarityFrame; // Optional UI frame
    [SerializeField] private GameObject rarityGlow; // Optional particle effect

    [Header("Configuration")]
    [SerializeField] private ExperienceConfigSO expConfig;
    [SerializeField] private bool useOutlineShader = true;
    [SerializeField] private bool useColorTint = false;
    [SerializeField] private float tintIntensity = 0.3f;

    private Enemy enemy;
    private Material outlineMaterial;
    private Color originalColor;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();

        if (enemySprite == null)
        {
            enemySprite = GetComponentInChildren<SpriteRenderer>();
        }

        if (enemySprite != null)
        {
            originalColor = enemySprite.color;
        }
    }

    private void Start()
    {
        if (enemy != null && expConfig != null)
        {
            ApplyRarityVisuals(enemy.enemyRarity);
        }
    }

    private void ApplyRarityVisuals(EnemyRarity rarity)
    {
        Color rarityColor = expConfig != null ? expConfig.GetRarityColor(rarity) : Color.white;

        // Apply color tint to sprite
        if (useColorTint && enemySprite != null)
        {
            Color tintedColor = Color.Lerp(originalColor, rarityColor, tintIntensity);
            enemySprite.color = tintedColor;
        }

        // Apply colored frame (if using UI frame)
        if (rarityFrame != null)
        {
            rarityFrame.color = rarityColor;
            rarityFrame.gameObject.SetActive(rarity != EnemyRarity.Common);
        }

        // Apply particle glow effect
        if (rarityGlow != null)
        {
            var particleMain = rarityGlow.GetComponent<ParticleSystem>().main;
            particleMain.startColor = rarityColor;
            rarityGlow.SetActive(rarity != EnemyRarity.Common);
        }

        // Apply outline shader (if you have an outline shader)
        if (useOutlineShader && enemySprite != null)
        {
            // This requires a custom shader with outline support
            // Example: enemySprite.material.SetColor("_OutlineColor", rarityColor);
        }
    }

    /// <summary>
    /// Call this to update visuals if rarity changes at runtime
    /// </summary>
    public void RefreshRarityVisuals()
    {
        if (enemy != null)
        {
            ApplyRarityVisuals(enemy.enemyRarity);
        }
    }
}