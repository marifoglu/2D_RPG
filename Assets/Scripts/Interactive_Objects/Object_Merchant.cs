using UnityEngine;

public class Object_Merchant : Object_NPC, IInteractable
{
    [Header("Quest & Dialog")]
    [SerializeField] private DialogueLineSO firstDialogueLine;
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

        // Find player inventory if not already cached
        if (inventory == null)
        {
            inventory = Player.instance?.inventory;
        }

        // Validate before setting up UI
        if (inventory == null)
        {
            Debug.LogWarning("Cannot interact with merchant: Player inventory not found!");
            return;
        }

        if (merchant == null)
        {
            Debug.LogWarning("Cannot interact with merchant: Merchant inventory not found!");
            return;
        }

        ui.merchantUI.SetupMerchantUI(merchant, inventory);
        ui.OpenDialogueUI(firstDialogueLine, new DialogueNPCData(rewardNpc, quest));

        //ui.OpenQuestUI(quest);
        //ui.OpenMerchantUI(true);

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