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
    private GameObject blockParent;

    public CityBlock(BoundsInt boundsTemp, TileBase tileTemp, string typeTemp, GameObject parentTemp)
    {
        bounds = boundsTemp;
        tile = tileTemp;
        blockType = typeTemp;
        blockParent = parentTemp;
        SelfSetup();
    }

    public void DrawSelf(Tilemap frontMap, Tilemap backMap)
    {

        if (blockType == "block")
        {

        }
        else if (blockType == "empty")
        {
            
        }

        foreach (Building building in buildings)
        {
            building.DrawSelf(frontMap, backMap);
        }

    }

    private void SelfSetup()
    {

        if (blockType == "block")
        {
            var buildingParent = new GameObject("Building Parent 0");
            buildingParent.transform.parent = blockParent.transform;
            buildings.Add(new Building(new BoundsInt(new Vector3Int(bounds.min.x, bounds.min.y, -1), new Vector3Int(bounds.size.x, bounds.size.y, 2)), tile, "testBuilding", buildingParent));
        }

    }

}
