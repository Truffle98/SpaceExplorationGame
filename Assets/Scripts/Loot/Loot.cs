using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public LootData loot;
    private ItemGrid lootGrid;

    void Start()
    {
        lootGrid = GameObject.Find("Loot Inventory").GetComponent<ItemGrid>();
        loot.lootGrid = lootGrid;
    }
}
