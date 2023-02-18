using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;
    [HideInInspector]
    public int onGridPositionX, onGridPositionY;
    public GameObject grid;

    public void Set(ItemData itemData)
    {
        this.itemData = itemData;

        GetComponent<Image>().sprite = itemData.itemIcon;

        Vector2 size = new Vector2();
        size.x = itemData.width * ItemGrid.tileSizeWidth;
        size.y = itemData.height * ItemGrid.tileSizeHeight;
        GetComponent<RectTransform>().sizeDelta = size;
    }
    
    public void ResizeToActive()
    {
        GetComponent<Image>().sprite = itemData.itemIcon;

        Vector2 size = new Vector2();
        size.x = ItemGrid.tileSizeWidth;
        size.y = ItemGrid.tileSizeHeight;
        GetComponent<RectTransform>().sizeDelta = size;
    }
}
