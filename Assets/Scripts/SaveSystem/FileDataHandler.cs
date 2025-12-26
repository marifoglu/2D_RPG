using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string fullPath;
    private bool encryptData;
    private string codeWord = "Mirethal";

    public FileDataHandler(string dataDirPath, string dataFileName, bool encryptData)
    {
        fullPath = Path.Combine(dataDirPath, dataFileName);
        this.encryptData = encryptData;
    }

    public void SaveData(GameData gameData)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            
            string dataToSave = JsonUtility.ToJson(gameData, true);

            if (encryptData)
                dataToSave = EncryptDecrypt(dataToSave);
            

            using(FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream))
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

    public GameData LoadData()
    {
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

                // Validate JSON before parsing
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

                // Create backup of corrupted file
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

    public void DeleteData()
    {
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

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
