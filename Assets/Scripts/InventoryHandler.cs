using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHandler : MonoBehaviour
{
    private GameObject inventory, lootInventory;
    private InventoryController inventoryController;
    private bool awake;
    void Start()
    {
        lootInventory = GameObject.Find("Loot Inventory");
        inventoryController = GameObject.Find("Player").GetComponent<InventoryController>();
        inventory = gameObject.transform.GetChild(0).gameObject;
        awake = false;
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
    }

    public void Show(bool awake)
    {
        inventory.SetActive(awake);
        this.awake = !this.awake;
    }
}
