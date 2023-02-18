using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHandler : MonoBehaviour
{
    private CircleCollider2D playerCollider;
    private GameObject inventory, lootInventory, activeItems, inventoryHandler;
    private InventoryController inventoryController;
    private bool awake, lootAwake, lootable;
    private Loot loot;
    private List<ItemData> itemsNotRemoved = new List<ItemData>();
    void Start()
    {
        playerCollider = gameObject.GetComponent<CircleCollider2D>();
        inventoryHandler = GameObject.Find("Inventory Handler");
        lootInventory = GameObject.Find("Loot Inventory");
        inventoryController = GameObject.Find("Player").GetComponent<InventoryController>();
        inventory = GameObject.Find("Inventory");
        activeItems = GameObject.Find("Excess Actions");
        awake = false;
        inventoryController.activeItems = GameObject.Find("Active Items").GetComponent<ItemGrid>();
        inventoryController.inventoryHandler = inventoryHandler;
        lootInventory.SetActive(lootAwake);
        Show(awake);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!lootInventory.activeSelf)
            {
                if (awake || (!awake && !inventoryController.selectedItem))
                {
                    Show(awake);
                }
            }
        }
        if (lootInventory.activeSelf)
        {
            inventoryController.otherInventory = lootInventory.GetComponent<ItemGrid>();
        }
        if (Input.GetKeyDown(KeyCode.E) && !inventoryController.selectedItem)
        {
            if (lootable)
            {
                if (inventory.activeSelf && !lootInventory.activeSelf)
                {
                    inventory.SetActive(!lootAwake);
                } else {
                    Show(!lootAwake);
                }
                lootInventory.SetActive(!lootAwake);
                lootAwake = !lootAwake;

                if (lootInventory.activeSelf)
                {
                    lootInventory.GetComponent<ItemGrid>().inventoryItems = loot.inventoryItems;

                    foreach (InventoryItem inventoryItem in loot.actualItems)
                    {
                        if (inventoryItem.grid == inventory) { return; }
                        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
                        rectTransform.SetParent(lootInventory.GetComponent<RectTransform>());
                        Vector2 position = new Vector2();
                        position.x = inventoryItem.onGridPositionX * 32 + inventoryItem.itemData.width / 2;
                        position.y = -(inventoryItem.onGridPositionY * 32 + inventoryItem.itemData.height) + 5;
                        rectTransform.localPosition = position;
                    }
                }
            }   
        }
    }    

    public void Show(bool awake)
    {
        inventory.SetActive(awake);
        activeItems.SetActive(awake);
        this.awake = !this.awake;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Lootable")
        {
            lootable = true;
            this.loot = other.gameObject.GetComponent<Loot>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Lootable")
        {
            lootable = false;
            lootInventory.SetActive(false);
            lootAwake = false;
        }
    }
}
