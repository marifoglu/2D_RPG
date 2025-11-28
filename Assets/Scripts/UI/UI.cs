using UnityEngine;


public class UI : MonoBehaviour
{
    public UI_SkillToolTip skillToolTip;
    public UI_SkillTree skillTree;
    private bool skillTreeEnabled;

    private void Awake()
    {
        // Allow inspector assignment, but fallback to searching children
        if (skillToolTip == null)
            skillToolTip = GetComponentInChildren<UI_SkillToolTip>();

        if (skillTree == null)
            skillTree = GetComponentInChildren<UI_SkillTree>(true);
    }

    public void ToggleSkillTreeUI()
    {
        skillTreeEnabled = !skillTreeEnabled;

        if (skillTree != null)
        {
            skillTree.gameObject.SetActive(skillTreeEnabled);
        }
        else
        {
            Debug.LogWarning("UI.ToggleSkillTreeUI: skillTree is null. Ensure an UI_SkillTree is assigned or present as a child.", this);
        }

        if (skillToolTip != null)
        {
            skillToolTip.ShowToolTip(false, null);
        }
        else
        {
            Debug.LogWarning("UI.ToggleSkillTreeUI: skillToolTip is null. Ensure an UI_SkillToolTip is assigned or present as a child.", this);
        }
    }
}