using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity_DropManager : MonoBehaviour
{
    [SerializeField] private GameObject itemDropPrefab;
    [SerializeField] private ItemListDataSO dropData;

    [Header("Drop Settings")]
    [SerializeField] private int maxRarityAmount = 1200;
    [SerializeField] private int maxItemsToDrop = 3;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            DropItems();
        }
    }
    public virtual void DropItems()
    {
        List<ItemDataSO> itemToDrop = RollDrops();
        int amountToDrop = Mathf.Min(itemToDrop.Count, maxItemsToDrop);

        for(int i = 0; i < amountToDrop; i++)
        {
            CreateItemDrop(itemToDrop[i]);
        }
    }

    public void CreateItemDrop(ItemDataSO itemData)
    {
        GameObject newItem = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
        newItem.GetComponent<Object_ItemPickup>().SetupItem(itemData);
    }

    public List<ItemDataSO> RollDrops()
    {
        List<ItemDataSO> possibleDrops = new List<ItemDataSO>();
        List<ItemDataSO> finalDrops = new List<ItemDataSO>();
        float maxRarityAmount = this.maxRarityAmount;   


        //Setup1: Filter items by rarity
        foreach (var item in dropData.itemList)
        {
            float dropChance = item.GetDropChance();

            if (Random.Range(0,100) <= dropChance)
            {
                possibleDrops.Add(item);
            }
        }
        
        //Setup2: Select final drops
        possibleDrops = possibleDrops.OrderByDescending(item => item.itemRarity).ToList();

        //Setup3: Add items to final drop list
        foreach(var item in possibleDrops)
        {
            if(maxRarityAmount > item.itemRarity)
            {
                finalDrops.Add(item);
                maxRarityAmount -= item.itemRarity;
            }
        }
        return finalDrops;
    }

}
