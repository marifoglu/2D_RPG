using System;
using UnityEngine;

[Serializable]
public class SaveSlotData
{
    public int slotIndex;
    public string saveName;
    public string lastSceneName;
    public string saveDateTime;
    public float playTime; // in seconds
    public int playerLevel; // if you have leveling system
    public int gold;
    public bool isEmpty;

    public SaveSlotData(int slotIndex)
    {
        this.slotIndex = slotIndex;
        this.isEmpty = true;
        this.saveName = "";
        this.lastSceneName = "";
        this.saveDateTime = "";
        this.playTime = 0f;
        this.playerLevel = 1;
        this.gold = 0;
    }

    public void UpdateFromGameData(GameData gameData, string sceneName)
    {
        isEmpty = false;
        lastSceneName = sceneName;
        saveDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
        gold = gameData.gold;
        // Add player level if you implement leveling system
        // playerLevel = gameData.playerLevel;
    }

    public string GetDisplayName()
    {
        if (isEmpty)
            return "Empty Slot";

        return $"Slot {slotIndex + 1} - {lastSceneName}";
    }

    public string GetDisplayInfo()
    {
        if (isEmpty)
            return "No save data";

        string timeStr = FormatPlayTime(playTime);
        return $"{saveDateTime} | Gold: {gold} | Time: {timeStr}";
    }

    private string FormatPlayTime(float seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        if (time.TotalHours >= 1)
            return $"{(int)time.TotalHours}h {time.Minutes}m";
        else
            return $"{time.Minutes}m {time.Seconds}s";
    }
}