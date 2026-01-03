using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject saveLoadPanel;

    [Header("Save/Load Menu")]
    [SerializeField] private UI_SaveLoadMenu saveLoadMenu;

    [Header("Options Menu")]
    [SerializeField] private UI_Options optionsMenu;

    private void Start()
    {
        ShowMainMenu();

        transform.root.GetComponentInChildren<UI_FadeScreen>().DoFadeIn();
        transform.root.GetComponentInChildren<UI_Options>(true).LoadUpVolumes();
        AudioManager.instance.StartBackgroundMusic("MainMenuMusics");

        if (saveLoadMenu != null)
            saveLoadMenu.OnBackPressed += ShowMainMenu;

        // Subscribe to options close event
        if (optionsMenu == null)
            optionsMenu = transform.root.GetComponentInChildren<UI_Options>(true);

        if (optionsMenu != null)
            optionsMenu.OnOptionsClose += ShowMainMenu;
    }

    private void OnDestroy()
    {
        if (saveLoadMenu != null)
            saveLoadMenu.OnBackPressed -= ShowMainMenu;

        if (optionsMenu != null)
            optionsMenu.OnOptionsClose -= ShowMainMenu;
    }

    public void LoadGameButton()
    {
        PlayButtonSound();
        OpenSaveLoadMenu(false);
    }

    public void ContinueButton()
    {
        PlayButtonSound();

        int mostRecentSlot = FindMostRecentSave();

        if (mostRecentSlot >= 0)
        {
            SaveManager.instance.SetCurrentSlot(mostRecentSlot);
            GameManager.instance.ContinuePlay();
        }
        else
        {
            OpenSaveLoadMenu(false);
        }
    }

    public void NewGameButton()
    {
        PlayButtonSound();

        int emptySlot = FindFirstEmptySlot();

        if (emptySlot >= 0)
        {
            SaveManager.instance.StartNewGame(emptySlot);
            GameManager.instance.ChangeScene("Demo_Level_0", RespawnType.Enter);
        }
        else
        {
            OpenSaveLoadMenu(false);
        }
    }

    public void OpenOptionsButton()
    {
        PlayButtonSound();
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        if (saveLoadPanel != null)
            saveLoadPanel.SetActive(false);
    }

    public void BackToMainMenuButton()
    {
        PlayButtonSound();
        ShowMainMenu();
    }

    public void QuitGameButton()
    {
        PlayButtonSound();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ShowMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        if (saveLoadPanel != null)
            saveLoadPanel.SetActive(false);
    }

    private void OpenSaveLoadMenu(bool saveMode)
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (saveLoadPanel != null)
        {
            saveLoadPanel.SetActive(true);

            if (saveLoadMenu != null)
                saveLoadMenu.SetMode(saveMode);
        }
    }

    private int FindMostRecentSave()
    {
        if (SaveManager.instance == null || SaveManager.instance.SaveSlots == null)
            return -1;

        SaveSlotData[] slots = SaveManager.instance.SaveSlots;
        int mostRecentSlot = -1;
        string mostRecentDate = "";

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && !slots[i].isEmpty)
            {
                if (mostRecentSlot < 0 || string.Compare(slots[i].saveDateTime, mostRecentDate) > 0)
                {
                    mostRecentSlot = i;
                    mostRecentDate = slots[i].saveDateTime;
                }
            }
        }

        return mostRecentSlot;
    }

    private int FindFirstEmptySlot()
    {
        if (SaveManager.instance == null || SaveManager.instance.SaveSlots == null)
            return 0;

        SaveSlotData[] slots = SaveManager.instance.SaveSlots;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null || slots[i].isEmpty)
                return i;
        }

        return -1;
    }

    private void PlayButtonSound()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlayGlobalSFX("ButtonClick");
    }

    public bool HasAnySaveData()
    {
        if (SaveManager.instance == null || SaveManager.instance.SaveSlots == null)
            return false;

        foreach (var slot in SaveManager.instance.SaveSlots)
        {
            if (slot != null && !slot.isEmpty)
                return true;
        }

        return false;
    }
}