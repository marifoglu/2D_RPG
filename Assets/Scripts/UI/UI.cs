using UnityEngine;

public class UI : MonoBehaviour
{
    #region UI Components
    public UI_SkillToolTip skillToolTip { get; private set; }
    public UI_ItemToolTip itemToolTip { get; private set; }
    public UI_StatToolTip statToolTip { get; private set; }

    public UI_SkillTree skillTreeUI { get; private set; }
    public UI_Inventory inventoryUI { get; private set; }
    public UI_Storage storageUI { get; private set; }
    public UI_Craft craftUI { get; private set; }
    public UI_Merchant merchantUI { get; private set; }
    public UI_InGame inGameUI { get; private set; }
    #endregion

    private bool skillTreeEnabled;
    private bool inventoryEnabled;

    private void Awake()
    {
        // Find tooltips - check self first, then children
        itemToolTip = GetComponent<UI_ItemToolTip>();
        if (itemToolTip == null)
            itemToolTip = GetComponentInChildren<UI_ItemToolTip>(true);
        if (itemToolTip == null)
            Debug.LogError("UI_ItemToolTip not found on UI or its children!");

        skillToolTip = GetComponent<UI_SkillToolTip>();
        if (skillToolTip == null)
            skillToolTip = GetComponentInChildren<UI_SkillToolTip>(true);
        if (skillToolTip == null)
            Debug.LogError("UI_SkillToolTip not found on UI or its children!");

        statToolTip = GetComponent<UI_StatToolTip>();
        if (statToolTip == null)
            statToolTip = GetComponentInChildren<UI_StatToolTip>(true);
        if (statToolTip == null)
            Debug.LogError("UI_StatToolTip not found on UI or its children!");

        // Find UI panels
        skillTreeUI = GetComponent<UI_SkillTree>();
        if (skillTreeUI == null)
            skillTreeUI = GetComponentInChildren<UI_SkillTree>(true);

        inventoryUI = GetComponent<UI_Inventory>();
        if (inventoryUI == null)
            inventoryUI = GetComponentInChildren<UI_Inventory>(true);

        storageUI = GetComponent<UI_Storage>();
        if (storageUI == null)
            storageUI = GetComponentInChildren<UI_Storage>(true);

        craftUI = GetComponent<UI_Craft>();
        if (craftUI == null)
            craftUI = GetComponentInChildren<UI_Craft>(true);

        merchantUI = GetComponent<UI_Merchant>();
        if (merchantUI == null)
            merchantUI = GetComponentInChildren<UI_Merchant>(true);

        inGameUI = GetComponent<UI_InGame>();
        if (inGameUI == null)
            inGameUI = GetComponentInChildren<UI_InGame>(true);

        // Initialize enabled states with null checks
        skillTreeEnabled = skillTreeUI != null && skillTreeUI.gameObject.activeSelf;
        inventoryEnabled = inventoryUI != null && inventoryUI.gameObject.activeSelf;
    }

    private void Start()
    {
        skillTreeUI.UnlockDefaultSkills();      
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