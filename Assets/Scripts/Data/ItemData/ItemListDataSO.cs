using UnityEngine;

[CreateAssetMenu(fileName = "List of items - ", menuName = "RPG Setup/Item data/Item list")]
public class ItemListDataSO : ScriptableObject
{
    public ItemDataSO[] items;
}
