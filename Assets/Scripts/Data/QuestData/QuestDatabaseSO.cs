using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Quest Data/Quest Database", fileName = "Quest Database ")]
public class QuestDatabaseSO : ScriptableObject
{
    public QuestDataSO[] allQuests;


    public QuestDataSO GetQuestById(string id)
    {
        return allQuests.FirstOrDefault(q => q != null && q.questSaveID == id);
    }

#if UNITY_EDITOR
    [ContextMenu("Auto-fill with all QuestDataSO")]
    public void CollectItemsData()
    {
        string[] guids = AssetDatabase.FindAssets("t:QuestDataSO");

        allQuests = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<QuestDataSO>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(q => q != null)
            .ToArray();

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
