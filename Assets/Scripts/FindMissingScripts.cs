using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class FindMissingScripts : EditorWindow
{
    [MenuItem("Tools/Find Missing Scripts")]
    static void ShowWindow()
    {
        GetWindow<FindMissingScripts>("Find Missing Scripts");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Find Missing Scripts in Scene"))
        {
            FindInScene();
        }

        if (GUILayout.Button("Find Missing Scripts in All Assets"))
        {
            FindInAssets();
        }
    }

    static void FindInScene()
    {
        int missingCount = 0;
        GameObject[] gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        
        foreach (GameObject go in gameObjects)
        {
            missingCount += FindMissingScriptsRecursive(go);
        }

        Debug.Log($"Found {missingCount} missing scripts in active scene");
    }

    static void FindInAssets()
    {
        string[] prefabPaths = AssetDatabase.GetAllAssetPaths();
        int missingCount = 0;

        foreach (string path in prefabPaths)
        {
            if (path.EndsWith(".prefab"))
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    int count = FindMissingScriptsRecursive(prefab);
                    if (count > 0)
                    {
                        Debug.Log($"Found {count} missing scripts in: {path}", prefab);
                        missingCount += count;
                    }
                }
            }
        }

        Debug.Log($"Total missing scripts in assets: {missingCount}");
    }

    static int FindMissingScriptsRecursive(GameObject go)
    {
        int count = 0;
        Component[] components = go.GetComponents<Component>();

        foreach (Component component in components)
        {
            if (component == null)
            {
                count++;
                Debug.Log($"Missing script on: {GetGameObjectPath(go)}", go);
            }
        }

        foreach (Transform child in go.transform)
        {
            count += FindMissingScriptsRecursive(child.gameObject);
        }

        return count;
    }

    static string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;

        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }

        return path;
    }
}