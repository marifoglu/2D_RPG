using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string dataDirPath;
    private string baseFileName;
    private bool encryptData;
    private string codeWord = "Mirethal";

    private const int MAX_SAVE_SLOTS = 5;
    private const string SLOT_METADATA_FILE = "saveSlotMetadata.json";

    public FileDataHandler(string dataDirPath, string dataFileName, bool encryptData)
    {
        this.dataDirPath = dataDirPath;
        this.baseFileName = dataFileName;
        this.encryptData = encryptData;
    }

    #region Single File Methods (Legacy Support)

    private string GetFullPath(int slotIndex = -1)
    {
        if (slotIndex < 0)
            return Path.Combine(dataDirPath, baseFileName);

        string fileName = $"save_slot_{slotIndex}_{baseFileName}";
        return Path.Combine(dataDirPath, fileName);
    }

    public void SaveData(GameData gameData)
    {
        SaveData(gameData, -1);
    }

    public GameData LoadData()
    {
        return LoadData(-1);
    }

    public void DeleteData()
    {
        DeleteData(-1);
    }

    #endregion

    #region Multi-Slot Methods

    public void SaveData(GameData gameData, int slotIndex)
    {
        string fullPath = GetFullPath(slotIndex);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToSave = JsonUtility.ToJson(gameData, true);

            if (encryptData)
                dataToSave = EncryptDecrypt(dataToSave);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToSave);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving data to file: " + fullPath + " \n " + e);
        }
    }

    public GameData LoadData(int slotIndex)
    {
        string fullPath = GetFullPath(slotIndex);
        GameData loadData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (encryptData)
                    dataToLoad = EncryptDecrypt(dataToLoad);

                if (string.IsNullOrWhiteSpace(dataToLoad))
                {
                    Debug.LogWarning("Save file is empty. Creating new save data.");
                    return null;
                }

                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading data from file: " + fullPath + " \n " + e);
                Debug.LogWarning("Save file is corrupted. Creating backup and starting fresh.");

                try
                {
                    string backupPath = fullPath + ".corrupted." + DateTime.Now.ToString("yyyyMMddHHmmss");
                    File.Copy(fullPath, backupPath);
                    Debug.Log($"Corrupted save backed up to: {backupPath}");
                    File.Delete(fullPath);
                }
                catch (Exception backupException)
                {
                    Debug.LogError("Failed to backup corrupted file: " + backupException);
                }
            }
        }
        return loadData;
    }

    public void DeleteData(int slotIndex)
    {
        string fullPath = GetFullPath(slotIndex);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            Debug.Log($"Deleted save file: {fullPath}");
        }
    }

    public bool SaveExists(int slotIndex)
    {
        string fullPath = GetFullPath(slotIndex);
        return File.Exists(fullPath);
    }

    #endregion

    #region Slot Metadata

    public void SaveSlotMetadata(SaveSlotData[] slots)
    {
        string metadataPath = Path.Combine(dataDirPath, SLOT_METADATA_FILE);

        try
        {
            Directory.CreateDirectory(dataDirPath);

            SaveSlotMetadataWrapper wrapper = new SaveSlotMetadataWrapper { slots = slots };
            string json = JsonUtility.ToJson(wrapper, true);

            File.WriteAllText(metadataPath, json);
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving slot metadata: " + e);
        }
    }

    public SaveSlotData[] LoadSlotMetadata()
    {
        string metadataPath = Path.Combine(dataDirPath, SLOT_METADATA_FILE);
        SaveSlotData[] slots = new SaveSlotData[MAX_SAVE_SLOTS];

        // Initialize empty slots
        for (int i = 0; i < MAX_SAVE_SLOTS; i++)
        {
            slots[i] = new SaveSlotData(i);
        }

        if (File.Exists(metadataPath))
        {
            try
            {
                string json = File.ReadAllText(metadataPath);
                SaveSlotMetadataWrapper wrapper = JsonUtility.FromJson<SaveSlotMetadataWrapper>(json);

                if (wrapper != null && wrapper.slots != null)
                {
                    for (int i = 0; i < Mathf.Min(wrapper.slots.Length, MAX_SAVE_SLOTS); i++)
                    {
                        slots[i] = wrapper.slots[i];
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading slot metadata: " + e);
            }
        }

        // Verify slots match actual files
        for (int i = 0; i < MAX_SAVE_SLOTS; i++)
        {
            if (!SaveExists(i))
            {
                slots[i] = new SaveSlotData(i);
            }
        }

        return slots;
    }

    public void DeleteSlotMetadata(int slotIndex, SaveSlotData[] slots)
    {
        if (slotIndex >= 0 && slotIndex < slots.Length)
        {
            slots[slotIndex] = new SaveSlotData(slotIndex);
            SaveSlotMetadata(slots);
        }
    }

    #endregion

    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";

        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ codeWord[i % codeWord.Length]);
        }
        return modifiedData;
    }
}

[Serializable]
public class SaveSlotMetadataWrapper
{
    public SaveSlotData[] slots;
}