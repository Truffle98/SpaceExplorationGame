using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemGrid))]
public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InventoryController inventoryController;
    private ItemGrid inventory;
    private InventoryHighlight highlighter;
    private ItemCardHandler itemCard;

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.inventory = inventory;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.inventory = null;
    }

    void Start()
    {
        inventoryController = GameObject.Find("Player").GetComponent<InventoryController>();
        inventory = GetComponent<ItemGrid>();
        highlighter = GetComponent<InventoryHighlight>();
        itemCard = GetComponent<ItemCardHandler>();
        inventoryController.inventoryHighlight = highlighter;
        inventoryController.itemCard = itemCard;
    }
}
