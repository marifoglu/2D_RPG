using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioDatabaseSO;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioDatabaseSO audioDatabase;
    [SerializeField] private AudioSource backgroundSource;
    [SerializeField] private AudioSource sfxSource;

    private Transform player;

    private AudioClip lastMusicPlayed;
    private Coroutine currentBackgroundCo;
    private string currentBackgroundMusicGroup;
    [SerializeField] private bool backgroundShouldPlay;



    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }   

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (backgroundShouldPlay && !backgroundSource.isPlaying)
        {
            if(string.IsNullOrEmpty(currentBackgroundMusicGroup) == false)
                NextBackgroundMusic(currentBackgroundMusicGroup);
        }

        if(backgroundSource.isPlaying && !backgroundShouldPlay)
            StopBackgroundMusic();
    }

    public void StartBackgroundMusic(string musicGroup)
    {
        backgroundShouldPlay = true;

        if (musicGroup == currentBackgroundMusicGroup)
            return;
        
        NextBackgroundMusic(musicGroup); 
    }

    public void NextBackgroundMusic(string musicGroup)
    {
        backgroundShouldPlay = true;
        currentBackgroundMusicGroup = musicGroup;

        if(currentBackgroundCo != null)
        {
            StopCoroutine(currentBackgroundCo);
        }
        currentBackgroundCo = StartCoroutine(SwitchMusicCo(musicGroup));
    }

    public void StopBackgroundMusic()
    {
        backgroundShouldPlay = false;
        
        StartCoroutine(FadeVolumeCo(backgroundSource, 0f, 1f));

        if(currentBackgroundCo != null)
            StopCoroutine(currentBackgroundCo);
    }

    private IEnumerator SwitchMusicCo(string musicGroup)
    {
        AudioClipData data = audioDatabase.Get(musicGroup);
        AudioClip nextMusic = data.GetRandomClip();

        if(data == null || data.clips.Count == 0)
        {
            Debug.LogWarning($"AudioManager: Music group '{musicGroup}' not found or has no clips.");
            yield break;
        }

        if(data.clips.Count > 1)
        {
            while (nextMusic == lastMusicPlayed)
            {
                nextMusic = data.GetRandomClip();
            }
        }

        if(backgroundSource.isPlaying)
        {
            yield return FadeVolumeCo(backgroundSource, 0f, 1f);
        }

        lastMusicPlayed = nextMusic;
        backgroundSource.clip = nextMusic;
        backgroundSource.volume = 0f;
        backgroundSource.Play();

        StartCoroutine(FadeVolumeCo(backgroundSource, data.maxVolume, 1f));
    }

    private IEnumerator FadeVolumeCo(AudioSource source, float targetVolume, float duration)
    {
        float time = 0f;
        float startVolume = source.volume;

        while (time < duration)
        {
            time += Time.deltaTime;

            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);

            yield return null;
        }
        source.volume = targetVolume;
    }
    public void PlaySFX(string soundName, AudioSource sfxSource, float minDistanceToHear = 5)
    {
        if (player == null)
            return;

        var data = audioDatabase.Get(soundName);
        if (data == null)
        {
            Debug.LogWarning($"AudioManager: AudioClipData with name '{soundName}' not found.");
            return;
        }

        var clip = data.GetRandomClip();
        if (clip == null) return;
    
        float maxVolume = data.maxVolume;
        float distance = Vector3.Distance(sfxSource.transform.position, player.position);
        float t = Mathf.Clamp01(1 - (distance / minDistanceToHear));

        sfxSource.pitch = Random.Range(0.95f, 1.1f);
        sfxSource.volume = Mathf.Lerp(0, maxVolume, t);
        sfxSource.PlayOneShot(clip);
    }

    public void PlayGlobalSFX(string soundName)
    {
        var data = audioDatabase.Get(soundName);
        if (data == null)
            return;
        
        var clip = data.GetRandomClip();    
        if(clip == null) return;

        Debug.Log("Playing global SFX: " + soundName);
        sfxSource.pitch = Random.Range(0.95f, 1.1f);
        sfxSource.volume = data.maxVolume;
        sfxSource.PlayOneShot(clip);
    }
}
