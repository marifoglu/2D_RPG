using UnityEditor;
using UnityEngine;

public enum RewardType { Merchant, Blacksmith, None };
public enum QuestType { Main, Side, Kill, Talk, Delivery };


[CreateAssetMenu(menuName = "RPG Setup/Quest Data/New Quest", fileName = "Quest - ")]
public class QuestDataSO : ScriptableObject
{
    public string questSaveID;
    [Space]
    public QuestType questType;
    public string questName;
    [TextArea] public string questDescription;
    [TextArea] public string questGoal;

    public string questTargetID; // Enemy name, NPC name or Itemname etc..
    public int requiredAmount;
    public ItemDataSO itemToDelivery; // only for delivery quests

    [Header("Reward")]
    public RewardType rewardType;
    public Inventory_Item[] rewardItems;

    private void OnValidate()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        questSaveID = AssetDatabase.AssetPathToGUID(path);
#endif

    }
}
