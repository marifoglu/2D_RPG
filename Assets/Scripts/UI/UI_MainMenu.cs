using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Options Menu")]
    [SerializeField] private UI_Options optionsMenu;

    [Header("Settings")]
    [SerializeField] private string firstGameScene = "Demo_Level_0";

    private void Start()
    {
        ShowMainMenu();

        var fadeScreen = transform.root.GetComponentInChildren<UI_FadeScreen>();
        if (fadeScreen != null)
            fadeScreen.DoFadeIn();

        var options = transform.root.GetComponentInChildren<UI_Options>(true);
        if (options != null)
            options.LoadUpVolumes();

        if (AudioManager.instance != null)
            AudioManager.instance.StartBackgroundMusic("MainMenuMusics");

        if (optionsMenu == null)
            optionsMenu = transform.root.GetComponentInChildren<UI_Options>(true);

        if (optionsMenu != null)
            optionsMenu.OnOptionsClose += ShowMainMenu;
    }

    private void OnDestroy()
    {
        if (optionsMenu != null)
            optionsMenu.OnOptionsClose -= ShowMainMenu;
    }

    // PLAY BUTTON - Just start the game
    public void PlayButton()
    {
        PlayButtonSound();
        GameManager.instance.ChangeScene(firstGameScene, RespawnType.Enter);
    }

    // Same as PlayButton for compatibility
    public void NewGameButton()
    {
        PlayButtonSound();

        // Optional: Start fresh save
        if (SaveManager.instance != null)
            SaveManager.instance.NewGame();

        GameManager.instance.ChangeScene(firstGameScene, RespawnType.Enter);
    }

    // Continue from last save
    public void ContinueButton()
    {
        PlayButtonSound();

        if (SaveManager.instance != null)
            SaveManager.instance.LoadGame();

        GameManager.instance.ContinuePlay();
    }

    public void OpenOptionsButton()
    {
        PlayButtonSound();
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        if (optionsPanel != null)
            optionsPanel.SetActive(true);
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
    }

    private void PlayButtonSound()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlayGlobalSFX("ButtonClick");
    }
}