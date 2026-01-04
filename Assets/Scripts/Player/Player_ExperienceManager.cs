using System;
using UnityEngine;

public class Player_ExperienceManager : MonoBehaviour, ISaveable
{
    [Header("Configuration")]
    [SerializeField] private ExperienceConfigSO expConfig;

    [Header("Skill Point Settings")]
    [Tooltip("Experience required to earn 1 skill point")]
    [SerializeField] private int expPerSkillPoint = 100;

    [Header("Current Progress")]
    [SerializeField] private int currentExp;

    // Events for UI updates
    public event Action OnExperienceChanged;
    public event Action OnSkillPointEarned;

    private UI_SkillTree skillTree;

    private void Start()
    {
        // Find skill tree reference
        UI ui = FindAnyObjectByType<UI>();
        if (ui != null)
        {
            skillTree = ui.skillTreeUI;
            Debug.Log("[Player_ExperienceManager] Found UI_SkillTree");
        }

        if (skillTree == null)
        {
            Debug.LogError("[Player_ExperienceManager] UI_SkillTree not found! Make sure it exists in the scene.");
        }

        Debug.Log($"[Player_ExperienceManager] Started with {currentExp} experience. Need {expPerSkillPoint} EXP per skill point.");
    }

    public void AddExperience(int amount)
    {
        currentExp += amount;
        OnExperienceChanged?.Invoke();

        // Check if we earned skill points
        CheckForSkillPoints();

        Debug.Log($"[Player_ExperienceManager] Gained {amount} EXP. Total: {currentExp}/{expPerSkillPoint}");
    }

    public void AddExperienceFromEnemy(EnemyRarity rarity)
    {
        if (expConfig == null)
        {
            Debug.LogWarning("[Player_ExperienceManager] ExperienceConfigSO is null!");
            return;
        }

        int expAmount = expConfig.GetExperienceForRarity(rarity);
        Debug.Log($"[Player_ExperienceManager] Enemy rarity: {rarity}, EXP: {expAmount}");
        AddExperience(expAmount);
    }

    private void CheckForSkillPoints()
    {
        if (expPerSkillPoint <= 0) return;

        while (currentExp >= expPerSkillPoint)
        {
            currentExp -= expPerSkillPoint;

            // Grant skill point to skill tree
            if (skillTree != null)
            {
                Debug.Log($"[Player_ExperienceManager] GRANTING 1 SKILL POINT! Previous points: {skillTree.GetSkillPoints()}");
                skillTree.AddSkillPoints(1);
                Debug.Log($"<color=yellow>[Player_ExperienceManager] SKILL POINT EARNED! Total: {skillTree.GetSkillPoints()}</color>");
            }
            else
            {
                Debug.LogError("[Player_ExperienceManager] Cannot grant skill point - UI_SkillTree is null!");
            }

            OnSkillPointEarned?.Invoke();
            OnExperienceChanged?.Invoke();
        }
    }

    // Getters
    public int GetCurrentExp() => currentExp;


    public int GetCurrentSkillPoints()
    {
        if (skillTree != null)
        {
            return skillTree.GetSkillPoints();
        }
        return 0;
    }

    public float GetExpProgress()
    {
        if (expPerSkillPoint <= 0) return 0f;
        return (float)currentExp / expPerSkillPoint;
    }

    public int GetExpRequired()
    {
        return expPerSkillPoint;
    }

    #region Save/Load
    public void LoadData(GameData gameData)
    {
        Debug.Log($"[Player_ExperienceManager] LoadData called. GameData.currentExperience: {gameData.currentExperience}");

        currentExp = gameData.currentExperience;
        OnExperienceChanged?.Invoke();

        Debug.Log($"[Player_ExperienceManager] LoadData complete. Current exp: {currentExp}");
    }

    public void SaveData(ref GameData gameData)
    {
        Debug.Log($"[Player_ExperienceManager] SaveData called. Current exp: {currentExp}");

        gameData.currentExperience = currentExp;

        Debug.Log($"[Player_ExperienceManager] SaveData complete. Saved experience: {gameData.currentExperience}");
    }
    #endregion
}