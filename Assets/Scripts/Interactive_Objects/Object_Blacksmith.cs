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
        anim.SetBool("isBlacksmith", true);
        storage = GetComponent<Inventory_Storage>();
    }

    public void Interact()
    {
        if (ui == null || ui.storageUI == null || inventory == null || storage == null)
        {
            Debug.LogWarning("Cannot open storage: UI or inventory components are not initialized");
            return;
        }

        ui.storageUI.SetupStorage(inventory, storage);
        ui.storageUI.gameObject.SetActive(true);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        inventory = player.GetComponent<Inventory_Player>();
        storage.SetInventory(inventory);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        if (ui != null)
        {
            ui.SwitchOffAllToolTips();

            if (ui.storageUI != null)
                ui.storageUI.gameObject.SetActive(false);
        }
    }
}
