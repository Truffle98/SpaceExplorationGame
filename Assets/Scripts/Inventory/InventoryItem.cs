using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;
    [HideInInspector]
    public int onGridPositionX, onGridPositionY;
    public GameObject grid;
    public int count = 0;
    public TMP_Text countText;

    public void Set(ItemData itemData)
    {
        if (count == 0) {
            this.count = 1;
        }
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

    public void ExecuteAction() {
        itemData.ExecuteAction();
    }

    public void IncreaseCount(int increment)
    {
        TMP_Text text = null;
        if (this.count == 1) {
            text = Instantiate(this.countText, gameObject.transform);
        } else {
            text = gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        }
        this.count += increment;
        text.text = this.count.ToString();
    }

    public void DecreaseCount()
    {
        this.count--;
        if (count == 1) {
            Destroy(gameObject.transform.GetChild(0).gameObject);
            return;
        } else if (count > 1) {
            TMP_Text text = gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
            text.text = this.count.ToString();
        }
    }
}
