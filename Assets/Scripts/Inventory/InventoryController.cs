using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public TMP_Text helperText;

    void Start()
    {
        helperText.CrossFadeAlpha(0.0f, 0.0f, false);
    }

    void Update()
    {
        DragItemIcon();
        CheckActiveItemInputs();

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
        }
        else if (Input.GetMouseButtonDown(1))
        {
            RightMouseButtonPress();
        }
    }

    private void CheckActiveItemInputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (activeItems.inventoryItems[0, 0] != null)
            {
                activeItems.inventoryItems[0, 0].ExecuteAction();
            }
            else
            {
                helperText.text = "No item in active item slot 1";
                helperText.CrossFadeAlpha(1f, 0f, false);
                helperText.CrossFadeAlpha(0.0f, 1f, false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (activeItems.inventoryItems[0, 1] != null)
            {
                activeItems.inventoryItems[0, 1].ExecuteAction();
            }
            else
            {
                helperText.text = "No item in active item slot 2";
                helperText.CrossFadeAlpha(1f, 0f, false);
                helperText.CrossFadeAlpha(0.0f, 1f, false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (activeItems.inventoryItems[0, 2] != null)
            {
                activeItems.inventoryItems[0, 2].ExecuteAction();
            }
            else
            {
                helperText.text = "No item in active item slot 3";
                helperText.CrossFadeAlpha(1f, 0f, false);
                helperText.CrossFadeAlpha(0.0f, 1f, false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (activeItems.inventoryItems[0, 3] != null)
            {
                activeItems.inventoryItems[0, 3].ExecuteAction();
            }
            else
            {
                helperText.text = "No item in active item slot 4";
                helperText.CrossFadeAlpha(1f, 0f, false);
                helperText.CrossFadeAlpha(0.0f, 1f, false);
            }
        }
    }

    private void RightMouseButtonPress()
    {
        Vector2Int clickPosition = GetTiledGridPosition();
        if (inventory.ItemAt(clickPosition.x, clickPosition.y) != null)
        {
            if (inventory.gameObject.name == "Inventory")
            {
                itemToMove = mainInventory.PickUpItem(clickPosition.x, clickPosition.y, true);
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
                itemToMove = otherInventory.PickUpItem(clickPosition.x, clickPosition.y, true);
                placed = mainInventory.FindPlaceToPut(itemToMove);
                if (!placed)
                {
                    placed = otherInventory.FindPlaceToPut(itemToMove);
                }
            }
            else if (inventory.gameObject.name == "Active Items")
            {
                itemToMove = activeItems.PickUpItem(clickPosition.x, clickPosition.y, true);

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
        if (Input.GetKey(KeyCode.LeftShift)) {
            selectedItem = inventory.PickUpItem(position.x, position.y, true);
        } else {
            selectedItem = inventory.PickUpItem(position.x, position.y, false);
        }
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
