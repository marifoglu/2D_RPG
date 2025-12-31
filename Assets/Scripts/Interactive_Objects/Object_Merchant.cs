using UnityEngine;

public class Object_Merchant : Object_NPC, IInteractable
{
    [Header("Quest & Dialog")]
    [SerializeField] private QuestDataSO[] quest;

    private Inventory_Player inventory;
    private Inventory_Merchant merchant;

    protected override void Awake()
    {
        base.Awake();
        merchant = GetComponent<Inventory_Merchant>();
    }

    public override void Interact()
    {
        base.Interact();
        ui.OpenQuestUI(quest);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.CompareTag("Player"))
        {
            inventory = collision.GetComponent<Inventory_Player>();
            if (inventory != null)
                merchant.SetInventory(inventory);
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        if (collision.CompareTag("Player"))
        {
            if (ui != null)
            {
                ui.HideAllTooltips();
                ui.OpenMerchantUI(false);
            }

            inventory = null;
        }
    }
}