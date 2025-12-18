using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject[] uiElements;

    #region UI Components

    public bool alternativeInput { get; private set; }
    public PlayerInputSet input { get; private set; }
    public UI_SkillToolTip skillToolTip { get; private set; }
    public UI_ItemToolTip itemToolTip { get; private set; }
    public UI_StatToolTip statToolTip { get; private set; }

    public UI_SkillTree skillTreeUI { get; private set; }
    public UI_Inventory inventoryUI { get; private set; }
    public UI_Storage storageUI { get; private set; }
    public UI_Craft craftUI { get; private set; }
    public UI_Merchant merchantUI { get; private set; }
    public UI_InGame inGameUI { get; private set; }
    public UI_Options optionsUI { get; private set; }
    #endregion

    private bool skillTreeEnabled;
    private bool inventoryEnabled;
    private bool optionsEnabled;

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
        EnsureSkillTreeUI();
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

        optionsUI = GetComponent<UI_Options>();
        if (optionsUI == null)
            optionsUI = GetComponentInChildren<UI_Options>(true);

        // Initialize enabled states with null checks
        skillTreeEnabled = skillTreeUI != null && skillTreeUI.gameObject.activeSelf;
        inventoryEnabled = inventoryUI != null && inventoryUI.gameObject.activeSelf;
        optionsEnabled = optionsUI != null && optionsUI.gameObject.activeSelf;
    }

    private void Start()
    {
        // Ensure inGameUI is fully initialized before unlocking skills
        if (inGameUI != null)
            StartCoroutine(InitializeSkillsAfterFrame());
        else
            Debug.LogError("InGameUI is null! Cannot unlock default skills.");
    }

    private System.Collections.IEnumerator InitializeSkillsAfterFrame()
    {
        // Wait one frame to ensure all Start() methods have been called
        yield return null;

        if (skillTreeUI != null)
            skillTreeUI.UnlockDefaultSkills();
    }

    public void SetupControlUI(PlayerInputSet inputSet)
    {
        input = inputSet;

        input.UI.SkillTreeUi.performed += ctx => ToggleSkillTreeUI();
        input.UI.InventoryUI.performed += ctx => ToggleInventoryUI();

        input.UI.AlternativeInput.performed += ctx => alternativeInput = true;
        input.UI.AlternativeInput.canceled += ctx => alternativeInput = false;

        //input.UI.OptionsUI.performed += ctx => ToggleOptionsUI();
        input.UI.OptionsUI.performed += ctx =>
        {
            foreach (var element in uiElements)
            {
                if (element.activeSelf)
                {
                    Time.timeScale = 1;
                    SwitchToInGameUI();
                    return;
                }
            }
            Time.timeScale = 0;
            OpenOptionsUI();
        };
    }

    public void SetupSkillTree(Player_SkillManager skillManager)
    {
        EnsureSkillTreeUI();

        if (skillTreeUI != null)
            skillTreeUI.SetSkillManager(skillManager);
        else
            Debug.LogError("SkillTreeUI is null! Cannot set skill manager.");
    }


    public void OpenOptionsUI()
    {
        foreach(var element in uiElements)
        {
            element.gameObject.SetActive(false);
        }
        HideAllToolTips();
        StopPlayerControl(true);
        optionsUI.gameObject.SetActive(true);
    }

    public void SwitchToInGameUI()
    {
        foreach (var element in uiElements)
        {
            element.gameObject.SetActive(false);
        }
        HideAllToolTips();
        StopPlayerControl(false);
        optionsUI.gameObject.SetActive(true);
        
        skillTreeEnabled = false;
        inventoryEnabled = false;

    }
    private void StopPlayerControl(bool stopControls)
    {
        if (stopControls)
            input.PlayerCharacter.Disable();
        else
            input.PlayerCharacter.Enable();
    }

    private void StopPlayerControlsIfNeeded()
    {
        foreach( var element in uiElements)
        {
            if (element.activeSelf)
            {
                StopPlayerControl(true);
                return;
            }
        }
        StopPlayerControl(false);
    }

    public void ToggleSkillTreeUI()
    {
        skillTreeUI.transform.SetAsLastSibling();
        SetToolTipsAsLastSibling();

        if (skillTreeUI == null)
        {
            Debug.LogWarning("SkillTreeUI is not assigned or found!");
            return;
        }

        skillTreeEnabled = !skillTreeEnabled;
        skillTreeUI.gameObject.SetActive(skillTreeEnabled);

        HideAllToolTips() ;


        StopPlayerControlsIfNeeded();
    }

    public void ToggleInventoryUI()
    {
        inventoryUI.transform.SetAsLastSibling();
        SetToolTipsAsLastSibling();


        if (inventoryUI == null)
        {
            Debug.LogWarning("InventoryUI is not assigned or found!");
            return;
        }

        inventoryEnabled = !inventoryEnabled;
        inventoryUI.gameObject.SetActive(inventoryEnabled);

        HideAllToolTips();

        StopPlayerControlsIfNeeded();
    }

    public void ToggleOptionsUI()
    {
        if (optionsUI == null)
        {
            Debug.LogWarning("OptionsUI is not assigned or found!");
            return;
        }

        optionsEnabled = !optionsEnabled;
        optionsUI.gameObject.SetActive(optionsEnabled);

        StopPlayerControl(optionsEnabled);
    }

    public void OpenStorageUI(bool openStorageUI)
    {
        storageUI.gameObject.SetActive(openStorageUI);
        StopPlayerControl(openStorageUI);

        if (openStorageUI == false)
        {
            craftUI.gameObject.SetActive(false);
            HideAllToolTips();
        }
    }

    public void OpenMerchantUI(bool openMerchantUI)
    {
        merchantUI.gameObject.SetActive(openMerchantUI);
        StopPlayerControl(openMerchantUI);

        if (openMerchantUI == false)
            HideAllToolTips();
    }

    private void EnsureSkillTreeUI()
    {
        if (skillTreeUI != null)
            return;

        skillTreeUI = GetComponent<UI_SkillTree>();
        if (skillTreeUI == null)
            skillTreeUI = GetComponentInChildren<UI_SkillTree>(true);
    }
    public void HideAllToolTips()
    {
        if (itemToolTip != null)
            itemToolTip.ShowToolTip(false, null);

        if (skillToolTip != null)
            skillToolTip.ShowToolTip(false, null);

        if (statToolTip != null)
            statToolTip.ShowToolTip(false, null);
    }

    private void SetToolTipsAsLastSibling()
    {
        itemToolTip.transform.SetAsLastSibling();
        skillToolTip.transform.SetAsLastSibling();
        statToolTip.transform.SetAsLastSibling();
    }
}