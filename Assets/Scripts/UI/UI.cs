using UnityEngine;

public class UI : MonoBehaviour
{
    public UI_SkillToolTip skillToolTip { get; private set; }
    public UI_ItemToolTip itemToolTip { get; private set; }
    public UI_StatToolTip statToolTip { get; private set; } 
    public UI_SkillTree skillTreeUI { get; private set; }
    public UI_Inventory inventoryUI { get; private set; }

    private bool skillTreeEnabled;
    private bool inventoryEnabled;


    private void Awake()
    {
        if (skillToolTip == null)
            skillToolTip = GetComponentInChildren<UI_SkillToolTip>();

        if (itemToolTip == null)
            itemToolTip = GetComponentInChildren<UI_ItemToolTip>();

        if (statToolTip == null)
            statToolTip = GetComponentInChildren<UI_StatToolTip>();

        if (skillTreeUI == null)
            skillTreeUI = GetComponentInChildren<UI_SkillTree>(true);

        if (inventoryUI == null)
            inventoryUI = GetComponentInChildren<UI_Inventory>(true);

        skillTreeEnabled = skillTreeUI.gameObject.activeSelf;
        inventoryEnabled = inventoryUI.gameObject.activeSelf;

    }

    public void ToggleSkillTreeUI()
    {
        skillTreeEnabled = !skillTreeEnabled;

        if (skillTreeUI != null)
            skillTreeUI.gameObject.SetActive(skillTreeEnabled);
        
        if (skillToolTip != null)
            skillToolTip.ShowToolTip(false, null);
    }

    public void ToggleInventoryUI()
    {
        inventoryEnabled = !inventoryEnabled;
        if (inventoryUI != null)
            inventoryUI.gameObject.SetActive(inventoryEnabled);
        
        if (itemToolTip != null)
            itemToolTip.ShowToolTip(false, null);
    }
}