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
}
