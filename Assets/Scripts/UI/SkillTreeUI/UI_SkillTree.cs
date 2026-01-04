using System.Linq;
using TMPro;
using UnityEngine;

public class UI_SkillTree : MonoBehaviour, ISaveable
{
    [SerializeField] private int skillPoints;
    [SerializeField] private TextMeshProUGUI skillPointsText;
    [SerializeField] private UI_TreeConnectHandler[] parentNodes;
    private UI_TreeNode[] allTreeNodes;
    public Player_SkillManager skillManager { get; private set; }

    private void Start()
    {
        UpdateAllConnections();
        UpdateSkillPointsUI();
        
        Debug.Log($"[UI_SkillTree] Started with {skillPoints} skill points");
    }

    private void UpdateSkillPointsUI()
    {
        if (skillPointsText != null)
            skillPointsText.text = skillPoints.ToString();
            
        Debug.Log($"[UI_SkillTree] Updated UI - Skill Points: {skillPoints}");
    }

    public void UnlockDefaultSkills()
    {
        allTreeNodes = GetComponentsInChildren<UI_TreeNode>(true);
        skillManager = FindAnyObjectByType<Player_SkillManager>();

        foreach (var node in allTreeNodes)
            node.UnlockDefaultSkill();
    }

    [ContextMenu("Reset Skill Tree")]
    public void RefundAllSkills()
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();

        foreach (var node in skillNodes)
            node.Refund();
    }

    public bool EnoughSkillPoints(int cost) => skillPoints >= cost;
    
    public void RemoveSkillPoints(int cost)
    {
        skillPoints = skillPoints - cost;
        UpdateSkillPointsUI();
        Debug.Log($"[UI_SkillTree] Removed {cost} points. Remaining: {skillPoints}");
    }
    
    public void AddSkillPoints(int points)
    {
        skillPoints = skillPoints + points;
        UpdateSkillPointsUI();
        Debug.Log($"[UI_SkillTree] Added {points} points. Total: {skillPoints}");
    }
    
    // Public getter for current skill points
    public int GetSkillPoints() => skillPoints;

    public void SetSkillManager(Player_SkillManager manager)
    {
        skillManager = manager;

        if (skillManager == null)
        {
            Debug.LogError("SetSkillManager called with null manager!");
        }
    }

    [ContextMenu("Update All Connections")]
    public void UpdateAllConnections()
    {
        foreach (var node in parentNodes)
        {
            node.UpdateAllConnections();
        }
    }

    public void LoadData(GameData gameData)
    {
        Debug.Log($"[UI_SkillTree] LoadData called. GameData.skillPoints: {gameData.skillPoints}");

        // ALWAYS load the saved value (0 is valid)
        skillPoints = gameData.skillPoints;

        UpdateSkillPointsUI();

        // Initialize allTreeNodes if null
        if (allTreeNodes == null || allTreeNodes.Length == 0)
            allTreeNodes = GetComponentsInChildren<UI_TreeNode>(true);

        // Verify skillManager exists before loading
        if (skillManager == null)
        {
            Debug.LogWarning("SkillManager is null during LoadData");
            var player = FindAnyObjectByType<Player>();
            if (player != null && player.skillManager != null)
            {
                SetSkillManager(player.skillManager);
            }
            else
            {
                Debug.LogError("Cannot find SkillManager");
                return;
            }
        }

        // Load skill tree unlock states
        if (gameData.skillTreeUI != null)
        {
            foreach (var node in allTreeNodes)
            {
                if (node == null || node.skillData == null)
                    continue;

                string skillName = node.skillData.displayName;

                if (string.IsNullOrEmpty(skillName))
                    continue;

                if (gameData.skillTreeUI.TryGetValue(skillName, out bool isUnlocked) && isUnlocked)
                {
                    node.UnlockWithSaveData();
                }
            }
        }

        // Load skill upgrades
        if (skillManager.allSkills == null || skillManager.allSkills.Length == 0)
        {
            Debug.LogWarning("SkillManager.allSkills is null during LoadData");
            return;
        }

        if (gameData.skillUpgrades != null)
        {
            foreach (var skill in skillManager.allSkills)
            {
                if (skill == null)
                    continue;

                if (gameData.skillUpgrades.TryGetValue(skill.GetSkillType(), out SkillUpgradeType upgradeType))
                {
                    var upgradeNode = allTreeNodes.FirstOrDefault(node =>
                        node != null &&
                        node.skillData != null &&
                        node.skillData.upgradeData != null &&
                        node.skillData.upgradeData.upgradeType == upgradeType);

                    if (upgradeNode != null)
                    {
                        skill.SetSkillUpgrade(upgradeNode.skillData);
                    }
                }
            }
        }

        Debug.Log($"[UI_SkillTree] LoadData complete. Final skill points: {skillPoints}");
    }

    public void SaveData(ref GameData gameData)
    {
        Debug.Log($"[UI_SkillTree] SaveData called. Current skill points: {skillPoints}");
        
        gameData.skillPoints = skillPoints;

        if (gameData.skillTreeUI == null)
            gameData.skillTreeUI = new SerializableDictionary<string, bool>();

        if (gameData.skillUpgrades == null)
            gameData.skillUpgrades = new SerializableDictionary<SkillType, SkillUpgradeType>();

        gameData.skillTreeUI.Clear();
        gameData.skillUpgrades.Clear();

        // Initialize allTreeNodes if null
        if (allTreeNodes == null || allTreeNodes.Length == 0)
            allTreeNodes = GetComponentsInChildren<UI_TreeNode>(true);

        // Save skill tree unlock states
        foreach (var node in allTreeNodes)
        {
            if (node == null || node.skillData == null)
                continue;

            string skillName = node.skillData.displayName;

            if (string.IsNullOrEmpty(skillName))
            {
                Debug.LogWarning($"Skill node has null or empty displayName: {node.gameObject.name}");
                continue;
            }

            gameData.skillTreeUI[skillName] = node.isUnlocked;
        }

        // Save skill upgrades
        if (skillManager == null)
        {
            Debug.LogWarning("SkillManager is null during SaveData");
            var player = FindAnyObjectByType<Player>();
            if (player != null && player.skillManager != null)
            {
                SetSkillManager(player.skillManager);
            }
            else
            {
                Debug.LogError("Cannot find SkillManager");
                return;
            }
        }

        if (skillManager.allSkills == null || skillManager.allSkills.Length == 0)
        {
            Debug.LogWarning("SkillManager.allSkills is null during SaveData");
            return;
        }

        foreach (var skill in skillManager.allSkills)
        {
            if (skill == null)
                continue;

            gameData.skillUpgrades[skill.GetSkillType()] = skill.GetUpgradeType();
        }
        
        Debug.Log($"[UI_SkillTree] SaveData complete. Saved skill points: {gameData.skillPoints}");
    }
}