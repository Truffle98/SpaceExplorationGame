using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSetup
{
    public string buildingType;
    public string startRoom, startDoor;
    public int exteriorDoors;
    public Dictionary<string, RoomSetup> roomSetups;

    public void AcceptJSON(BuildingSetupJSON templateJSON, Dictionary<string, RoomSetup> roomSetupsTemp)
    {
        roomSetups = roomSetupsTemp;

        startRoom = templateJSON.startRoom;
        startDoor = templateJSON.startDoor;
        exteriorDoors = templateJSON.exteriorDoors;
    }

}