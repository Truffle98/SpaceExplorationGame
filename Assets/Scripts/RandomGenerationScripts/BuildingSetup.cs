using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSetup
{
    public string startingRoom, startingDoor;
    public int exteriorDoors;
    public Dictionary<string, RoomSetup> roomSetups;

    public BuildingSetup(string startingRoomTemp, string startingDoorTemp, int exteriorDoorsTemp, Dictionary<string, RoomSetup> roomSetupsTemp)
    {
        startingRoom = startingRoomTemp;
        startingDoor = startingDoorTemp;
        exteriorDoors = exteriorDoorsTemp;
        roomSetups = roomSetupsTemp;
    }

}