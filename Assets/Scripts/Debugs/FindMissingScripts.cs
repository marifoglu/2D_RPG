using UnityEngine;

public class FindMissingScripts : MonoBehaviour
{
    [ContextMenu("Find Missing Scripts")]
    void FindMissingScriptsInScene()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int count = 0;

        foreach (GameObject obj in allObjects)
        {
            Component[] components = obj.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    count++;
                    Debug.LogError($"Missing script found on: {GetGameObjectPath(obj)} (Component index: {i})", obj);
                }
            }
        }

        if (count == 0)
            Debug.Log("No missing scripts found!");
        else
            Debug.LogWarning($"Found {count} missing script(s)!");
    }

    private string GetGameObjectPath(GameObject obj)
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