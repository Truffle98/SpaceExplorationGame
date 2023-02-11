using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Holds info about a room to be made
//Has the max and minimum size, the name of the room, the name of the door to be loaded
//Should have some names of assets for things to be loaded such as flooring and objects in the room, as well as descriptions on how to load the objects
//Should also have information about the rooms to come afterwards, the first rooms and the small ones to follow
//Has info if additional doors can be attached
public class RoomSetup
{

    public Vector2Int minSize, maxSize;
    public string roomType, doorType, floorType;
    public string[] roomsToMake, smallRoomsToMake;
    public int[] roomsToMakeProbabilities, smallRoomsToMakeProbabilities;
    public string[] roomObjects;
    public int[] roomObjectsProbabilities, roomObjectsCount;
    public bool canAddDoors;

    public RoomSetup()
    {



    }

    public void AcceptJSON(TemplateJSON templateJSON)
    {

        roomType = templateJSON.roomType;
        doorType = templateJSON.doorType;
        floorType = templateJSON.floorType;
        minSize = new Vector2Int(templateJSON.roomMin[0], templateJSON.roomMin[1]);
        maxSize = new Vector2Int(templateJSON.roomMax[0], templateJSON.roomMax[1]);
        
        roomsToMake = new string[templateJSON.roomsToMake.Length];
        roomsToMakeProbabilities = new int[templateJSON.roomsToMake.Length];
        for (int i = 0; i < templateJSON.roomsToMake.Length; i++)
        {
            roomsToMake[i] = templateJSON.roomsToMake[i];
            roomsToMakeProbabilities[i] = templateJSON.roomsToMakeProbabilities[i];
        }

        smallRoomsToMake = new string[templateJSON.smallRoomsToMake.Length];
        smallRoomsToMakeProbabilities = new int[templateJSON.smallRoomsToMake.Length];
        for (int i = 0; i < templateJSON.smallRoomsToMake.Length; i++)
        {
            smallRoomsToMake[i] = templateJSON.smallRoomsToMake[i];
            smallRoomsToMakeProbabilities[i] = templateJSON.smallRoomsToMakeProbabilities[i];
        }
        roomObjects = new string[templateJSON.roomObjects.Length];
        roomObjectsProbabilities = new int[templateJSON.roomObjects.Length];
        roomObjectsCount = new int[templateJSON.roomObjects.Length];
        
        for (int i = 0; i < templateJSON.roomObjects.Length; i++)
        {
            roomObjects[i] = templateJSON.roomObjects[i];
            roomObjectsProbabilities[i] = templateJSON.roomObjectsProbabilities[i];
            roomObjectsCount[i] = templateJSON.roomObjectsCount[i];
        }

        canAddDoors = templateJSON.canAddDoors;
    }

    public Queue<RoomSetup> GenerateRoomSetupQueue(Dictionary<string, RoomSetup> roomSetups)
    {

        Queue<RoomSetup> roomQueue = new Queue<RoomSetup>();

        for(int i = 0; i < roomsToMake.Length; i++)
        {
            if (Random.Range(0, 100) < roomsToMakeProbabilities[i])
            {
                roomQueue.Enqueue(roomSetups[roomsToMake[i]]);
            }
        }

        return roomQueue;

    }

    public Queue<RoomSetup> GenerateSmallRoomSetupQueue(Dictionary<string, RoomSetup> roomSetups)
    {

        Queue<RoomSetup> roomQueue = new Queue<RoomSetup>();
        
        for(int i = 0; i < smallRoomsToMake.Length; i++)
        {
            if (Random.Range(0, 100) < smallRoomsToMakeProbabilities[i])
            {
                roomQueue.Enqueue(roomSetups[smallRoomsToMake[i]]);
            }
        }

        return roomQueue;

    }

}