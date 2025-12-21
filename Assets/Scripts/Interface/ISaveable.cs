using UnityEngine;

public interface ISaveable
{
    public void SaveData(ref GameData gameData);

    public void LoadData(GameData gameData);
}
