using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Holds info about rooms that should branch from the current
public class BranchingPoint
{

    public BoundsInt bounds;
    public Queue<RoomSetup> roomsToMake;
    public string doorType;

    public BranchingPoint(BoundsInt boundsTemp, Queue<RoomSetup> roomsToMakeTemp, string doorTypeTemp)
    {

        bounds = boundsTemp;
        roomsToMake = roomsToMakeTemp;
        doorType = doorTypeTemp;

    }

}