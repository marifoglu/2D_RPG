// Assets/Scripts/UI/UI_LowHealthEffect.cs - ACTUALLY FINAL THIS TIME
using UnityEngine;
using UnityEngine.UI;

public class UI_LowHealthEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image damageOverlay;

    [Header("Settings")]
    [SerializeField] private float lowHealthThreshold = 10f;
    [SerializeField] private Color damageColor = new Color(1f, 0f, 0f, 1f);
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 0.7f;

    [Header("Edge Settings")]
    [SerializeField] private float edgeSize = 0.08f; // SMALLER = THINNER EDGES (try 0.05 to 0.10)

    private Entity_Health playerHealth;
    private Player player;
    private float currentAlpha;
    private bool isInitialized = false;

    private void Awake()
    {
        // ✅ CREATE VIGNETTE IN AWAKE SO IT'S READY BEFORE START
        if (damageOverlay != null)
        {
            damageOverlay.raycastTarget = false;
            CreateEdgeOnlyVignette();
            damageOverlay.color = new Color(damageColor.r, damageColor.g, damageColor.b, 0f); // START WITH ALPHA 0!
        }
    }

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        if (player != null)
        {
            playerHealth = player.health;
        }

        isInitialized = true;
    }

    private void CreateEdgeOnlyVignette()
    {
        int size = 512;
        Texture2D vignetteTexture = new Texture2D(size, size);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // Normalize coordinates (0 to 1)
                float xNorm = x / (float)size;
                float yNorm = y / (float)size;

                // Distance from nearest edge (0 at edges, 0.5 at center)
                float distanceFromEdgeX = Mathf.Min(xNorm, 1f - xNorm);
                float distanceFromEdgeY = Mathf.Min(yNorm, 1f - yNorm);
                float distanceFromNearestEdge = Mathf.Min(distanceFromEdgeX, distanceFromEdgeY);

                // Only show in edge zone
                float alpha = 0f;
                if (distanceFromNearestEdge < edgeSize)
                {
                    // Fade from edge (1.0) to inner boundary (0.0)
                    alpha = 1f - (distanceFromNearestEdge / edgeSize);
                    alpha = Mathf.Pow(alpha, 2f); // POWER 2 = sharper falloff
                }

                vignetteTexture.SetPixel(x, y, new Color(1, 1, 1, alpha));
            }
        }

        vignetteTexture.Apply();

        Sprite vignetteSprite = Sprite.Create(
            vignetteTexture,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f)
        );

        damageOverlay.sprite = vignetteSprite;
    }

    private void Update()
    {
        if (!isInitialized || playerHealth == null || damageOverlay == null)
            return;

        float currentHealth = playerHealth.CurrentHealth;

        if (currentHealth <= lowHealthThreshold && currentHealth > 0)
        {
            PulseEffect();
        }
        else
        {
            FadeOut();
        }
    }

    private void PulseEffect()
    {
        float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        currentAlpha = Mathf.Lerp(minAlpha, maxAlpha, pulse);

        float healthPercentage = Mathf.Clamp01(playerHealth.CurrentHealth / lowHealthThreshold);
        currentAlpha *= (1f - healthPercentage);

        damageOverlay.color = new Color(damageColor.r, damageColor.g, damageColor.b, currentAlpha);
    }

    private void FadeOut()
    {
        if (damageOverlay.color.a > 0.01f)
        {
            currentAlpha = Mathf.Lerp(damageOverlay.color.a, 0f, Time.deltaTime * 5f);
            damageOverlay.color = new Color(damageColor.r, damageColor.g, damageColor.b, currentAlpha);
        }
        else if (damageOverlay.color.a > 0)
        {
            damageOverlay.color = new Color(damageColor.r, damageColor.g, damageColor.b, 0f);
        }
    }
}