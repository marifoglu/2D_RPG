//using UnityEngine;

//public class Object_Merchant : Object_NPC, IInteractable
//{
//    private Inventory_Player inventory;
//    private Inventory_Merchant merchant;

//    protected override void Awake()
//    {
//        base.Awake();
//        merchant = GetComponent<Inventory_Merchant>();
//    }
//    protected override void Update()
//    {
//        base.Update();
//        if (Input.GetKeyDown(KeyCode.Z))
//        {
//            merchant.FillShopList();
//        }
//    }
//    public void Interact()
//    {
//        if (inventory == null)
//        {
//            Debug.LogWarning("Cannot interact with merchant: Player inventory not found.");
//            return;
//        }

//        if (ui != null && ui.merchantUI != null)
//        {
//            ui.merchantUI.SetupMerchantUI(merchant, inventory);
//            ui.OpenMerchantUI(true);
//        }
//    }

//    protected override void OnTriggerEnter2D(Collider2D collision)
//    {
//        base.OnTriggerEnter2D(collision);
//        inventory = player.GetComponent<Inventory_Player>();
//        merchant.SetInventory(inventory);
//    }

//    protected override void OnTriggerExit2D(Collider2D collision)
//    {
//        base.OnTriggerExit2D(collision);

//        if (ui != null)
//        {
//            ui.HideAllToolTips();
//            ui.OpenMerchantUI(false);
//        }
//    }
//}
//using UnityEngine;

using UnityEngine;

public class Object_Merchant : Object_NPC, IInteractable
{
    private Inventory_Player inventory;
    private Inventory_Merchant merchant;

    protected override void Awake()
    {
        base.Awake();
        merchant = GetComponent<Inventory_Merchant>();
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            merchant.FillShopList();
        }
    }

    public void Interact()
    {
        if (inventory == null)
        {
            Debug.LogWarning("Cannot interact with merchant: Player inventory not found.");
            return;
        }

        if (ui == null || ui.merchantUI == null)
        {
            Debug.LogWarning("Cannot interact with merchant: UI or MerchantUI not found.");
            return;
        }

        // Toggle: if open, close it. If closed, open it.
        if (ui.merchantUI.gameObject.activeSelf)
        {
            ui.OpenMerchantUI(false);
        }
        else
        {
            ui.merchantUI.SetupMerchantUI(merchant, inventory);
            ui.OpenMerchantUI(true);
        }
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
                ui.HideAllToolTips();
                ui.OpenMerchantUI(false);
            }

            inventory = null;
        }
    }
}