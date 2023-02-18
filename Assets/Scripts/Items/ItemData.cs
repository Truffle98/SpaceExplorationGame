using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public int width = 1;
    public int height = 1;
    public int actualWidth = 1;
    public int actualHeight = 1;
    public string itemName;
    public Sprite itemIcon;
}
