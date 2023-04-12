using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemCardHandler : MonoBehaviour
{
    [SerializeField] RectTransform itemCard;

    public void Show(bool b)
    {
        itemCard.gameObject.SetActive(b);
    }

    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem, int xPos, int yPos)
    {
        Vector2 position = new Vector2();

        if (targetGrid.BoundryCheck(xPos+targetItem.itemData.width, yPos, 3, 4))
        {
            position.x = (xPos + targetItem.itemData.width) * 32;
            position.y = -(yPos * 32);
            itemCard.localPosition = position;
            SetItemCardData(targetItem);
        } else if (targetGrid.BoundryCheck(xPos-3, yPos, 3, 4)) {
            position.x = (xPos - 3) * 32;
            position.y = -(yPos * 32);
            itemCard.localPosition = position;
            SetItemCardData(targetItem);
        } else if (targetGrid.BoundryCheck(xPos-3, targetGrid.height - 4, 3, 4)) {
            position.x = (xPos - 3) * 32;
            position.y = -(targetGrid.height - 4) * 32;
            itemCard.localPosition = position;
            SetItemCardData(targetItem);
        } else if (targetGrid.BoundryCheck(xPos+targetItem.itemData.width, targetGrid.height - 4, 3, 4)) {
            position.x = (xPos + targetItem.itemData.width) * 32;
            position.y = -(targetGrid.height - 4) * 32;
            itemCard.localPosition = position;
            SetItemCardData(targetItem);
        } else if (targetGrid.BoundryCheck(xPos, yPos - 4, 3, 4)) {
            position.x = (xPos) * 32;
            position.y = -(yPos - 4) * 32;
            itemCard.localPosition = position;
            SetItemCardData(targetItem);
        } else if (targetGrid.BoundryCheck(xPos, yPos + targetItem.itemData.height, 3, 4)) {
            position.x = (xPos) * 32;
            position.y = -(yPos + targetItem.itemData.height) * 32;
            itemCard.localPosition = position;
            SetItemCardData(targetItem);
        }
    }

    public void SetParent(ItemGrid targetGrid)
    {
        itemCard.SetParent(targetGrid.GetComponent<RectTransform>());
        itemCard.SetAsLastSibling();
    }

    private void SetItemCardData(InventoryItem targetItem)
    {
        GameObject card = itemCard.gameObject;
        TMP_Text text = card.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TMP_Text>();
        text.text = "Width: " + targetItem.itemData.width;

        text = card.transform.GetChild(0).GetChild(1).gameObject.GetComponent<TMP_Text>();
        text.text = "Height: " + targetItem.itemData.height;

        text = card.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>();
        text.text = targetItem.itemData.itemName;
    }
}
