using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_Options : MonoBehaviour
{
    private Player player;

    //[SerializeField] private Toggle healthBarToggle;
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


    private void Start()
    {
        player = FindFirstObjectByType<Player>();
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

    private void OnEnable()
    {
        masterSlider.value = PlayerPrefs.GetFloat("masterMixer", .6f);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxMixer", .6f);
        bgmSlider.value = PlayerPrefs.GetFloat("bgmMixer", .6f);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat("masterMixer", masterSlider.value);
        PlayerPrefs.SetFloat("sfxMixer", sfxSlider.value);
        PlayerPrefs.SetFloat("bgmMixer", bgmSlider.value);
    }

    public void LoadUpVolumes()
    {
        masterSlider.value = PlayerPrefs.GetFloat("masterMixer", .6f);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxMixer", .6f);
        bgmSlider.value = PlayerPrefs.GetFloat("bgmMixer", .6f);
    }
}