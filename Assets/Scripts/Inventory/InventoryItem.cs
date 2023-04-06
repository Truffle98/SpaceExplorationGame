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

    public void Set(ItemData itemData, int count)
    {
        this.count = count;
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
        if (itemData.itemName == "Adrenaline Syringe") {
            IncreaseSpeed();
        }
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

    public void UseItem() {
        if (count > 1) {
            DecreaseCount();
            return;
        }
        
        grid.GetComponent<ItemGrid>().PickUpItem(onGridPositionX, onGridPositionY, false, ref grid.GetComponent<ItemGrid>().overlapItemFake);
        grid.GetComponent<ItemGrid>().overlapItemFake = null;
        Destroy(gameObject);
    }

    private void IncreaseSpeed()
    {
        PlayerScript player = GameObject.Find("Player").GetComponent<PlayerScript>();
        player.Sprint(10, player.speed * 2.5f);
        UseItem();
    }
}
