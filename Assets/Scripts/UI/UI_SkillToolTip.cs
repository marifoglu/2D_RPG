using TMPro;
using UnityEngine;

public class UI_SkillToolTip : UI_ToolTip
{

    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillRequirement;

    public override void ShowToolTip(bool show, RectTransform targetRect)
    {
        base.ShowToolTip(show, targetRect);
    }

    public void ShowToolTip(bool show, RectTransform targetRect, Skill_DataSO skillData)
    {
        base.ShowToolTip(show, targetRect);

        if(show == false)
            return;

        skillNameText.text = skillData.displayName;
        skillDescription.text = skillData.description;
        skillRequirement.text = $"Requirement: \n"
            + " - " + skillData.cost + " Skill point.";
    }
}
