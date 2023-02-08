using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Holds info about a room to be made
public class RoomSetup
{

    public Vector2Int minSize, maxSize;
    public string roomType, doorType;
    public bool canAddDoors;

    public RoomSetup(Vector2Int minSizeTemp, Vector2Int maxSizeTemp, string typeTemp, bool canAddDoorsTemp = true)
    {

        minSize = minSizeTemp;
        maxSize = maxSizeTemp;
        roomType = typeTemp;
        canAddDoors = canAddDoorsTemp;

        if (roomType == "startRoom")
        {
            doorType = "single";
        }
        else if (roomType == "testRoom")
        {
            doorType = "single";
        }

    }

    public Queue<RoomSetup> GenerateRoomSetupQueue()
    {

        Queue<RoomSetup> roomQueue = new Queue<RoomSetup>();
        RoomSetup testRoom = new RoomSetup(new Vector2Int(5, 5), new Vector2Int(10, 10), "testRoom");

        if (roomType == "startRoom")
        {
            roomQueue.Enqueue(testRoom);
            roomQueue.Enqueue(testRoom);
        }
        else if (roomType == "testRoom")
        {
            roomQueue.Enqueue(testRoom);
            roomQueue.Enqueue(testRoom);
            roomQueue.Enqueue(testRoom);
        }

        return roomQueue;

    }

    public Queue<RoomSetup> GenerateSmallRoomSetupQueue()
    {

        Queue<RoomSetup> roomQueue = new Queue<RoomSetup>();
        RoomSetup smallTestRoom = new RoomSetup(new Vector2Int(4, 4), new Vector2Int(6, 6), "smallTestRoom");

        if (roomType == "testRoom")
        {
            roomQueue.Enqueue(smallTestRoom);
        }

        return roomQueue;

    }

}