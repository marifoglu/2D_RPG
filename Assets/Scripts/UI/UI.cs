//using UnityEngine;

//public class UI : MonoBehaviour
//{
//    public static UI instance;

//    [SerializeField] private GameObject[] uiElements;
//    public bool alternativeInput { get; private set; }
//    private PlayerInputSet input;

//    #region UI Components
//    public UI_SkillToolTip skillToolTip { get; private set; }
//    public UI_ItemToolTip itemToolTip { get; private set; }
//    public UI_StatToolTip statToolTip { get; private set; }
//    public UI_SkillTree skillTreeUI { get; private set; }
//    public UI_Inventory inventoryUI { get; private set; }
//    public UI_Storage storageUI { get; private set; }
//    public UI_Craft craftUI { get; private set; }
//    public UI_Merchant merchantUI { get; private set; }
//    public UI_InGame inGameUI { get; private set; }
//    public UI_Options optionsUI { get; private set; }
//    public UI_DeathScreen deathScreenUI { get; private set; }
//    public UI_FadeScreen fadeScreenUI { get; private set; }
//    public UI_Quest questUI { get; private set; }
//    //public UI_PauseMenu pauseMenuUI { get; private set; }
//    //public UI_LoadingScreen loadingScreenUI { get; private set; }
//    public UI_Dialogue dialogueUI { get; private set; }

//    #endregion

//    private bool skillTreeEnabled;
//    private bool inventoryEnabled;

//    private void Awake()
//    {
//        instance = this;

//        //itemToolTip = GetComponentInChildren<UI_ItemToolTip>();
//        //skillToolTip = GetComponentInChildren<UI_SkillToolTip>();
//        //statToolTip = GetComponentInChildren<UI_StatToolTip>();

//        itemToolTip = GetComponentInChildren<UI_ItemToolTip>(true);
//        skillToolTip = GetComponentInChildren<UI_SkillToolTip>(true);
//        statToolTip = GetComponentInChildren<UI_StatToolTip>(true);


//        skillTreeUI = GetComponentInChildren<UI_SkillTree>(true);
//        inventoryUI = GetComponentInChildren<UI_Inventory>(true);
//        storageUI = GetComponentInChildren<UI_Storage>(true);
//        craftUI = GetComponentInChildren<UI_Craft>(true);
//        merchantUI = GetComponentInChildren<UI_Merchant>(true);
//        inGameUI = GetComponentInChildren<UI_InGame>(true);
//        optionsUI = GetComponentInChildren<UI_Options>(true);
//        deathScreenUI = GetComponentInChildren<UI_DeathScreen>(true);
//        fadeScreenUI = GetComponentInChildren<UI_FadeScreen>(true);
//        questUI = GetComponentInChildren<UI_Quest>(true);       
//        dialogueUI = GetComponentInChildren<UI_Dialogue>(true);


//        if (skillTreeUI != null)
//        {
//            skillTreeEnabled = skillTreeUI.gameObject.activeSelf;
//        }

//        if (inventoryUI != null)
//        {
//            inventoryEnabled = inventoryUI.gameObject.activeSelf;
//        }
//    }

//    private void Start()
//    {
//        skillTreeUI.UnlockDefaultSkills();
//    }

//    public void SetupControlsUI(PlayerInputSet inputSet)
//    {
//        input = inputSet;

//        input.UI.SkillTreeUi.performed += ctx => ToggleSkillTreeUI();
//        input.UI.InventoryUI.performed += ctx => ToggleInventoryUI();

//        input.UI.AlternativeInput.performed += ctx => alternativeInput = true;
//        input.UI.AlternativeInput.canceled += ctx => alternativeInput = false;

//        input.UI.OptionsUI.performed += ctx =>
//        {
//            foreach (var element in uiElements)
//            {
//                if (element.activeSelf)
//                {
//                    Time.timeScale = 1;
//                    SwitchToInGameUI();
//                    return;
//                }
//            }

//            Time.timeScale = 0;
//            OpenOptionsUI();
//        };

//        input.UI.DialogueUI_Interaction.performed += ctx =>
//        {
//            if (dialogueUI.gameObject.activeInHierarchy)
//                dialogueUI.DialogueInteraction();
//        };

//        input.UI.Dialogue_Navigation.performed += ctx =>
//        {
//            int direction = Mathf.RoundToInt(ctx.ReadValue<float>());

//            if (dialogueUI.gameObject.activeInHierarchy)
//                dialogueUI.NavigateChoice(direction);
//        };
//    }

//    public void OpenDeathScreenUI()
//    {
//        SwitchTo(deathScreenUI.gameObject);
//        input.Disable(); // pay attention to this, use gamepad
//    }

//    public void OpenOptionsUI()
//    {
//        HideAllTooltips();
//        StopPlayerControls(true);
//        SwitchTo(optionsUI.gameObject);
//    }

//    public void SwitchToInGameUI()
//    {

//        HideAllTooltips();
//        StopPlayerControls(false);
//        SwitchTo(inGameUI.gameObject);

//        skillTreeEnabled = false;
//        inventoryEnabled = false;
//    }

//    private void SwitchTo(GameObject objectToSwitchOn)
//    {
//        foreach (var element in uiElements)
//            element.gameObject.SetActive(false);

//        objectToSwitchOn.SetActive(true);
//    }

//    private void StopPlayerControls(bool stopControls)
//    {
//        if (stopControls)
//            input.PlayerCharacter.Disable();
//        else
//            input.PlayerCharacter.Enable();
//    }

//    private void StopPlayerControlsIfNeeded()
//    {
//        foreach (var element in uiElements)
//        {
//            if (element.activeSelf)
//            {
//                StopPlayerControls(true);
//                return;
//            }
//        }

//        StopPlayerControls(false);
//    }

//    public void ToggleSkillTreeUI()
//    {
//        if (skillTreeUI == null)
//        {
//            Debug.LogWarning("SkillTreeUI is not assigned or found!");
//            return;
//        }

//        skillTreeUI.transform.SetAsLastSibling();
//        SetTooltipsAsLastSibling();
//        fadeScreenUI.transform.SetAsLastSibling();

//        skillTreeEnabled = !skillTreeEnabled;
//        skillTreeUI.gameObject.SetActive(skillTreeEnabled);
//        HideAllTooltips();

//        StopPlayerControlsIfNeeded();
//    }

//    public void ToggleInventoryUI()
//    {
//        if (inventoryUI == null)
//        {
//            Debug.LogWarning("InventoryUI is not assigned or found!");
//            return;
//        }

//        inventoryUI.transform.SetAsLastSibling();
//        SetTooltipsAsLastSibling();
//        fadeScreenUI.transform.SetAsLastSibling();

//        inventoryEnabled = !inventoryEnabled;
//        inventoryUI.gameObject.SetActive(inventoryEnabled);
//        HideAllTooltips();

//        StopPlayerControlsIfNeeded();
//    }
//    public void OpenDialogueUI(DialogueLineSO firstLine, DialogueNPCData npcData)
//    {
//        StopPlayerControls(true);
//        HideAllTooltips();

//        dialogueUI.gameObject.SetActive(true);
//        dialogueUI.SetupNpcData(npcData);
//        dialogueUI.PlayDialogueLine(firstLine);
//    }
//    public void OpenQuestUI(QuestDataSO[] questToShow)
//    {
//        StopPlayerControls(true);
//        HideAllTooltips();

//        questUI.gameObject.SetActive(true);
//        questUI.SetupQuestUI(questToShow);
//    }
//    public void OpenCraftUI(bool openStorageUI)
//    {
//        craftUI.gameObject.SetActive(openStorageUI);
//        StopPlayerControls(openStorageUI);

//        if (openStorageUI == false)
//        {
//            storageUI.gameObject.SetActive(false);
//            HideAllTooltips();
//        }
//    }

//    public void OpenStorageUI(bool openStorageUI)
//    {
//        storageUI.gameObject.SetActive(openStorageUI);
//        StopPlayerControls(openStorageUI);

//        if (openStorageUI == false)
//        {
//            craftUI.gameObject.SetActive(false);
//            HideAllTooltips();
//        }
//    }

//    public void OpenMerchantUI(bool openMerchantUI)
//    {
//        merchantUI.gameObject.SetActive(openMerchantUI);
//        StopPlayerControls(openMerchantUI);

//        if (openMerchantUI == false)
//            HideAllTooltips();
//    }

//    public void HideAllTooltips()
//    {
//        if (itemToolTip != null)
//            itemToolTip.ShowToolTip(false, null);

//        if (skillToolTip != null)
//            skillToolTip.ShowToolTip(false, null);

//        if (statToolTip != null)
//            statToolTip.ShowToolTip(false, null);
//    }

//    private void SetTooltipsAsLastSibling()
//    {
//        if (itemToolTip != null)
//            itemToolTip.transform.SetAsLastSibling();

//        if (skillToolTip != null)
//            skillToolTip.transform.SetAsLastSibling();

//        if (statToolTip != null)
//            statToolTip.transform.SetAsLastSibling();
//    }
//}



using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance;

    [SerializeField] private GameObject[] uiElements;
    public bool alternativeInput { get; private set; }
    private PlayerInputSet input;

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
    public UI_Options optionsUI { get; private set; }
    public UI_DeathScreen deathScreenUI { get; private set; }
    public UI_FadeScreen fadeScreenUI { get; private set; }
    public UI_Quest questUI { get; private set; }
    //public UI_PauseMenu pauseMenuUI { get; private set; }
    //public UI_LoadingScreen loadingScreenUI { get; private set; }
    public UI_Dialogue dialogueUI { get; private set; }

    #endregion

    private bool skillTreeEnabled;
    private bool inventoryEnabled;

    private void Awake()
    {
        instance = this;

        //itemToolTip = GetComponentInChildren<UI_ItemToolTip>();
        //skillToolTip = GetComponentInChildren<UI_SkillToolTip>();
        //statToolTip = GetComponentInChildren<UI_StatToolTip>();

        itemToolTip = GetComponentInChildren<UI_ItemToolTip>(true);
        skillToolTip = GetComponentInChildren<UI_SkillToolTip>(true);
        statToolTip = GetComponentInChildren<UI_StatToolTip>(true);


        skillTreeUI = GetComponentInChildren<UI_SkillTree>(true);
        inventoryUI = GetComponentInChildren<UI_Inventory>(true);
        storageUI = GetComponentInChildren<UI_Storage>(true);
        craftUI = GetComponentInChildren<UI_Craft>(true);
        merchantUI = GetComponentInChildren<UI_Merchant>(true);
        inGameUI = GetComponentInChildren<UI_InGame>(true);
        optionsUI = GetComponentInChildren<UI_Options>(true);
        deathScreenUI = GetComponentInChildren<UI_DeathScreen>(true);
        fadeScreenUI = GetComponentInChildren<UI_FadeScreen>(true);
        questUI = GetComponentInChildren<UI_Quest>(true);
        dialogueUI = GetComponentInChildren<UI_Dialogue>(true);


        if (skillTreeUI != null)
        {
            skillTreeEnabled = skillTreeUI.gameObject.activeSelf;
        }

        if (inventoryUI != null)
        {
            inventoryEnabled = inventoryUI.gameObject.activeSelf;
        }
    }

    private void Start()
    {
        skillTreeUI.UnlockDefaultSkills();
    }

    public void SetupControlsUI(PlayerInputSet inputSet)
    {
        input = inputSet;

        input.UI.SkillTreeUi.performed += ctx => ToggleSkillTreeUI();
        input.UI.InventoryUI.performed += ctx => ToggleInventoryUI();

        input.UI.AlternativeInput.performed += ctx => alternativeInput = true;
        input.UI.AlternativeInput.canceled += ctx => alternativeInput = false;

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

        input.UI.DialogueUI_Interaction.performed += ctx =>
        {
            if (dialogueUI.gameObject.activeInHierarchy)
                dialogueUI.DialogueInteraction();
        };

        input.UI.Dialogue_Navigation.performed += ctx =>
        {
            int direction = Mathf.RoundToInt(ctx.ReadValue<float>());

            if (dialogueUI.gameObject.activeInHierarchy)
                dialogueUI.NavigateChoice(direction);
        };
    }

    public void OpenDeathScreenUI()
    {
        SwitchTo(deathScreenUI.gameObject);
        if (input != null)
            input.Disable(); // pay attention to this, use gamepad
    }

    public void OpenOptionsUI()
    {
        HideAllTooltips();
        StopPlayerControls(true);
        SwitchTo(optionsUI.gameObject);
    }

    public void SwitchToInGameUI()
    {

        HideAllTooltips();
        StopPlayerControls(false);
        SwitchTo(inGameUI.gameObject);

        skillTreeEnabled = false;
        inventoryEnabled = false;
    }

    private void SwitchTo(GameObject objectToSwitchOn)
    {
        foreach (var element in uiElements)
            element.gameObject.SetActive(false);

        objectToSwitchOn.SetActive(true);
    }

    private void StopPlayerControls(bool stopControls)
    {
        if (input == null)
        {
            Debug.LogWarning("PlayerInputSet is not initialized yet");
            return;
        }

        if (stopControls)
            input.PlayerCharacter.Disable();
        else
            input.PlayerCharacter.Enable();
    }

    private void StopPlayerControlsIfNeeded()
    {
        foreach (var element in uiElements)
        {
            if (element.activeSelf)
            {
                StopPlayerControls(true);
                return;
            }
        }

        StopPlayerControls(false);
    }

    public void ToggleSkillTreeUI()
    {
        if (skillTreeUI == null)
        {
            Debug.LogWarning("SkillTreeUI is not assigned or found!");
            return;
        }

        skillTreeUI.transform.SetAsLastSibling();
        SetTooltipsAsLastSibling();
        fadeScreenUI.transform.SetAsLastSibling();

        skillTreeEnabled = !skillTreeEnabled;
        skillTreeUI.gameObject.SetActive(skillTreeEnabled);
        HideAllTooltips();

        StopPlayerControlsIfNeeded();
    }

    public void ToggleInventoryUI()
    {
        if (inventoryUI == null)
        {
            Debug.LogWarning("InventoryUI is not assigned or found!");
            return;
        }

        inventoryUI.transform.SetAsLastSibling();
        SetTooltipsAsLastSibling();
        fadeScreenUI.transform.SetAsLastSibling();

        inventoryEnabled = !inventoryEnabled;
        inventoryUI.gameObject.SetActive(inventoryEnabled);
        HideAllTooltips();

        StopPlayerControlsIfNeeded();
    }
    public void OpenDialogueUI(DialogueLineSO firstLine, DialogueNPCData npcData)
    {
        if (firstLine == null)
        {
            Debug.LogError("Cannot open dialogue: firstLine is null! Make sure to assign a DialogueLineSO in the Inspector.");
            return;
        }

        if (dialogueUI == null)
        {
            Debug.LogError("Cannot open dialogue: dialogueUI is null!");
            return;
        }

        StopPlayerControls(true);
        HideAllTooltips();

        dialogueUI.gameObject.SetActive(true);
        dialogueUI.SetupNpcData(npcData);
        dialogueUI.PlayDialogueLine(firstLine);
    }
    public void OpenQuestUI(QuestDataSO[] questToShow)
    {
        StopPlayerControls(true);
        HideAllTooltips();

        questUI.gameObject.SetActive(true);
        questUI.SetupQuestUI(questToShow);
    }
    public void OpenCraftUI(bool openStorageUI)
    {
        craftUI.gameObject.SetActive(openStorageUI);
        StopPlayerControls(openStorageUI);

        if (openStorageUI == false)
        {
            storageUI.gameObject.SetActive(false);
            HideAllTooltips();
        }
    }

    public void OpenStorageUI(bool openStorageUI)
    {
        storageUI.gameObject.SetActive(openStorageUI);
        StopPlayerControls(openStorageUI);

        if (openStorageUI == false)
        {
            craftUI.gameObject.SetActive(false);
            HideAllTooltips();
        }
    }

    public void OpenMerchantUI(bool openMerchantUI)
    {
        merchantUI.gameObject.SetActive(openMerchantUI);
        StopPlayerControls(openMerchantUI);

        if (openMerchantUI == false)
            HideAllTooltips();
    }

    public void HideAllTooltips()
    {
        if (itemToolTip != null)
            itemToolTip.ShowToolTip(false, null);

        if (skillToolTip != null)
            skillToolTip.ShowToolTip(false, null);

        if (statToolTip != null)
            statToolTip.ShowToolTip(false, null);
    }

    private void SetTooltipsAsLastSibling()
    {
        if (itemToolTip != null)
            itemToolTip.transform.SetAsLastSibling();

        if (skillToolTip != null)
            skillToolTip.transform.SetAsLastSibling();

        if (statToolTip != null)
            statToolTip.transform.SetAsLastSibling();
    }
}