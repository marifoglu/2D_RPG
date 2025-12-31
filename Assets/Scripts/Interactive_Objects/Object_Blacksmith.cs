using UnityEngine;

public class Object_Blacksmith : Object_NPC, IInteractable
{
    private Animator anim;
    private Inventory_Player inventory;
    private Inventory_Storage storage;

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponentInChildren<Animator>();
        if (anim != null)
            anim.SetBool("isBlacksmith", true);
        storage = GetComponent<Inventory_Storage>();
    }

    public override void Interact()
    {
        base.Interact();

        if (ui == null || ui.storageUI == null || inventory == null || storage == null)
        {
            Debug.LogWarning("Cannot open storage: UI or inventory components are not initialized");
            return;
        }

        // Toggle: if open, close it. If closed, open it.
        if (ui.storageUI.gameObject.activeSelf)
        {
            ui.OpenStorageUI(false);
        }
        else
        {
            ui.storageUI.SetupStorageUI(storage);
            ui.craftUI.SetupCraftUI(storage);
            ui.OpenStorageUI(true);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.CompareTag("Player"))
        {
            inventory = collision.GetComponent<Inventory_Player>();
            if (inventory != null)
                storage.SetInventory(inventory);
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
                ui.OpenStorageUI(false);
            }

            inventory = null;
        }
    }
}