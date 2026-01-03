using UnityEngine;

public class UI_PauseMenu : MonoBehaviour
{
    public void ResumeButton()
    {
        UI.instance.ClosePauseMenu();
    }

    public void OptionsButton()
    {
        gameObject.SetActive(false);
        UI.instance.OpenOptionsUI();
    }

    public void SaveButton()
    {
        GameManager.instance.QuickSave();
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1;
        GameManager.instance.ChangeScene("MainMenu", RespawnType.NoneSpecific);
    }

    public void QuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}