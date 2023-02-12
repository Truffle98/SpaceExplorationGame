using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LootData : ScriptableObject
{
    public ItemData[] items;
    public GameObject item;
    [HideInInspector]
    public ItemGrid lootGrid;

    public void PopulateLootInventory()
    {
        foreach (ItemData item in items)
        {
            InventoryItem inventoryItem = Instantiate(this.item).GetComponent<InventoryItem>();
            inventoryItem.Set(item);
            lootGrid.FindPlaceToPut(inventoryItem);
        }
    }
}
