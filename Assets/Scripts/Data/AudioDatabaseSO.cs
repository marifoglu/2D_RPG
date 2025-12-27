using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Database", menuName = "Audio/Audio Database")]
public class AudioDatabaseSO : ScriptableObject
{
    public List<AudioClipData> player;
    public List<AudioClipData> uiAudio;

    private Dictionary<string, AudioClipData> clipCollection;

    [Header("Music List")]
    public List<AudioClipData> mainMenuMusic;
    public List<AudioClipData> levelMusic;

    private void OnEnable()
    {
        clipCollection = new Dictionary<string, AudioClipData>();

        AddToCollection(player);
        AddToCollection(uiAudio);
        AddToCollection(mainMenuMusic);
        AddToCollection(levelMusic);
    }

    public AudioClipData Get(string groupName)
    {
        return clipCollection.TryGetValue(groupName, out var data) ? data : null;
    }
    private void AddToCollection(List<AudioClipData> listToAdd)
    {
        foreach (var data in listToAdd)
        {
            if (data != null && clipCollection.ContainsKey(data.audioName) == false)
            {
                clipCollection.Add(data.audioName, data);
            }
        }
    }
            

    [System.Serializable]
    public class AudioClipData
    {
        public string audioName;
        public List<AudioClip> clips = new List<AudioClip>();
        [Range(0f, 1f)] public float maxVolume = 1f;

        public AudioClip GetRandomClip()
        {
            if (clips == null || clips.Count == 0)
                return null;

            return clips[Random.Range(0,clips.Count)];
        }
    }
}
