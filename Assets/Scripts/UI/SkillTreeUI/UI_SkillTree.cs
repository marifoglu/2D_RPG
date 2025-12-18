using UnityEngine;

public class UI_SkillTree : MonoBehaviour
{
    [SerializeField] private int skillPoints;
    [SerializeField] private UI_TreeConnectHandler[] parentNodes;
    private UI_TreeNode[] allTreeNodes;
    public Player_SkillManager skillManager { get; private set; }

    private void Start()
    {
        UpdateAllConnections();
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
    public void RemoveSkillPoints(int cost) => skillPoints -= cost;
    public void AddSkillPoints(int points) => skillPoints += points;


    [ContextMenu("Update All Connections")]
    public void UpdateAllConnections()
    {
        foreach (var node in parentNodes)
        {
            node.UpdateAllConnections();
        }

        GetComponentInChildren<UI_SkillToolTip>()?.transform.SetAsLastSibling();
    }
}