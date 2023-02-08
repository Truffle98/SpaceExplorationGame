using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Holds info about a door in a building
public class Door
{

    public Vector2Int location;
    public string doorType;
    public int orientation;
    public float orientationAngle;

    public Door(Vector2Int locationTemp, int orientationTemp, string typeTemp)
    {

        location = locationTemp;
        orientation = orientationTemp;
        orientationAngle = orientation * (Mathf.PI / 2f);
        doorType = typeTemp;

    }

    public void DrawSelf(Tilemap map)
    {
        
        Vector2Int cornerOne = new Vector2Int(0,0), cornerTwo = new Vector2Int(0,0);

        if (doorType == "single")
        {

            cornerOne = new Vector2Int(location.x, location.y);
            cornerTwo = new Vector2Int(location.x, location.y);

        }

        else if (doorType == "wideDouble")
        {
            cornerOne = new Vector2Int(location.x, location.y);
            cornerTwo = new Vector2Int((int)Mathf.Round(location.x + Mathf.Cos(orientationAngle - Mathf.PI * 0.75f) * Mathf.Sqrt(2f)), (int)Mathf.Round(location.y + Mathf.Sin(orientationAngle - Mathf.PI / 0.75f) * Mathf.Sqrt(2f)));
        
        }

        Vector2Int bottomLeft = new Vector2Int(Mathf.Min(cornerOne.x, cornerTwo.x), Mathf.Min(cornerOne.y, cornerTwo.y)), topRight = new Vector2Int(Mathf.Max(cornerOne.x, cornerTwo.x), Mathf.Max(cornerOne.y, cornerTwo.y));

        for (int i = bottomLeft.x; i <= topRight.x; i++)
        {
            for (int j = bottomLeft.y; j <= topRight.y; j++)
            {
                map.SetTile(new Vector3Int(i, j, 0), null);
            }
        }

    }

}