using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour
{
    public ItemData[] items;
    public GameObject item;
    // [HideInInspector]
    public GameObject lootInventory;
    private InventoryItem overlapItem;
    private InventoryHandler inventoryHandler;
    private ItemGrid lootGrid;
    private bool awake;

    void Start()
    {
        lootInventory = GameObject.Find("Loot Inventory");
        inventoryHandler = GameObject.Find("Inventory Handler").GetComponent<InventoryHandler>();
        lootGrid = lootInventory.GetComponent<ItemGrid>();

        InventoryItem inventoryItem = Instantiate(item).GetComponent<InventoryItem>();
        inventoryItem.Set(items[0]);
        lootGrid.PlaceItem(inventoryItem, 1, 1, 0, 0, ref overlapItem);

        inventoryItem = Instantiate(item).GetComponent<InventoryItem>();
        inventoryItem.Set(items[1]);
        lootGrid.PlaceItem(inventoryItem, 2, 1, 0, 0, ref overlapItem);

        inventoryItem = Instantiate(item).GetComponent<InventoryItem>();
        inventoryItem.Set(items[2]);
        lootGrid.PlaceItem(inventoryItem, 1, 4, 0, 0, ref overlapItem);

        lootInventory.SetActive(awake);

        // In the future I want to create a 2d array (similar to the one in ItemGrid) filled with all the items stored in the chest,
        // then when I walk close enough to the chest this list will replace the loot inventory's 2d array. Additionally, I want to put the
        // opening functionality on the InventoryHandler script
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryHandler.Show(!awake);
            lootInventory.SetActive(!awake);
            awake = !awake;
        }
    }
}
