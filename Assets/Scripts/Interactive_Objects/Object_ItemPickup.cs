using UnityEngine;

public class Object_ItemPickup : MonoBehaviour
{
    
    private SpriteRenderer sr;

    [SerializeField] private ItemDataSO itemData;

    private void OnValidate()
    {
        if (itemData == null)
            return;

        sr = GetComponent<SpriteRenderer>();
        sr.sprite = itemData.itemIcon;

        gameObject.name = $"Object_ItemPickup_{itemData.itemName}";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Picked up item: {itemData.itemName}");
        Destroy(gameObject);
    }

}
