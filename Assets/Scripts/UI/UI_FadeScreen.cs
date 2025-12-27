using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UI_FadeScreen : MonoBehaviour
{
    public Coroutine fadeEffectCo { get; private set; }
    private Coroutine deathFadeCo; // SEPARATE coroutine for death / won't get killed!
    private bool isDeathFadeActive = false; // Block regular fades during death
    private Image fadeImage;

    [Header("Death Screen")]
    [SerializeField] private TextMeshProUGUI deathText;
    [SerializeField] private float delayBeforeText = 2f;        // Wait 2 secs on black before showing text
    [SerializeField] private float deathTextDisplayDuration = 3f; // Text stays visible for 3 secs
    [SerializeField] private float textFadeInDuration = 1f;    // How long text takes to fade in
    [SerializeField] private float textFadeOutDuration = 1f;     // How long text takes to fade out
    [SerializeField] private Color textStartColor = Color.white;
    [SerializeField] private Color textEndColor = Color.red;

    public bool skipNextFadeOut { get; set; } = false;

    private void Awake()
    {
        InitializeFadeImage();
    }

    private void InitializeFadeImage()
    {
        if (fadeImage == null)
            fadeImage = GetComponent<Image>();

        //if (fadeImage != null)
        //    fadeImage.color = new Color(0, 0, 0, 1);
    }

    public void DoFadeIn(float duration = 1) // black > transperent
    {
        if (isDeathFadeActive)
        {
            Debug.Log("DoFadeIn blocked - death fade is active!");
            return;
        }

        InitializeFadeImage();
        fadeImage.color = new Color(0, 0, 0, 1);
        FadeEffect(0f, duration);
    }

    public void DoFadeOut(float duration = 1) // transperent > black 
    {
        if (isDeathFadeActive)
        {
            Debug.Log("DoFadeOut blocked - death fade is active!");
            return;
        }

        InitializeFadeImage();
        fadeImage.color = new Color(0, 0, 0, 0);
        FadeEffect(1f, duration);
    }

    public void DoDeathFade(float fadeDuration = 1f, System.Action onComplete = null)
    {
        if (deathFadeCo != null)
            StopCoroutine(deathFadeCo);

        deathFadeCo = StartCoroutine(DeathFadeCo(fadeDuration, onComplete));
    }

    private IEnumerator DeathFadeCo(float fadeDuration, System.Action onComplete)
    {
        Debug.Log("=== DEATH FADE STARTED ===");
        isDeathFadeActive = true; // BLOCK all other fades!

        // Hide death text initially
        if (deathText != null)
        {
            deathText.gameObject.SetActive(false);
            Debug.Log("Death text found and hidden initially");
        }
        else
        {
            Debug.LogError("!!! DEATH TEXT IS NULL - DID YOU ASSIGN IT IN INSPECTOR? !!!");
        }

        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        InitializeFadeImage();
        fadeImage.color = new Color(0, 0, 0, 0);

        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            var color = fadeImage.color;
            color.a = Mathf.Lerp(0f, 1f, time / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 1);
        Debug.Log("Fade to black complete. Now waiting " + delayBeforeText + " seconds before text...");

        yield return new WaitForSecondsRealtime(delayBeforeText);

        Debug.Log("Delay complete. Now showing text for " + deathTextDisplayDuration + " seconds...");

        if (deathText != null)
        {
            deathText.gameObject.SetActive(true);
            deathText.transform.SetParent(transform); // Make sure it's child of fade screen
            deathText.transform.SetAsLastSibling();

            Color startColor = textStartColor;
            startColor.a = 0f;
            deathText.color = startColor;

            Debug.Log("Death text ACTIVATED! Starting fade in with color transition...");

            float textTime = 0f;
            while (textTime < textFadeInDuration)
            {
                textTime += Time.unscaledDeltaTime;
                float t = textTime / textFadeInDuration;

                // Lerp both alpha and color
                Color currentColor = Color.Lerp(textStartColor, textEndColor, t);
                currentColor.a = Mathf.Lerp(0f, 1f, t);
                deathText.color = currentColor;

                yield return null;
            }

            // Ensure final color is set
            deathText.color = textEndColor;
            Debug.Log("Text fade complete - now red!");
        }
        else
        {
            Debug.LogError("!!! DEATH TEXT IS STILL NULL !!!");
        }

        float remainingDisplayTime = deathTextDisplayDuration - textFadeInDuration;
        if (remainingDisplayTime > 0)
        {
            Debug.Log("Waiting " + remainingDisplayTime + " more seconds...");
            yield return new WaitForSecondsRealtime(remainingDisplayTime);
        }
        Debug.Log("Text display time FINISHED!");

        // Fade out death text
        if (deathText != null)
        {
            Debug.Log("Fading out death text...");
            float fadeOutTime = 0f;
            Color startFadeColor = deathText.color;

            while (fadeOutTime < textFadeOutDuration)
            {
                fadeOutTime += Time.unscaledDeltaTime;
                float t = fadeOutTime / textFadeOutDuration;

                Color currentColor = startFadeColor;
                currentColor.a = Mathf.Lerp(1f, 0f, t);
                deathText.color = currentColor;

                yield return null;
            }

            deathText.gameObject.SetActive(false);
            Debug.Log("Death text hidden");
        }

        skipNextFadeOut = true;
        isDeathFadeActive = false; // Allow other fades again

        Debug.Log("=== DEATH FADE COMPLETE - CALLING CALLBACK ===");

        onComplete?.Invoke();
    }

    public void SetToBlack()
    {
        InitializeFadeImage();
        fadeImage.color = new Color(0, 0, 0, 1);
    }

    public bool IsFullyBlack()
    {
        return fadeImage != null && fadeImage.color.a >= 0.99f;
    }

    private void FadeEffect(float targetAlpha, float duration)
    {
        if (fadeEffectCo != null)
            StopCoroutine(fadeEffectCo);

        fadeEffectCo = StartCoroutine(FadeEffectCo(targetAlpha, duration));
    }

    private IEnumerator FadeEffectCo(float targetAlpha, float duration)
    {
        float startAlpha = fadeImage.color.a;
        float time = 0f;

        while (time < duration)
        {
            time = time + Time.deltaTime;

            var color = fadeImage.color;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

            fadeImage.color = color;

            yield return null;
        }

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);
    }
}