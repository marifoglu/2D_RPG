using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
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
    }

    private void UpdateSkillPointsUI()
    {
        skillPointsText.text = skillPoints.ToString();
    }

    public void SetSkillManager(Player_SkillManager manager)
    {
        skillManager = manager;
    }

    public void UnlockDefaultSkills()
    {

        
        if (allTreeNodes == null)
            allTreeNodes = GetComponentsInChildren<UI_TreeNode>(true);

        // Make sure skillManager is set before unlocking
        if (skillManager == null)
        {
            Debug.LogError("SkillManager is null! Cannot unlock default skills.");
            return;
        }

        foreach (var node in allTreeNodes)
        {
            if (node.skillData != null && node.skillData.unlockedByDefault && !node.isUnlocked)
            {
                node.UnlockDefaultSkill();
            }
        }
    }

    [ContextMenu("Reset All Skill Points")]
    public void ResetAllSkillPoints()
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();

        foreach (var node in skillNodes)
        {
            node.Refund();
        }
    }

    public void RefundAllSkills()
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();

        foreach (var node in skillNodes)
            node.Refund();
    }

    public bool EnoughSkillPoints(int cost) => skillPoints >= cost;
    public void RemoveSkillPoints(int cost)
    {
        skillPoints -= cost;
        UpdateSkillPointsUI();
    }
    public void AddSkillPoints(int points)
    {
        skillPoints += points;
        UpdateSkillPointsUI();
    }


    [ContextMenu("Update All Connections")]
    public void UpdateAllConnections()
    {
        foreach (var node in parentNodes)
        {
            node.UpdateAllConnections();
        }

        GetComponentInChildren<UI_SkillToolTip>()?.transform.SetAsLastSibling();
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.skillPoints = skillPoints;
        gameData.skillTreeUI.Clear();
        gameData.skillUpgrades.Clear();

        foreach (var node in allTreeNodes)
        {
            string skillName = node.skillData.displayName;
            gameData.skillTreeUI[skillName] = node.isUnlocked;
        }

        foreach(var skill in skillManager.allSkills)
        {
            gameData.skillUpgrades[skill.GetSkillType()] = skill.GetUpgradeType();
        }
    }

    public void LoadData(GameData gameData)
    {
        skillPoints = gameData.skillPoints;

        foreach (var node in allTreeNodes)
        {
            string skillName = node.skillData.displayName;
            if (gameData.skillTreeUI.TryGetValue(skillName, out bool isUnlocked) && isUnlocked)
            {
                node.UnlockWithSaveData();
            }
        }

        foreach (var skill in skillManager.allSkills)
        {
            if (gameData.skillUpgrades.TryGetValue(skill.GetSkillType(), out SkillUpgradeType upgradeType))
            {
                var upgradeNodes = allTreeNodes.FirstOrDefault(node => node.skillData.upgradeData.upgradeType == upgradeType);
                if(upgradeNodes != null)
                {
                    skill.SetSkillUpgrade(upgradeNodes.skillData);
                }   
            }
        }

    }
}