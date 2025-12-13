//using UnityEngine;

//public class UI : MonoBehaviour
//{
//    public UI_SkillToolTip skillToolTip { get; private set; }
//    public UI_ItemToolTip itemToolTip { get; private set; }
//    public UI_StatToolTip statToolTip { get; private set; }
//    public UI_SkillTree skillTreeUI { get; private set; }
//    public UI_Inventory inventoryUI { get; private set; }
//    public UI_Storage storageUI { get; private set; }
//    public UI_Craft craftUI { get; private set; }
//    public UI_Merchant merchantUI { get; set; }

//    private bool skillTreeEnabled;
//    private bool inventoryEnabled;


//    private void Awake()
//    {
//        if (skillToolTip == null)
//            skillToolTip = GetComponentInChildren<UI_SkillToolTip>();

//        if (itemToolTip == null)
//            itemToolTip = GetComponentInChildren<UI_ItemToolTip>();

//        if (statToolTip == null)
//            statToolTip = GetComponentInChildren<UI_StatToolTip>();

//        if (skillTreeUI == null)
//            skillTreeUI = GetComponentInChildren<UI_SkillTree>(true);

//        if (inventoryUI == null)
//            inventoryUI = GetComponentInChildren<UI_Inventory>(true);

//        if (storageUI == null)
//            storageUI = GetComponentInChildren<UI_Storage>(true);

//        if (craftUI == null)
//            craftUI = GetComponentInChildren<UI_Craft>(true);

//        if (merchantUI == null)
//            merchantUI = GetComponentInChildren<UI_Merchant>(true);

//        skillTreeEnabled = skillTreeUI != null && skillTreeUI.gameObject.activeSelf;
//        inventoryEnabled = inventoryUI != null && inventoryUI.gameObject.activeSelf;

//    }

//    public void SwitchOffAllToolTips()
//    {
//        if (itemToolTip != null)
//            itemToolTip.ShowToolTip(false, null);

//        if (skillToolTip != null)
//            skillToolTip.ShowToolTip(false, null);

//        if (statToolTip != null)
//            statToolTip.ShowToolTip(false, null);
//    }

//    public void ToggleSkillTreeUI()
//    {
//        skillTreeEnabled = !skillTreeEnabled;

//        if (skillTreeUI != null)
//            skillTreeUI.gameObject.SetActive(skillTreeEnabled);

//        if (skillToolTip != null)
//            skillToolTip.ShowToolTip(false, null);
//    }

//    public void ToggleInventoryUI()
//    {
//        inventoryEnabled = !inventoryEnabled;
//        if (inventoryUI != null)
//            inventoryUI.gameObject.SetActive(inventoryEnabled);

//        if (itemToolTip != null)
//            itemToolTip.ShowToolTip(false, null);
//    }
//}
using UnityEngine;

public class UI : MonoBehaviour
{
    public UI_SkillToolTip skillToolTip { get; private set; }
    public UI_ItemToolTip itemToolTip { get; private set; }
    public UI_StatToolTip statToolTip { get; private set; }

    public UI_SkillTree skillTreeUI { get; private set; }
    public UI_Inventory inventoryUI { get; private set; }
    public UI_Storage storageUI { get; private set; }
    public UI_Craft craftUI { get; private set; }
    public UI_Merchant merchantUI { get; private set; }

    private bool skillTreeEnabled;
    private bool inventoryEnabled;

    private void Awake()
    {
        itemToolTip = GetComponentInChildren<UI_ItemToolTip>();
        skillToolTip = GetComponentInChildren<UI_SkillToolTip>();
        statToolTip = GetComponentInChildren<UI_StatToolTip>();

        skillTreeUI = GetComponentInChildren<UI_SkillTree>(true);
        inventoryUI = GetComponentInChildren<UI_Inventory>(true);
        storageUI = GetComponentInChildren<UI_Storage>(true);
        craftUI = GetComponentInChildren<UI_Craft>(true);
        merchantUI = GetComponentInChildren<UI_Merchant>(true);

        // Initialize enabled states with null checks
        skillTreeEnabled = skillTreeUI != null && skillTreeUI.gameObject.activeSelf;
        inventoryEnabled = inventoryUI != null && inventoryUI.gameObject.activeSelf;
    }

    public void SwitchOffAllToolTips()
    {
        if (itemToolTip != null)
            itemToolTip.ShowToolTip(false, null);

        if (skillToolTip != null)
            skillToolTip.ShowToolTip(false, null);

        if (statToolTip != null)
            statToolTip.ShowToolTip(false, null);
    }

    public void ToggleSkillTreeUI()
    {
        if (skillTreeUI == null)
        {
            Debug.LogWarning("SkillTreeUI is not assigned or found!");
            return;
        }

        skillTreeEnabled = !skillTreeEnabled;
        skillTreeUI.gameObject.SetActive(skillTreeEnabled);

        if (skillToolTip != null)
            skillToolTip.ShowToolTip(false, null);
    }

    public void ToggleInventoryUI()
    {
        if (inventoryUI == null)
        {
            Debug.LogWarning("InventoryUI is not assigned or found!");
            return;
        }

        inventoryEnabled = !inventoryEnabled;
        inventoryUI.gameObject.SetActive(inventoryEnabled);

        if (statToolTip != null)
            statToolTip.ShowToolTip(false, null);

        if (itemToolTip != null)
            itemToolTip.ShowToolTip(false, null);
    }
}