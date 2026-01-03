using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_Options : MonoBehaviour
{
    private Player player;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float mixerMultiplier = 25f;

    [Header("Master Volume Settings")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private string masterParameter;

    [Header("BGM Volume Settings")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private string bgmParameter;

    [Header("SFX Volume Settings")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private string sfxParameter;

    [Header("Buttons")]
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button cancelButton;

    public event Action OnOptionsClose;

    private float originalMasterVolume;
    private float originalBgmVolume;
    private float originalSfxVolume;

    private void Awake()
    {
        if (acceptButton != null)
            acceptButton.onClick.AddListener(OnAcceptButtonClicked);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
    }

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
    }

    private void OnDestroy()
    {
        if (acceptButton != null)
            acceptButton.onClick.RemoveAllListeners();

        if (cancelButton != null)
            cancelButton.onClick.RemoveAllListeners();
    }

    private void OnEnable()
    {
        // Store current saved values as original
        originalMasterVolume = PlayerPrefs.GetFloat("masterVolume", 0.6f);
        originalBgmVolume = PlayerPrefs.GetFloat("bgmVolume", 0.6f);
        originalSfxVolume = PlayerPrefs.GetFloat("sfxVolume", 0.6f);

        // Set sliders to saved values
        masterSlider.value = originalMasterVolume;
        bgmSlider.value = originalBgmVolume;
        sfxSlider.value = originalSfxVolume;
    }

    public void MasterSliderValue(float value)
    {
        float newValue = value > 0.0001f ? Mathf.Log10(value) * mixerMultiplier : -80f;
        audioMixer.SetFloat(masterParameter, newValue);
    }

    public void BGMSliderValue(float value)
    {
        float newValue = value > 0.0001f ? Mathf.Log10(value) * mixerMultiplier : -80f;
        audioMixer.SetFloat(bgmParameter, newValue);
    }

    public void SFXSliderValue(float value)
    {
        float newValue = value > 0.0001f ? Mathf.Log10(value) * mixerMultiplier : -80f;
        audioMixer.SetFloat(sfxParameter, newValue);
    }

    public void GoMainMenuButton() => GameManager.instance.ChangeScene("MainMenu", RespawnType.NoneSpecific);

    private void OnAcceptButtonClicked()
    {
        // Save to PlayerPrefs
        PlayerPrefs.SetFloat("masterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("bgmVolume", bgmSlider.value);
        PlayerPrefs.SetFloat("sfxVolume", sfxSlider.value);
        PlayerPrefs.Save();

        OnOptionsClose?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnCancelButtonClicked()
    {
        // Restore original values
        masterSlider.value = originalMasterVolume;
        bgmSlider.value = originalBgmVolume;
        sfxSlider.value = originalSfxVolume;

        // Apply original values to mixer
        MasterSliderValue(originalMasterVolume);
        BGMSliderValue(originalBgmVolume);
        SFXSliderValue(originalSfxVolume);

        OnOptionsClose?.Invoke();
        gameObject.SetActive(false);
    }

    public void LoadUpVolumes()
    {
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume", 0.6f);
        bgmSlider.value = PlayerPrefs.GetFloat("bgmVolume", 0.6f);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", 0.6f);
    }
}