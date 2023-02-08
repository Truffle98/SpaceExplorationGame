using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHighlight : MonoBehaviour
{
    [SerializeField] RectTransform highlighter;

    public void Show(bool b)
    {
        highlighter.gameObject.SetActive(b);
    }

    public void SetSize(InventoryItem targetItem)
    {
        Vector2 size = new Vector2();
        size.x = targetItem.itemData.width * ItemGrid.tileSizeWidth;
        size.y = targetItem.itemData.height * ItemGrid.tileSizeHeight;
        highlighter.sizeDelta = size;
    }

    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem, int xPos, int yPos)
    {
        Vector2 position = new Vector2();
        position.x = xPos * 32 + targetItem.itemData.width*16;
        position.y = -(yPos * 32 + targetItem.itemData.height*16);

        if (targetGrid.BoundryCheck(xPos, yPos, targetItem.itemData.width, targetItem.itemData.height))
        {
            highlighter.localPosition = position;
        }
    }

    public void SetParent(ItemGrid targetGrid)
    {
        highlighter.SetParent(targetGrid.GetComponent<RectTransform>());
        highlighter.SetAsFirstSibling();
    }
}
