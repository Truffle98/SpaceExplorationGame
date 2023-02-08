using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CityBlock
{

    private BoundsInt bounds;
    private TileBase tile;
    private string blockType;
    private List<Building> buildings = new List<Building>();

    public CityBlock(BoundsInt boundsTemp, TileBase tileTemp, string typeTemp)
    {
        bounds = boundsTemp;
        tile = tileTemp;
        blockType = typeTemp;
        SelfSetup();
    }

    public void DrawSelf(Tilemap map)
    {

        if (blockType == "block")
        {

        }
        else if (blockType == "empty")
        {
            
        }

        foreach (Building building in buildings)
        {
            building.DrawSelf(map);
        }

    }

    private void SelfSetup()
    {

        if (blockType == "block")
        {
            buildings.Add(new Building(new BoundsInt(new Vector3Int(bounds.min.x, bounds.min.y, -1), new Vector3Int(bounds.size.x, bounds.size.y, 2)), tile, "block"));
        }

    }

}
