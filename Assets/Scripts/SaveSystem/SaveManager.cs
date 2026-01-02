//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class SaveManager : MonoBehaviour
//{
//    public static SaveManager instance;

//    private FileDataHandler dataHandler;
//    private GameData gameData;
//    private List<ISaveable> allSaveables;

//    [SerializeField] private string fileName = "miretalGameData.json";
//    [SerializeField] private bool encryptData = true;

//    private void Awake()
//    {
//        instance = this;
//    }

//    private IEnumerator Start()
//    {
//        Debug.Log(Application.persistentDataPath);
//        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
//        allSaveables = FindISaveable();

//        yield return null;
//        LoadGame();
//    }

//    public void LoadGame()
//    {
//        gameData = dataHandler.LoadData();

//        if (gameData == null)
//        {
//            Debug.Log("No data found. Initializing data to defaults.");
//            gameData = new GameData();
//            return;
//        }
//        foreach (var saveable in allSaveables)
//        {
//            saveable.LoadData(gameData);
//        }
//    }

//    public void SaveGame()
//    {
//        if (gameData == null)
//        {
//            Debug.LogWarning("Cannot save game: gameData is null");
//            return;

//        }
//        foreach (var saveable in allSaveables)
//        {
//            saveable.SaveData(ref gameData);
//        }
//        dataHandler.SaveData(gameData);
//    }

//    public GameData GetGameData() => gameData;

//    private void OnApplicationQuit()
//    {
//        SaveGame();
//    }

//    [ContextMenu("*** Delete Save Data ***")]
//    public void DeleteSaveData()
//    {
//        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
//        dataHandler.DeleteData();

//        LoadGame();
//    }
//    private List<ISaveable> FindISaveable()
//    {
//        return
//            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
//            .OfType<ISaveable>()
//            .ToList();
//    }
//}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private FileDataHandler dataHandler;
    private GameData gameData;
    private List<ISaveable> allSaveables;

    [SerializeField] private string fileName = "miretalGameData.json";
    [SerializeField] private bool encryptData = true;

    private void Awake()
    {
        instance = this;
    }

    private IEnumerator Start()
    {
        Debug.Log(Application.persistentDataPath);
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        allSaveables = FindISaveable();

        yield return null;
        LoadGame();
    }

    public void LoadGame()
    {
        gameData = dataHandler.LoadData();

        if (gameData == null)
        {
            Debug.Log("No data found. Initializing data to defaults.");
            gameData = new GameData();
            return;
        }

        if (allSaveables == null || allSaveables.Count == 0)
        {
            Debug.LogWarning("No saveable objects found. Skipping load.");
            return;
        }

        foreach (var saveable in allSaveables)
        {
            saveable.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        if (gameData == null)
        {
            Debug.LogWarning("Cannot save game: gameData is null");
            return;
        }

        if (allSaveables == null || allSaveables.Count == 0)
        {
            Debug.LogWarning("Cannot save game: No saveable objects found");
            return;
        }

        foreach (var saveable in allSaveables)
        {
            saveable.SaveData(ref gameData);
        }
        dataHandler.SaveData(gameData);
    }

    public GameData GetGameData() => gameData;

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    [ContextMenu("*** Delete Save Data ***")]
    public void DeleteSaveData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.DeleteData();

        LoadGame();
    }
    private List<ISaveable> FindISaveable()
    {
        return
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<ISaveable>()
            .ToList();
    }
}