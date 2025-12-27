//using System.Collections;
//using UnityEngine;
//using System.Linq;
//using UnityEngine.SceneManagement;

//public class GameManager : MonoBehaviour, ISaveable
//{
//    public static GameManager instance;
//    private Vector3 lastPlayerPosition;

//    private string lastScenePlayed;
//    private bool dataLoaded;

//    private void Awake()
//    {
//        if (instance != null && instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        instance = this;
//        DontDestroyOnLoad(gameObject);
//    }

//    public void ContinuePlay()
//    {
//        if (string.IsNullOrEmpty(lastScenePlayed))
//        {
//            Debug.LogWarning("No last scene found. Starting from default scene.");
//            lastScenePlayed = "Demo_Level_0";
//        }

//        ChangeScene(lastScenePlayed, RespawnType.NoneSpecific);
//    }

//    public void RestartScene()
//    {
//        string sceneName = SceneManager.GetActiveScene().name;
//        ChangeScene(sceneName, RespawnType.NoneSpecific);
//    }

//    public void ChangeScene(string sceneName, RespawnType respwanType)
//    {
//        if (string.IsNullOrEmpty(sceneName))
//        {
//            Debug.LogError("Cannot change to scene with empty name!");
//            return;
//        }

//        SaveManager.instance.SaveGame();

//        Time.timeScale = 1;
//        StartCoroutine(ChangeSceneCo(sceneName, respwanType));
//    }

//    private IEnumerator ChangeSceneCo(string sceneName, RespawnType respawnType)
//    {
//        UI_FadeScreen fadeScreen = FindFadeScreenUI();

//        // If death already faded to black, skip the fade-out!
//        if (fadeScreen.skipNextFadeOut)
//        {
//            // Already black from death fade - just make sure it stays black
//            fadeScreen.SetToBlack();
//            fadeScreen.skipNextFadeOut = false; // Reset the flag
//            // No need to wait - we're already black!
//        }
//        else
//        {
//            // Normal transition: fade to black
//            fadeScreen.DoFadeOut();
//            yield return fadeScreen.fadeEffectCo;
//        }

//        SceneManager.LoadScene(sceneName);

//        dataLoaded = false;
//        yield return null;

//        while (dataLoaded == false)
//        {
//            yield return null;
//        }

//        fadeScreen = FindFadeScreenUI();
//        fadeScreen.DoFadeIn(); // black > transperent

//        Player player = Player.instance;

//        if (player == null)
//            yield break;

//        Vector3 position = GetNewPlayerPosition(respawnType);

//        if (position != Vector3.zero)
//            player.TeleportPlayer(position);
//    }

//    private UI_FadeScreen FindFadeScreenUI()
//    {
//        if (UI.instance != null)
//            return UI.instance.fadeScreenUI;
//        else
//            return FindFirstObjectByType<UI_FadeScreen>();
//    }

//    private Vector3 GetNewPlayerPosition(RespawnType type)
//    {
//        if (type == RespawnType.Portal)
//        {
//            Object_Portal portal = Object_Portal.instance;

//            Vector3 position = portal.GetPosition();

//            portal.SetTrigger(false);
//            portal.DisableIfNeeded();

//            return position;
//        }

//        if (type == RespawnType.NoneSpecific)
//        {
//            var data = SaveManager.instance.GetGameData();
//            var checkpoints = FindObjectsByType<Object_Checkpoint>(FindObjectsSortMode.None);
//            var unlockedCheckpoints = checkpoints
//                .Where(cp => data.unlockedCheckpoints.TryGetValue(cp.GetCheckpointID(), out bool unlocked) && unlocked)
//                .Select(cp => cp.GetPosition())
//                .ToList();

//            var enterWaypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None)
//                .Where(wp => wp.GetWaypointType() == RespawnType.Enter)
//                .Select(wp => wp.GetPositionAndSetTriggerFalse())
//                .ToList();

//            var selectedPositions = unlockedCheckpoints.Concat(enterWaypoints).ToList();

//            if (selectedPositions.Count == 0)
//                return Vector3.zero;

//            return selectedPositions.
//                OrderBy(position => Vector3.Distance(position, lastPlayerPosition))
//                .First();
//        }

//        return GetWaypointPosition(type);
//    }

//    private Vector3 GetWaypointPosition(RespawnType type)
//    {
//        var waypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None);

//        foreach (var point in waypoints)
//        {
//            if (point.GetWaypointType() == type)
//                return point.GetPositionAndSetTriggerFalse();
//        }

//        return Vector3.zero;
//    }

//    public void LoadData(GameData data)
//    {
//        lastScenePlayed = data.lastScenePlayed;
//        lastPlayerPosition = data.lastPlayerPosition;

//        if (string.IsNullOrEmpty(lastScenePlayed))
//            lastScenePlayed = "Demo_Level_0";

//        dataLoaded = true;
//    }

//    public void SaveData(ref GameData data)
//    {
//        string currentScene = SceneManager.GetActiveScene().name;

//        if (currentScene == "MainMenu")
//            return;

//        data.lastPlayerPosition = Player.instance.transform.position;
//        data.lastScenePlayed = currentScene;
//        dataLoaded = false;
//    }
//}

using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveable
{
    public static GameManager instance;
    private Vector3 lastPlayerPosition;

    private string lastScenePlayed;
    private bool dataLoaded;

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

    public void ContinuePlay()
    {
        if (string.IsNullOrEmpty(lastScenePlayed))
        {
            Debug.LogWarning("No last scene found. Starting from default scene.");
            lastScenePlayed = "Demo_Level_0";
        }

        ChangeScene(lastScenePlayed, RespawnType.NoneSpecific);
    }

    public void RestartScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        ChangeScene(sceneName, RespawnType.NoneSpecific);
    }

    public void ChangeScene(string sceneName, RespawnType respwanType)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Cannot change to scene with empty name!");
            return;
        }

        SaveManager.instance.SaveGame();

        Time.timeScale = 1;
        StartCoroutine(ChangeSceneCo(sceneName, respwanType));
    }

    private IEnumerator ChangeSceneCo(string sceneName, RespawnType respawnType)
    {
        UI_FadeScreen fadeScreen = FindFadeScreenUI();

        // CHECK THE FLAG: If death already faded to black, skip the fade-out!
        if (fadeScreen.skipNextFadeOut)
        {
            // Already black from death fade - just make sure it stays black
            fadeScreen.SetToBlack();
            fadeScreen.skipNextFadeOut = false; // Reset the flag
            // No need to wait - we're already black!
        }
        else
        {
            // Normal transition: fade to black
            fadeScreen.DoFadeOut();
            yield return fadeScreen.fadeEffectCo;
        }

        SceneManager.LoadScene(sceneName);

        dataLoaded = false;
        yield return null;

        while (dataLoaded == false)
        {
            yield return null;
        }

        fadeScreen = FindFadeScreenUI();
        fadeScreen.DoFadeIn(); // black > transperent

        Player player = Player.instance;

        if (player == null)
            yield break;

        Vector3 position = GetNewPlayerPosition(respawnType);

        if (position != Vector3.zero)
            player.TeleportPlayer(position);
    }

    private UI_FadeScreen FindFadeScreenUI()
    {
        if (UI.instance != null)
            return UI.instance.fadeScreenUI;
        else
            return FindFirstObjectByType<UI_FadeScreen>();
    }

    private Vector3 GetNewPlayerPosition(RespawnType type)
    {
        if (type == RespawnType.Portal)
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

            return selectedPositions.
                OrderBy(position => Vector3.Distance(position, lastPlayerPosition))
                .First();
        }

        return GetWaypointPosition(type);
    }

    private Vector3 GetWaypointPosition(RespawnType type)
    {
        var waypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None);

        foreach (var point in waypoints)
        {
            if (point.GetWaypointType() == type)
                return point.GetPositionAndSetTriggerFalse();
        }

        return Vector3.zero;
    }

    public void LoadData(GameData data)
    {
        lastScenePlayed = data.lastScenePlayed;
        lastPlayerPosition = data.lastPlayerPosition;

        if (string.IsNullOrEmpty(lastScenePlayed))
            lastScenePlayed = "Demo_Level_0";

        dataLoaded = true;
    }

    public void SaveData(ref GameData data)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "MainMenu")
            return;

        data.lastPlayerPosition = Player.instance.transform.position;
        data.lastScenePlayed = currentScene;
        dataLoaded = false;
    }
}