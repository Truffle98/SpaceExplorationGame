using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHandler : MonoBehaviour
{
    private CircleCollider2D playerCollider;
    private GameObject inventory, lootInventory;
    private InventoryController inventoryController;
    private bool awake, lootAwake, lootable;
    private LootData loot;
    void Start()
    {
        playerCollider = gameObject.GetComponent<CircleCollider2D>();
        lootInventory = GameObject.Find("Loot Inventory");
        inventoryController = GameObject.Find("Player").GetComponent<InventoryController>();
        inventory = GameObject.Find("Inventory").gameObject;
        awake = false;
        lootInventory.SetActive(lootAwake);
        Show(awake);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!lootInventory.activeSelf)
            {
                Show(awake);
            }
        }
        if (lootInventory.activeSelf)
        {
            inventoryController.otherInventory = lootInventory.GetComponent<ItemGrid>();
        }
        if (Input.GetKeyDown(KeyCode.E))
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
                    loot.PopulateLootInventory();
                }
            }   
        }
    }    

    public void Show(bool awake)
    {
        inventory.SetActive(awake);
        this.awake = !this.awake;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Lootable")
        {
            lootable = true;
            this.loot = other.gameObject.GetComponent<Loot>().loot;
        }
    }
}
