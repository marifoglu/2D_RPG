using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{
    public void PlayButton()
    {
        GameManager.instance.ContinuePlay();
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }
}
