using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [HideInInspector]
    public ItemGrid inventory, otherInventory, activeItems;
    public ItemGrid mainInventory;
    [HideInInspector]
    public InventoryItem selectedItem, overlapItem, itemToMove;
    private bool placed;
    private RectTransform rectTransform;
    [HideInInspector]
    public InventoryHighlight inventoryHighlight;
    public ItemCardHandler itemCard;
    public GameObject inventoryHandler;

    void Update()
    {
        DragItemIcon();

        if (inventory == null)
        {
            itemCard.Show(false);
            inventoryHighlight.Show(false);
            return;
        }

        HandleHighlight();

        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseButtonPress();
        } else if (Input.GetMouseButtonDown(1))
        {
            RightMouseButtonPress();
        }
    }

    private void RightMouseButtonPress()
    {
        Vector2Int clickPosition = GetTiledGridPosition();
        if (inventory.ItemAt(clickPosition.x, clickPosition.y) != null)
        {
            if (inventory.gameObject.name == "Inventory")
            {
                itemToMove = mainInventory.PickUpItem(clickPosition.x, clickPosition.y);

                itemToMove.itemData.width = 1;
                itemToMove.itemData.height = 1;
                itemToMove.Set(itemToMove.itemData);

                placed = activeItems.FindPlaceToPutActiveItems(itemToMove);
                if (!placed)
                {
                    placed = mainInventory.FindPlaceToPut(itemToMove);
                }
            }
            else if (inventory.gameObject.name == "Loot Inventory")
            {
                itemToMove = otherInventory.PickUpItem(clickPosition.x, clickPosition.y);
                placed = mainInventory.FindPlaceToPut(itemToMove);
                if (!placed)
                {
                    placed = otherInventory.FindPlaceToPut(itemToMove);
                }
            }
            else if (inventory.gameObject.name == "Active Items")
            {
                itemToMove = activeItems.PickUpItem(clickPosition.x, clickPosition.y);

                itemToMove.itemData.width = itemToMove.itemData.actualWidth;
                itemToMove.itemData.height = itemToMove.itemData.actualHeight;
                itemToMove.Set(itemToMove.itemData);

                placed = mainInventory.FindPlaceToPut(itemToMove);
                if (!placed)
                {
                    placed = activeItems.FindPlaceToPut(itemToMove);
                }
            }
        }
    }

    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTiledGridPosition();
        if (selectedItem)
        {
            selectedItem.transform.SetParent(inventoryHandler.transform);
            selectedItem.transform.SetAsLastSibling();
            itemCard.Show(false);
            inventoryHighlight.Show(true);
            inventoryHighlight.SetSize(selectedItem);
            inventoryHighlight.SetParent(inventory);
            inventoryHighlight.SetPosition(inventory, selectedItem, positionOnGrid.x, positionOnGrid.y);
        } else {
            if (positionOnGrid.x > inventory.width-1) { positionOnGrid.x = inventory.width-1; }
            if (positionOnGrid.x < 0) { positionOnGrid.x = 0 ;}
            if (positionOnGrid.y > inventory.height-1) { positionOnGrid.y = inventory.height-1; }
            if (positionOnGrid.y < 0) { positionOnGrid.y = 0; }
            InventoryItem item = inventory.ItemAt(positionOnGrid.x, positionOnGrid.y);
            if (!item)
            {
                inventoryHighlight.Show(false);
                itemCard.Show(false);
                return;
            }
            inventoryHighlight.Show(true);
            inventoryHighlight.SetSize(item);
            inventoryHighlight.SetParent(inventory);
            inventoryHighlight.SetPosition(inventory, item, item.onGridPositionX, item.onGridPositionY);

            itemCard.Show(true);
            itemCard.SetParent(inventory);
            itemCard.SetPosition(inventory, item, item.onGridPositionX, item.onGridPositionY);
        }
    }

    private void LeftMouseButtonPress()
    {
        Vector2Int clickPosition = inventory.GetTilePosition(Input.mousePosition);
        Vector2Int placePosition = GetTiledGridPosition();
        if (selectedItem == null)
        {
            PickUpItem(placePosition);
        }
        else
        {
            PlaceItem(placePosition, clickPosition);
        }
    }

    private Vector2Int GetTiledGridPosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        if (selectedItem)
        {
            mousePosition.x -= (selectedItem.itemData.width - 1) * ItemGrid.tileSizeWidth / 2;
            mousePosition.y += (selectedItem.itemData.height - 1) * ItemGrid.tileSizeHeight / 2;
        }

        Vector2Int position = inventory.GetTilePosition(mousePosition);
        return position;
    }

    private void PlaceItem(Vector2Int placePosition, Vector2Int clickPosition)
    {
        if (inventory.PlaceItem(selectedItem, placePosition.x, placePosition.y, clickPosition.x, clickPosition.y, ref overlapItem))
        {
            selectedItem = null;
            if (overlapItem != null)
            {
                selectedItem = overlapItem;
                overlapItem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
            }
        }
    }

    private void PickUpItem(Vector2Int position)
    {
        selectedItem = inventory.PickUpItem(position.x, position.y);
        if (selectedItem)
        {
            rectTransform = selectedItem.GetComponent<RectTransform>();
        }
    }

    private void DragItemIcon()
    {
        if (selectedItem)
        {
            if (inventory && inventory.gameObject.name == "Active Items")
            {
                selectedItem.itemData.width = 1;
                selectedItem.itemData.height = 1;
                selectedItem.Set(selectedItem.itemData);
                
            } else {
                selectedItem.itemData.width = selectedItem.itemData.actualWidth;
                selectedItem.itemData.height = selectedItem.itemData.actualHeight;
                selectedItem.Set(selectedItem.itemData);
            }
            rectTransform.position = new Vector3(Input.mousePosition.x - selectedItem.itemData.width*32/2, Input.mousePosition.y + selectedItem.itemData.height*32/2, Input.mousePosition.z);
        }
    }
}
