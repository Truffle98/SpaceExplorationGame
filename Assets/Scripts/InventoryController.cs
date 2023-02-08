using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [HideInInspector]
    public ItemGrid inventory, otherInventory;
    public ItemGrid mainInventory;
    private InventoryItem selectedItem, overlapItem, itemToMove;
    private bool placed;
    private RectTransform rectTransform;
    [HideInInspector]
    public InventoryHighlight inventoryHighlight;

    void Update()
    {
        DragItemIcon();

        if (inventory == null) { return; }

        HandleHighlight();

        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseButtonPress();
        } else if (Input.GetMouseButtonDown(1))
        {
            RightMouseButtonPress();
        } else if (Input.GetKeyDown(KeyCode.O)) {
            inventory.Organize();
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
                placed = otherInventory.FindPlaceToPut(itemToMove);
            }
            else if (inventory.gameObject.name == "Loot Inventory")
            {
                itemToMove = otherInventory.PickUpItem(clickPosition.x, clickPosition.y);
                placed = mainInventory.FindPlaceToPut(itemToMove);
            }
            else
            {

            }
        }
    }

    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTiledGridPosition();
        if (selectedItem)
        {
            inventoryHighlight.Show(true);
            inventoryHighlight.SetSize(selectedItem);
            inventoryHighlight.SetParent(inventory);
            inventoryHighlight.SetPosition(inventory, selectedItem, positionOnGrid.x, positionOnGrid.y);
        } else {
            inventoryHighlight.Show(false);
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
            rectTransform.position = new Vector3(Input.mousePosition.x - selectedItem.itemData.width*32/2, Input.mousePosition.y + selectedItem.itemData.height*32/2, Input.mousePosition.z);
        }
    }
}
