using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Holds info about a room in a building
public class Room
{

    public BoundsInt bounds;
    public RoomSetup template; 
    public string roomType;

    public Room(BoundsInt boundsTemp, RoomSetup roomTemplate)
    {

        bounds = boundsTemp;
        template = roomTemplate;
        roomType = roomTemplate.roomType;

    }

    public void DrawSelf(Tilemap map)
    {
        //Debug.Log(bounds.center);
        //Debug.Log(bounds.min);
        //Debug.Log(bounds.max);
        map.BoxFill(Vector3Int.FloorToInt(bounds.center), null, bounds.min.x + 1, bounds.min.y + 1, bounds.max.x - 1, bounds.max.y - 1);
    }

}