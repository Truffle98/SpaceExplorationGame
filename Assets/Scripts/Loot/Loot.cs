using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public LootData loot;
    private ItemGrid lootGrid;
    public InventoryItem[,] inventoryItems;
    public List<InventoryItem> actualItems;

    void Start()
    {
        lootGrid = GameObject.Find("Loot Inventory").GetComponent<ItemGrid>();
        inventoryItems = new InventoryItem[lootGrid.width, lootGrid.height];
        foreach (ItemData item in loot.items)
        {
            InventoryItem inventoryItem = IsStackable(item);
            if (inventoryItem != null) {
                inventoryItem.IncreaseCount(1);
                continue;
            }
            
            inventoryItem = Instantiate(loot.item).GetComponent<InventoryItem>();
            item.width = item.actualWidth;
            item.height = item.actualHeight;
            inventoryItem.Set(item, 1);
            FindPlaceToPut(inventoryItem);
        }
    }

    private bool FindPlaceToPut(InventoryItem itemToMove)
    {
        for (int x = 0; x < lootGrid.width; x++)
        {
            for (int y = 0; y < lootGrid.height; y++)
            {
                if (IsSpace(y, x, itemToMove.itemData.width, itemToMove.itemData.height))
                {
                    PlaceItem(itemToMove, y, x);
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsSpace(int xPos, int yPos, int width, int height)
    {
        if (xPos + width > lootGrid.width-1 || yPos + height > lootGrid.height-1)
        {
            return false;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItems[xPos + x, yPos + y] != null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void PlaceItem(InventoryItem inventoryItem, int xPos, int yPos)
    {
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(gameObject.transform);

        for (int x = 0; x < inventoryItem.itemData.width; x++)
        {
            for (int y = 0; y < inventoryItem.itemData.height; y++)
            {
                inventoryItems[xPos + x, yPos + y] = inventoryItem;
            }
        }

        inventoryItem.onGridPositionX = xPos;
        inventoryItem.onGridPositionY = yPos;
        actualItems.Add(inventoryItem);
    }

    private InventoryItem IsStackable(ItemData itemData) {
        foreach (InventoryItem item in actualItems) {
            if (item.itemData.itemName == itemData.itemName) {
                return item;
            }
        }
        return null;
    }
}
