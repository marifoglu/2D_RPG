using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioDatabaseSO audioDatabase;
    [SerializeField] private AudioSource backgroundSource;
    [SerializeField] private AudioSource sfxSource;

    private Transform player;

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

        sfxSource.clip = clip;
        sfxSource.pitch = Random.Range(0.95f, 1.1f);
        sfxSource.volume = Mathf.Lerp(0, maxVolume, t);
        sfxSource.PlayOneShot(clip);

    }
}
