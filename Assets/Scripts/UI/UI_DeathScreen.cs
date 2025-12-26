using System.Collections;
using UnityEngine;
using TMPro;

public class UI_DeathScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI deathText;

    private void OnEnable()
    {
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Hide text initially
        if (deathText != null)
            deathText.gameObject.SetActive(false);

        // Fade to black
        UI_FadeScreen fadeScreen = UI.instance.fadeScreenUI;
        fadeScreen.gameObject.SetActive(true);
        fadeScreen.transform.SetAsLastSibling();
        fadeScreen.DoFadeOut(1f);

        yield return fadeScreen.fadeEffectCo;

        // Show text on black screen
        if (deathText != null)
        {
            deathText.gameObject.SetActive(true);
            deathText.transform.SetAsLastSibling();
        }

        // Wait 3 seconds
        yield return new WaitForSeconds(3f);

        // Hide text and death screen BEFORE restart
        if (deathText != null)
            deathText.gameObject.SetActive(false);

        gameObject.SetActive(false);

        // Go to checkpoint - screen stays black, no second fade
        GameManager.instance.RestartScene();
    }
}