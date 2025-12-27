using System.Collections;
using UnityEngine;
using TMPro;

public class UI_DeathScreen : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log("UI_DeathScreen ENABLED - Starting death sequence");
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        UI_FadeScreen fadeScreen = UI.instance.fadeScreenUI;

        if (fadeScreen == null)
        {
            Debug.LogError("UI_DeathScreen: fadeScreenUI is null!");
            yield break;
        }

        Debug.Log("UI_DeathScreen: Calling DoDeathFade...");

        bool fadeComplete = false;
        fadeScreen.DoDeathFade(1f, () =>
        {
            Debug.Log("UI_DeathScreen: Fade callback received!");
            fadeComplete = true;
        });

        // Wait for death fade to complete
        Debug.Log("UI_DeathScreen: Waiting for fade to complete...");
        while (!fadeComplete)
            yield return null;

        Debug.Log("UI_DeathScreen: Fade complete! Disabling self and restarting scene...");

        gameObject.SetActive(false);

        GameManager.instance.RestartScene();
    }
}