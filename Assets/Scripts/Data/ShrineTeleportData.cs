using UnityEngine;

[System.Serializable]
public class ShrineTeleportData
{
    public string shrineID;
    public string shrineName;
    public string sceneName;
    public float teleportX;
    public float teleportY;
    public float teleportZ;

    public Vector3 TeleportPosition => new Vector3(teleportX, teleportY, teleportZ);

    public ShrineTeleportData() { }

    public ShrineTeleportData(string id, string name, string scene, Vector3 position)
    {
        shrineID = id;
        shrineName = name;
        sceneName = scene;
        teleportX = position.x;
        teleportY = position.y;
        teleportZ = position.z;
    }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(shrineID) && !string.IsNullOrEmpty(sceneName);
    }
}