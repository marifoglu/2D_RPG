using UnityEngine;


public class UI : MonoBehaviour
{
    public UI_SkillToolTip skillToolTip;
    public UI_ItemToolTip itemToolTip;
    public UI_StatToolTip statToolTip;

    public UI_SkillTree skillTree;
    private bool skillTreeEnabled;

    private void Awake()
    {
        if (skillToolTip == null)
            skillToolTip = GetComponentInChildren<UI_SkillToolTip>();

        if (itemToolTip == null)
            itemToolTip = GetComponentInChildren<UI_ItemToolTip>();

        if (statToolTip == null)
            statToolTip = GetComponentInChildren<UI_StatToolTip>();

        if (skillTree == null)
            skillTree = GetComponentInChildren<UI_SkillTree>(true);
    }

    public void ToggleSkillTreeUI()
    {
        skillTreeEnabled = !skillTreeEnabled;

        if (skillTree != null)
            skillTree.gameObject.SetActive(skillTreeEnabled);
        
        if (skillToolTip != null)
            skillToolTip.ShowToolTip(false, null);
    }
}