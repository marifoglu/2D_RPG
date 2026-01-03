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

    [SerializeField] private string fileName = "miretalGameData.json";
    [SerializeField] private bool encryptData = true;

    public const int MAX_SAVE_SLOTS = 5;

    // Current active slot (-1 means no slot selected / new game)
    private int currentSlotIndex = -1;
    public int CurrentSlotIndex => currentSlotIndex;

    // Slot metadata
    private SaveSlotData[] saveSlots;
    public SaveSlotData[] SaveSlots => saveSlots;

    // Play time tracking
    private float sessionStartTime;
    private float totalPlayTime;

    // Events
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

    private IEnumerator Start()
    {
        Debug.Log("Save Directory: " + Application.persistentDataPath);
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);

        // Load slot metadata
        saveSlots = dataHandler.LoadSlotMetadata();

        allSaveables = FindISaveable();
        sessionStartTime = Time.time;

        yield return null;

        // Don't auto-load in main menu - wait for player to select a slot
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            // If we have an active slot, load it
            if (currentSlotIndex >= 0)
            {
                LoadGame(currentSlotIndex);
            }
        }
    }

    #region Slot Management

    public SaveSlotData GetSlotData(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS)
            return null;

        return saveSlots[slotIndex];
    }

    public bool IsSlotEmpty(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS)
            return true;

        return saveSlots[slotIndex].isEmpty;
    }

    public void SetCurrentSlot(int slotIndex)
    {
        currentSlotIndex = slotIndex;
    }

    #endregion

    #region Save Operations

    public void SaveGame()
    {
        if (currentSlotIndex < 0)
        {
            Debug.LogWarning("No save slot selected. Cannot save.");
            return;
        }

        SaveGame(currentSlotIndex);
    }

    public void SaveGame(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS)
        {
            Debug.LogError($"Invalid slot index: {slotIndex}");
            return;
        }

        if (gameData == null)
        {
            gameData = new GameData();
        }

        // Refresh saveable list
        allSaveables = FindISaveable();

        if (allSaveables == null || allSaveables.Count == 0)
        {
            Debug.LogWarning("No saveable objects found");
        }
        else
        {
            foreach (var saveable in allSaveables)
            {
                saveable.SaveData(ref gameData);
            }
        }

        // Save game data to slot
        dataHandler.SaveData(gameData, slotIndex);

        // Update slot metadata
        UpdateSlotMetadata(slotIndex);

        currentSlotIndex = slotIndex;

        Debug.Log($"Game saved to slot {slotIndex}");
        OnSaveCompleted?.Invoke();
    }

    private void UpdateSlotMetadata(int slotIndex)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // Calculate total play time
        float currentSessionTime = Time.time - sessionStartTime;
        saveSlots[slotIndex].playTime += currentSessionTime;
        sessionStartTime = Time.time; // Reset for next save

        saveSlots[slotIndex].UpdateFromGameData(gameData, currentScene);
        dataHandler.SaveSlotMetadata(saveSlots);
    }

    #endregion

    #region Load Operations

    public void LoadGame()
    {
        if (currentSlotIndex < 0)
        {
            Debug.LogWarning("No save slot selected. Starting new game.");
            gameData = new GameData();
            return;
        }

        LoadGame(currentSlotIndex);
    }

    public void LoadGame(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS)
        {
            Debug.LogError($"Invalid slot index: {slotIndex}");
            return;
        }

        gameData = dataHandler.LoadData(slotIndex);

        if (gameData == null)
        {
            Debug.Log($"No data found in slot {slotIndex}. Initializing defaults.");
            gameData = new GameData();
            return;
        }

        // Refresh saveable list
        allSaveables = FindISaveable();

        if (allSaveables == null || allSaveables.Count == 0)
        {
            Debug.LogWarning("No saveable objects found. Skipping load.");
            return;
        }

        foreach (var saveable in allSaveables)
        {
            saveable.LoadData(gameData);
        }

        currentSlotIndex = slotIndex;
        sessionStartTime = Time.time;
        totalPlayTime = saveSlots[slotIndex].playTime;

        Debug.Log($"Game loaded from slot {slotIndex}");
        OnLoadCompleted?.Invoke();
    }

    public void StartNewGame(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS)
        {
            Debug.LogError($"Invalid slot index: {slotIndex}");
            return;
        }

        // Delete any existing data in this slot
        if (!saveSlots[slotIndex].isEmpty)
        {
            DeleteSlot(slotIndex);
        }

        currentSlotIndex = slotIndex;
        gameData = new GameData();
        sessionStartTime = Time.time;
        totalPlayTime = 0f;

        // Initialize the slot metadata
        saveSlots[slotIndex] = new SaveSlotData(slotIndex);
        saveSlots[slotIndex].isEmpty = false;
        saveSlots[slotIndex].saveDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
        saveSlots[slotIndex].lastSceneName = "New Game";

        dataHandler.SaveSlotMetadata(saveSlots);

        Debug.Log($"New game started in slot {slotIndex}");
    }

    public void ContinueGame(int slotIndex)
    {
        if (IsSlotEmpty(slotIndex))
        {
            Debug.LogWarning($"Slot {slotIndex} is empty. Cannot continue.");
            return;
        }

        currentSlotIndex = slotIndex;
        LoadGame(slotIndex);
    }

    #endregion

    #region Delete Operations

    public void DeleteSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS)
        {
            Debug.LogError($"Invalid slot index: {slotIndex}");
            return;
        }

        // Delete the save file
        dataHandler.DeleteData(slotIndex);

        // Reset slot metadata
        saveSlots[slotIndex] = new SaveSlotData(slotIndex);
        dataHandler.SaveSlotMetadata(saveSlots);

        // If this was the current slot, reset it
        if (currentSlotIndex == slotIndex)
        {
            currentSlotIndex = -1;
            gameData = null;
        }

        Debug.Log($"Slot {slotIndex} deleted");
        OnSlotDeleted?.Invoke(slotIndex);
    }

    [ContextMenu("*** Delete All Save Data ***")]
    public void DeleteAllSaveData()
    {
        for (int i = 0; i < MAX_SAVE_SLOTS; i++)
        {
            DeleteSlot(i);
        }

        currentSlotIndex = -1;
        gameData = null;
    }

    #endregion

    #region Utility

    public GameData GetGameData() => gameData;

    public float GetTotalPlayTime()
    {
        float currentSession = Time.time - sessionStartTime;
        return totalPlayTime + currentSession;
    }

    private void OnApplicationQuit()
    {
        // Auto save on quit if we have an active slot
        if (currentSlotIndex >= 0 && SceneManager.GetActiveScene().name != "MainMenu")
        {
            SaveGame(currentSlotIndex);
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        // Auto-save on mobile when app is paused
        if (pauseStatus && currentSlotIndex >= 0 && SceneManager.GetActiveScene().name != "MainMenu")
        {
            SaveGame(currentSlotIndex);
        }
    }

    private List<ISaveable> FindISaveable()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<ISaveable>()
            .ToList();
    }

    public void RefreshSlotMetadata()
    {
        saveSlots = dataHandler.LoadSlotMetadata();
    }

    #endregion
}