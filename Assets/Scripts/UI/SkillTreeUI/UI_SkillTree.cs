//using System.Linq;
//using TMPro;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;

//public class UI_SkillTree : MonoBehaviour, ISaveable
//{
//    [SerializeField] private int skillPoints;
//    [SerializeField] private TextMeshProUGUI skillPointsText;
//    [SerializeField] private UI_TreeConnectHandler[] parentNodes;
//    private UI_TreeNode[] allTreeNodes;
//    public Player_SkillManager skillManager { get; private set; }

//    private void Start()
//    {
//        UpdateAllConnections();
//        UpdateSkillPointsUI();
//    }

//    private void UpdateSkillPointsUI()
//    {
//        skillPointsText.text = skillPoints.ToString();
//    }

//    public void SetSkillManager(Player_SkillManager manager)
//    {
//        skillManager = manager;
//    }

//    public void UnlockDefaultSkills()
//    {


//        if (allTreeNodes == null)
//            allTreeNodes = GetComponentsInChildren<UI_TreeNode>(true);

//        // Make sure skillManager is set before unlocking
//        if (skillManager == null)
//        {
//            Debug.LogError("SkillManager is null! Cannot unlock default skills.");
//            return;
//        }

//        foreach (var node in allTreeNodes)
//        {
//            if (node.skillData != null && node.skillData.unlockedByDefault && !node.isUnlocked)
//            {
//                node.UnlockDefaultSkill();
//            }
//        }
//    }

//    [ContextMenu("Reset All Skill Points")]
//    public void ResetAllSkillPoints()
//    {
//        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();

//        foreach (var node in skillNodes)
//        {
//            node.Refund();
//        }
//    }

//    public void RefundAllSkills()
//    {
//        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();

//        foreach (var node in skillNodes)
//            node.Refund();
//    }

//    public bool EnoughSkillPoints(int cost) => skillPoints >= cost;
//    public void RemoveSkillPoints(int cost)
//    {
//        skillPoints -= cost;
//        UpdateSkillPointsUI();
//    }
//    public void AddSkillPoints(int points)
//    {
//        skillPoints += points;
//        UpdateSkillPointsUI();
//    }


//    [ContextMenu("Update All Connections")]
//    public void UpdateAllConnections()
//    {
//        foreach (var node in parentNodes)
//        {
//            node.UpdateAllConnections();
//        }

//        GetComponentInChildren<UI_SkillToolTip>()?.transform.SetAsLastSibling();
//    }

//    public void SaveData(ref GameData gameData)
//    {
//        gameData.skillPoints = skillPoints;
//        gameData.skillTreeUI.Clear();
//        gameData.skillUpgrades.Clear();

//        foreach (var node in allTreeNodes)
//        {
//            string skillName = node.skillData.displayName;
//            gameData.skillTreeUI[skillName] = node.isUnlocked;
//        }

//        foreach(var skill in skillManager.allSkills)
//        {
//            gameData.skillUpgrades[skill.GetSkillType()] = skill.GetUpgradeType();
//        }
//    }

//    public void LoadData(GameData gameData)
//    {
//        skillPoints = gameData.skillPoints;

//        foreach (var node in allTreeNodes)
//        {
//            string skillName = node.skillData.displayName;
//            if (gameData.skillTreeUI.TryGetValue(skillName, out bool isUnlocked) && isUnlocked)
//            {
//                node.UnlockWithSaveData();
//            }
//        }

//        foreach (var skill in skillManager.allSkills)
//        {
//            if (gameData.skillUpgrades.TryGetValue(skill.GetSkillType(), out SkillUpgradeType upgradeType))
//            {
//                var upgradeNodes = allTreeNodes.FirstOrDefault(node => node.skillData.upgradeData.upgradeType == upgradeType);
//                if(upgradeNodes != null)
//                {
//                    skill.SetSkillUpgrade(upgradeNodes.skillData);
//                }   
//            }
//        }

//    }
//}






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
    }

    private void UpdateSkillPointsUI()
    {
        if (skillPointsText != null)
            skillPointsText.text = skillPoints.ToString();
    }

    public void SetSkillManager(Player_SkillManager manager)
    {
        skillManager = manager;
    }

    public void UnlockDefaultSkills()
    {
        if (allTreeNodes == null || allTreeNodes.Length == 0)
            allTreeNodes = GetComponentsInChildren<UI_TreeNode>(true);

        // Make sure skillManager is set before unlocking
        if (skillManager == null)
        {
            Debug.LogError("SkillManager is null! Cannot unlock default skills.");
            return;
        }

        foreach (var node in allTreeNodes)
        {
            if (node == null || node.skillData == null)
                continue;

            if (node.skillData.unlockedByDefault && !node.isUnlocked)
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
            if (node != null)
                node.Refund();
        }
    }

    public void RefundAllSkills()
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();

        foreach (var node in skillNodes)
        {
            if (node != null)
                node.Refund();
        }
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
        if (parentNodes == null)
            return;

        foreach (var node in parentNodes)
        {
            if (node != null)
                node.UpdateAllConnections();
        }

        var skillToolTip = GetComponentInChildren<UI_SkillToolTip>();
        if (skillToolTip != null)
            skillToolTip.transform.SetAsLastSibling();
    }

    public void SaveData(ref GameData gameData)
    {
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
            Debug.LogWarning("SkillManager is null during SaveData - cannot save skill upgrades");
            return;
        }

        if (skillManager.allSkills == null || skillManager.allSkills.Length == 0)
        {
            Debug.LogWarning("SkillManager.allSkills is null or empty during SaveData");
            return;
        }

        foreach (var skill in skillManager.allSkills)
        {
            if (skill == null)
                continue;

            gameData.skillUpgrades[skill.GetSkillType()] = skill.GetUpgradeType();
        }
    }

    public void LoadData(GameData gameData)
    {
        if (gameData.skillPoints > 0)
            skillPoints = gameData.skillPoints;

        UpdateSkillPointsUI();

        // Initialize allTreeNodes if null
        if (allTreeNodes == null || allTreeNodes.Length == 0)
            allTreeNodes = GetComponentsInChildren<UI_TreeNode>(true);

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
        if (skillManager == null)
        {
            Debug.LogWarning("SkillManager is null during LoadData - cannot load skill upgrades");
            return;
        }

        if (skillManager.allSkills == null || skillManager.allSkills.Length == 0)
        {
            Debug.LogWarning("SkillManager.allSkills is null or empty during LoadData");
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
                    // Find the node with matching upgrade type
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
    }
}