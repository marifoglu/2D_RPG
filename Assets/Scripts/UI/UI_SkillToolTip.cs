using System.Text;
using TMPro;
using UnityEngine;

public class UI_SkillToolTip : UI_ToolTip
{
    private UI_SkillTree skillTree;

    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillRequirement;

    [Space]
    [SerializeField] private string metConditionHex;
    [SerializeField] private string notMetConditionHex;
    [SerializeField] private string importandInfoHex;
    [SerializeField] private Color exampleColor;
    [SerializeField] private string lockedSkillText = "You've taken a different path - this skill now locked.";

    override protected void Awake()
    {
        base.Awake();
        skillTree = GetComponentInParent<UI_SkillTree>();
    }

    public override void ShowToolTip(bool show, RectTransform targetRect)
    {
        base.ShowToolTip(show, targetRect);
    }

    public void ShowToolTip(bool show, RectTransform targetRect, UI_TreeNode node)
    {
        base.ShowToolTip(show, targetRect);

        if(show == false)
            return;

        skillNameText.text = node.skillData.displayName;
        skillDescription.text = node.skillData.description;
        string skillLockedText = $"<color={importandInfoHex}>{lockedSkillText}</color>";
        string requirements = node.isLocked ? skillLockedText : GetRequirementText(node.skillData.cost, node.neededNodes, node.conflictNodes);

        skillRequirement.text = requirements;
    }

    private string GetRequirementText(int skillCost, UI_TreeNode[] neededNodes, UI_TreeNode[] conflicNodes)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Requirement:");

        string costColor = skillTree.EnoughSkillPoints(skillCost) ? metConditionHex : notMetConditionHex;

        sb.AppendLine($"<color={costColor}> - {skillCost} Skill Point('s) </color>");

        foreach (var node in neededNodes)
        {
            string nodeColor = node.isUnlocked ? metConditionHex : notMetConditionHex;
            sb.AppendLine($"<color={nodeColor}> - {node.skillData.displayName} </color>");
        }

        if( conflicNodes.Length <= 0 )
            return sb.ToString();

        sb.AppendLine(); // Space
        sb.AppendLine($"<color={importandInfoHex}>Locks out: </color>");

        foreach( var node in conflicNodes )
        {
            sb.AppendLine($"<color={importandInfoHex }> - {node.skillData.displayName} </color>");
        }

        return sb.ToString();
    }
}

