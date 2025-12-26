using UnityEngine;
using UnityEngine.UI;

public class UI_Options : MonoBehaviour
{
    private Player player;

    [SerializeField] private Toggle healthBarToggle;

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
    }

    public void GoMainMenuButton() => GameManager.instance.ChangeScene("MainMenu", RespawnType.NoneSpecific);
}
