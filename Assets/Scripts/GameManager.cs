using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveable
{
    public static GameManager instance;
    private Vector3 lastPlayerPosition;

    private string lastScenePlayed;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void RestartScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        ChangeScene(sceneName, RespawnType.NoneSpecific);
    }
    public void ChangeScene(string sceneName, RespawnType respawnType)
    {
        SaveManager.instance.SaveGame();
        StartCoroutine(ChangeSceneCo(sceneName, respawnType));
    }
    //public void SetLastPlayerPosition(Vector3 position) => lastPlayerPosition = position;

    public void ContinuePlay()
    {
        ChangeScene(lastScenePlayed, RespawnType.NoneSpecific);
    }
    private IEnumerator ChangeSceneCo(string sceneName, RespawnType respawnType)
    {
        // Fade Effect

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneName);

        yield return new WaitForSeconds(.2f);

        Vector3 position = GetNewPlayerPosition(respawnType);

        if(position != Vector3.zero)
        {
            Player.instance.TeleportPlayer(position);
        }
    }
    private Vector3 GetWaypointPosition(RespawnType type)
    {
        var wayPoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None);

        foreach (var point in wayPoints)
        {
            if(point.GetWaypointType() == type)
                return point.GetPositionAndSetTriggerFalse();
        }
        return Vector3.zero;
    }
    private Vector3 GetNewPlayerPosition(RespawnType type) 
    {
        if(type == RespawnType.Portal)
        {
            Object_Portal portal = Object_Portal.instance;

            Vector3 position = portal.GetPosition();

            portal.SetTrigger(false);
            portal.DisableIfNeeded();

            return position;    
        }

        if (type == RespawnType.NoneSpecific)
        {
            var data = SaveManager.instance.GetGameData();
            var checkpoints = FindObjectsByType<Object_Checkpoint>(FindObjectsSortMode.None);
            var unlockedCheckpoints = checkpoints
                .Where(cp => data.unlockedCheckpoints.TryGetValue(cp.GetCheckpointID(), out bool unlocked) && unlocked)
                .Select(cp => cp.GetPosition())
                .ToList();

            var enterWaypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None)
           .Where(wp => wp.GetWaypointType() == RespawnType.Enter)
           .Select(wp => wp.GetPositionAndSetTriggerFalse())
           .ToList();

            var selectedPositions = unlockedCheckpoints.Concat(enterWaypoints).ToList();

            if (selectedPositions.Count == 0)
                return Vector3.zero;
            
            return selectedPositions.OrderBy(position => Vector3.Distance(position, lastPlayerPosition)).First();
        }
        return GetWaypointPosition(type);
    }

    public void SaveData(ref GameData gameData)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "MainMenu")
            return;

        gameData.lastPlayerPosition = Player.instance.transform.position;
        gameData.lastScenePlayed = currentScene;
    }

    public void LoadData(GameData gameData)
    {
        lastPlayerPosition = gameData.lastPlayerPosition;
        lastScenePlayed = gameData.lastScenePlayed;

        if(string.IsNullOrEmpty(lastScenePlayed))
            lastScenePlayed = "Demo_Level_0";
    }
}
