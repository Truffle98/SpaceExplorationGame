using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    [HideInInspector]
    public const float tileSizeWidth = 32;
    [HideInInspector]
    public const float tileSizeHeight = 32;
    private RectTransform rectTransform;
    private Vector2 positionOnGrid = new Vector2();
    private Vector2Int tilePosition = new Vector2Int();
    private InventoryItem[,] inventoryItems;
    public int width = 15, height = 10;
    private InventoryItem overlapItemFake;
    private List<InventoryItem> actualItems;

    void Start()
    {
        actualItems = new List<InventoryItem>();
        rectTransform = GetComponent<RectTransform>();
        InitInventory(width,height);
    }

    public void InitInventory(int width, int height)
    {
        inventoryItems = new InventoryItem[width, height];
        Vector2 size = new Vector2(width*tileSizeWidth, height*tileSizeHeight);
        rectTransform.sizeDelta = size;
    }

    public Vector2Int GetTilePosition(Vector2 mousePosition)
    {
        positionOnGrid.x = mousePosition.x - rectTransform.position.x;
        positionOnGrid.y = rectTransform.position.y - mousePosition.y;

        tilePosition.x = (int)(positionOnGrid.x / tileSizeWidth);
        tilePosition.y = (int)(positionOnGrid.y / tileSizeHeight);

        return tilePosition;
    }

    public bool PlaceItem(InventoryItem inventoryItem, int xPos, int yPos, int clickX, int clickY, ref InventoryItem overlapItem)
    {
        if (!BoundryCheck(xPos, yPos, inventoryItem.itemData.width, inventoryItem.itemData.height)) { return false; }
        if (!IntersectionCheck(xPos, yPos, clickX, clickY, inventoryItem.itemData.width, inventoryItem.itemData.height, ref overlapItem))
        {
            overlapItem = null;
            return false;
        }

        if (overlapItem) { ClearItem(overlapItem); }

        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);

        for (int x = 0; x < inventoryItem.itemData.width; x++)
        {
            for (int y = 0; y < inventoryItem.itemData.height; y++)
            {
                inventoryItems[xPos + x, yPos + y] = inventoryItem;
            }
        }

        inventoryItem.onGridPositionX = xPos;
        inventoryItem.onGridPositionY = yPos;

        Vector2 position = new Vector2();
        position.x = xPos * tileSizeWidth + inventoryItem.itemData.width / 2;
        position.y = -(yPos * tileSizeHeight + inventoryItem.itemData.height) + 5;

        rectTransform.localPosition = position;
        if (!actualItems.Contains(inventoryItem))
        {
            actualItems.Add(inventoryItem);
        }
        return true;
    }

    public InventoryItem PickUpItem(int xPos, int yPos)
    {
        InventoryItem item = inventoryItems[xPos, yPos];

        if (!item) { return null; }

        ClearItem(item);
        actualItems.Remove(item);

        return item;
    }

    private void ClearItem(InventoryItem item)
    {
        for (int x = 0; x < item.itemData.width; x++)
        {
            for (int y = 0; y < item.itemData.height; y++)
            {
                inventoryItems[item.onGridPositionX + x, item.onGridPositionY + y] = null;
            }
        }
    }

    private bool PositionCheck(int xPos, int yPos)
    {
        if (xPos < 0 || yPos < 0)
        {
            return false;
        }

        if (xPos >= width || yPos >= height)
        {
            return false;
        }

        return true;
    }

    public bool BoundryCheck(int xPos, int yPos, int width, int height)
    {
        if (!PositionCheck(xPos, yPos)) { return false; }

        xPos += width-1;
        yPos += height-1;

        if (!PositionCheck(xPos, yPos)) { return false; }

        return true;
    }

    private bool IntersectionCheck(int xPos, int yPos, int clickX, int clickY, int width, int height, ref InventoryItem overlapItem)
    {
        if (inventoryItems[clickX, clickY] != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (inventoryItems[xPos + x, yPos + y] != null && inventoryItems[xPos + x, yPos + y] != inventoryItems[clickX, clickY])
                    {
                        return false;
                    }
                }
            }
            if (!overlapItem)
            {
                overlapItem = inventoryItems[clickX, clickY];
            }
            
        } else {
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
        }
        
        return true;
    }

    public bool FindPlaceToPut(InventoryItem itemToMove)
    {
        for (int x = 0; x < this.width; x++)
        {
            for (int y = 0; y < this.height; y++)
            {
                if (IsSpace(y, x, itemToMove.itemData.width, itemToMove.itemData.height))
                {
                    PlaceItem(itemToMove, y, x, y, x, ref overlapItemFake);
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsSpace(int xPos, int yPos, int width, int height)
    {
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

    public void Organize()
    {
        int area;
        int k;
        for (int i = 1; i < actualItems.Count; i++)
        {
            k = i;
            area = actualItems[k].itemData.width * actualItems[k].itemData.height;
            while (k > 0 && area >= actualItems[k-1].itemData.width * actualItems[k-1].itemData.height)
            {
                InventoryItem temp = actualItems[k-1];
                actualItems[k-1] = actualItems[k];
                actualItems[k] = temp;
                area = actualItems[k-1].itemData.width * actualItems[k-1].itemData.height;
                k--;
            }
        }

        inventoryItems = new InventoryItem[width, height];
        foreach (InventoryItem item in actualItems)
        {
            FindPlaceToPut(item);
        }
    }

    public object ItemAt(int x, int y)
    {
        return inventoryItems[x, y];
    }
}
