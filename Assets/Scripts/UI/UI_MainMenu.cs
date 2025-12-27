using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{

    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    private void Start()
    {
        transform.root.GetComponentInChildren<UI_FadeScreen>().DoFadeIn();

        transform.root.GetComponentInChildren<UI_Options>(true).LoadUpVolumes();
        AudioManager.instance.StartBackgroundMusic("MainMenuMusics");
    }

    public void PlayButton()
    {
        AudioManager.instance.PlayGlobalSFX("ButtonClick");
        GameManager.instance.ContinuePlay();
    }

    public void OpenOptionsButton()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void BackToMainMenuButton()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGameButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}