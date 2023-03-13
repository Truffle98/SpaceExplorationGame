using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Holds info for a Building to generate it's layout
public class BuildingSetup
{
    public string buildingType;
    public string startRoom, startDoor;
    public int exteriorDoors;
    public string[] enemies;
    public int[] enemiesProbabilities;
    public string[] tags;
    public Dictionary<string, RoomSetup> roomSetups;

    public void AcceptJSON(BuildingSetupJSON templateJSON, Dictionary<string, RoomSetup> roomSetupsTemp)
    {
        roomSetups = roomSetupsTemp;

        startRoom = templateJSON.startRoom;
        startDoor = templateJSON.startDoor;
        exteriorDoors = templateJSON.exteriorDoors;

        enemies = new string[templateJSON.enemies.Length];
        enemiesProbabilities = new int[templateJSON.enemiesProbabilities.Length];

        for (int i = 0; i < templateJSON.enemies.Length; i++)
        {
            enemies[i] = templateJSON.enemies[i];
            enemiesProbabilities[i] = templateJSON.enemiesProbabilities[i];
        }

    }

    public void AcceptTags(string[] tagsTemp)
    {
        tags = new string[tagsTemp.Length];
        for (int i = 0; i < tagsTemp.Length; i++)
        {
            tags[i] = tagsTemp[i];
        }
    }

}