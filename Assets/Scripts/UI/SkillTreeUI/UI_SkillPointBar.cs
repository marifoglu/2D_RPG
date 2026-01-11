using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillPointBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI skillPointsText;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject skillPointEarnedEffect;
    [SerializeField] private float effectDuration = 1f;
    [SerializeField] private Color normalColor = Color.cyan;
    [SerializeField] private Color nearCompleteColor = Color.yellow;
    [SerializeField] private float nearCompleteThreshold = 0.8f;

    private Player_ExperienceManager expManager;
    private UI_SkillTree skillTree;  // Direct reference to skill tree
    private Image fillImage;
    private bool isInitialized;

    private void Awake()
    {
        if (expSlider != null && expSlider.fillRect != null)
        {
            fillImage = expSlider.fillRect.GetComponent<Image>();
        }

        if (skillPointEarnedEffect != null)
        {
            skillPointEarnedEffect.SetActive(false);
        }
    }

    private void Start()
    {
        InitializeReferences();

        // Subscribe to save system load event to refresh UI after loading
        if (SaveManager.instance != null)
        {
            SaveManager.instance.OnLoadCompleted += OnGameLoaded;
        }
    }

    private void InitializeReferences()
    {
        // Find player experience manager
        Player player = FindAnyObjectByType<Player>();
        if (player != null)
        {
            expManager = player.GetComponent<Player_ExperienceManager>();

            if (expManager != null)
            {
                // Subscribe to events
                expManager.OnExperienceChanged += UpdateUI;
                expManager.OnSkillPointEarned += OnSkillPointEarned;
            }
            else
            {
                Debug.LogWarning("[UI_SkillPointBar] Player_ExperienceManager not found on Player!");
            }
        }

        // Find UI_SkillTree directly
        UI ui = FindAnyObjectByType<UI>();
        if (ui != null)
        {
            skillTree = ui.skillTreeUI;
        }

        if (skillTree == null)
        {
            skillTree = FindAnyObjectByType<UI_SkillTree>(FindObjectsInactive.Include);
        }

        if (skillTree == null)
        {
            Debug.LogWarning("[UI_SkillPointBar] UI_SkillTree not found!");
        }

        isInitialized = (expManager != null && skillTree != null);
        UpdateUI();
    }

    private void OnGameLoaded()
    {
        // Re-initialize if needed and refresh UI after save data is loaded
        if (!isInitialized)
        {
            InitializeReferences();
        }

        // Delay the UI update slightly to ensure all LoadData calls have completed
        Invoke(nameof(UpdateUI), 0.1f);

        Debug.Log("[UI_SkillPointBar] Game loaded - refreshing UI");
    }

    private void OnSkillPointEarned()
    {
        PlaySkillPointEarnedEffect();
        UpdateUI();  // Update immediately when skill point is earned
    }

    private void UpdateUI()
    {
        if (expManager == null) return;

        // Update slider
        float progress = expManager.GetExpProgress();
        if (expSlider != null)
        {
            expSlider.value = progress;
        }

        // Update fill color based on progress
        if (fillImage != null)
        {
            fillImage.color = progress >= nearCompleteThreshold ? nearCompleteColor : normalColor;
        }

        // Update experience text (e.g., "45/100")
        if (expText != null)
        {
            int current = expManager.GetCurrentExp();
            int required = expManager.GetExpRequired();
            expText.text = $"{current}/{required}";
        }

        // Update skill points count - READ DIRECTLY FROM UI_SkillTree
        if (skillPointsText != null && skillTree != null)
        {
            int totalPoints = skillTree.GetSkillPoints();
            skillPointsText.text = $"Skill Points: {totalPoints}";
       }
    }

    private void PlaySkillPointEarnedEffect()
    {
        if (skillPointEarnedEffect != null)
        {
            skillPointEarnedEffect.SetActive(true);
            Invoke(nameof(HideSkillPointEffect), effectDuration);
        }
    }

    private void HideSkillPointEffect()
    {
        if (skillPointEarnedEffect != null)
        {
            skillPointEarnedEffect.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (expManager != null)
        {
            expManager.OnExperienceChanged -= UpdateUI;
            expManager.OnSkillPointEarned -= OnSkillPointEarned;
        }

        if (SaveManager.instance != null)
        {
            SaveManager.instance.OnLoadCompleted -= OnGameLoaded;
        }
    }
}