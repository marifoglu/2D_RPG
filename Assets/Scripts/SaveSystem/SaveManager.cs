using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private FileDataHandler dataHandler;
    private GameData gameData;
    private List<ISaveable> allSaveables;

    [SerializeField] private string fileName = "mirethalGameData.json";
    [SerializeField] private bool encryptData = true;

    public const int MAX_SAVE_SLOTS = 5;

    // Current active slot (kept for compatibility, but we use single file)
    private int currentSlotIndex = 0;
    public int CurrentSlotIndex => currentSlotIndex;

    // Slot metadata (kept for compatibility)
    private SaveSlotData[] saveSlots;
    public SaveSlotData[] SaveSlots => saveSlots;

    // Play time tracking
    private float sessionStartTime;
    private float totalPlayTime;

    public event Action OnSaveCompleted;
    public event Action OnLoadCompleted;
    public event Action<int> OnSlotDeleted;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This is called every time a scene loads
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SaveManager] OnSceneLoaded: {scene.name}");

        if (scene.name != "MainMenu")
        {
            StartCoroutine(LoadAfterSceneReady());
        }
    }

    private IEnumerator LoadAfterSceneReady()
    {
        yield return null; // Wait one frame for all objects to initialize
        LoadGame();
        Debug.Log("Auto-loaded save data after scene change");
    }

    private IEnumerator Start()
    {
        Debug.Log("Save Directory: " + Application.persistentDataPath);
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);

        // Initialize slot metadata for compatibility
        saveSlots = new SaveSlotData[MAX_SAVE_SLOTS];
        for (int i = 0; i < MAX_SAVE_SLOTS; i++)
        {
            saveSlots[i] = new SaveSlotData(i);
        }

        allSaveables = FindISaveable();
        sessionStartTime = Time.time;

        yield return null;

    }

    #region Save Operations

    public void SaveGame()
    {
        if (gameData == null)
            gameData = new GameData();

        allSaveables = FindISaveable();

        Debug.Log($"[SaveManager] Saving - Found {allSaveables?.Count ?? 0} ISaveable objects:");

        if (allSaveables != null && allSaveables.Count > 0)
        {
            foreach (var saveable in allSaveables)
            {
                if (saveable != null)
                {
                    Debug.Log($"  - Saving: {saveable.GetType().Name}");
                    saveable.SaveData(ref gameData);
                }
            }
        }

        Debug.Log($"[SaveManager] After save - skillPoints: {gameData.skillPoints}, experience: {gameData.currentExperience}");

        dataHandler.SaveData(gameData, -1);

        Debug.Log("Game saved to: " + fileName);
        OnSaveCompleted?.Invoke();
    }

    public void SaveGame(int slotIndex)
    {
        SaveGame();
    }

    #endregion

    #region Load Operations

    public void LoadGame()
    {
        gameData = dataHandler.LoadData(-1);

        if (gameData == null)
        {
            Debug.Log("No save file found. Creating new game data.");
            gameData = new GameData();
            SaveGame(); // Create the file immediately
        }

        Debug.Log($"[SaveManager] Loaded GameData - skillPoints: {gameData.skillPoints}, experience: {gameData.currentExperience}");

        allSaveables = FindISaveable();

        Debug.Log($"[SaveManager] Found {allSaveables?.Count ?? 0} ISaveable objects:");
        if (allSaveables != null)
        {
            foreach (var saveable in allSaveables)
            {
                if (saveable != null)
                {
                    Debug.Log($"  - {saveable.GetType().Name}");
                    saveable.LoadData(gameData);
                }
            }
        }

        sessionStartTime = Time.time;

        Debug.Log("Game loaded from: " + fileName);

        OnLoadCompleted?.Invoke();
    }

    public void LoadGame(int slotIndex)
    {
        LoadGame();
    }

    public void StartNewGame(int slotIndex)
    {
        NewGame();
    }

    public void NewGame()
    {
        dataHandler.DeleteData(-1);
        gameData = new GameData();
        SaveGame();
        Debug.Log("New game started");
    }

    public void ContinueGame(int slotIndex)
    {
        LoadGame();
    }

    #endregion

    #region Slot Management (kept for compatibility)

    public SaveSlotData GetSlotData(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS)
            return null;
        return saveSlots[slotIndex];
    }

    public bool IsSlotEmpty(int slotIndex)
    {
        // Check if actual save file exists
        return !dataHandler.SaveExists(-1);
    }

    public void SetCurrentSlot(int slotIndex)
    {
        currentSlotIndex = slotIndex;
    }

    public void RefreshSlotMetadata()
    {
        // Update slot 0 based on actual file
        if (dataHandler.SaveExists(-1))
        {
            saveSlots[0].isEmpty = false;
            saveSlots[0].lastSceneName = SceneManager.GetActiveScene().name;
            saveSlots[0].saveDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
        }
        else
        {
            saveSlots[0].isEmpty = true;
        }
    }

    #endregion

    #region Delete Operations

    public void DeleteSlot(int slotIndex)
    {
        DeleteSaveData();
        OnSlotDeleted?.Invoke(slotIndex);
    }

    [ContextMenu("Delete Save Data")]
    public void DeleteSaveData()
    {
        if (dataHandler == null)
        {
            Debug.LogWarning("[SaveManager] DeleteSaveData called before initialization. Skipping delete.");
            return;
        }

        dataHandler.DeleteData(-1);
        gameData = null;
        Debug.Log("Save data deleted");
    }

    [ContextMenu("*** Delete All Save Data ***")]
    public void DeleteAllSaveData()
    {
        DeleteSaveData();
    }

    #endregion

    #region Utility

    public GameData GetGameData()
    {
        if (gameData == null)
            gameData = new GameData();
        return gameData;
    }

    public float GetTotalPlayTime()
    {
        float currentSession = Time.time - sessionStartTime;
        return totalPlayTime + currentSession;
    }

    public bool HasAnySaveData()
    {
        return dataHandler.SaveExists(-1);
    }

    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            SaveGame();
            Debug.Log("Auto-saved on quit");
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && SceneManager.GetActiveScene().name != "MainMenu")
        {
            SaveGame();
        }
    }

    private List<ISaveable> FindISaveable()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<ISaveable>()
            .ToList();
    }

    #endregion
}