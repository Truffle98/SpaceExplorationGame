using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TemplateReader
{

    public BuildingSetup ReadBuildingTemplate(string templateName)
    {
        var json = Resources.Load<TextAsset>("RoomAssets/RoomTemplateDescriptions/" + templateName);
        BuildingTemplateJSON buildingTemplate = JsonUtility.FromJson<BuildingTemplateJSON>(json.text);

        Dictionary<string, RoomSetup> roomSetups = new Dictionary<string, RoomSetup>();
        RoomSetup newSetup;
        RoomTemplateJSON currentTemplate;

        for (int i = 0; i < buildingTemplate.roomSetups.Length; i++)
        {
            currentTemplate = buildingTemplate.roomSetups[i];
            newSetup = new RoomSetup();
            newSetup.AcceptJSON(currentTemplate);
            roomSetups.Add(currentTemplate.roomType, newSetup);
        }
        
        BuildingSetup buildingSetup = new BuildingSetup(buildingTemplate.startRoom, buildingTemplate.startDoor, buildingTemplate.exteriorDoors, roomSetups);
        return buildingSetup;
    }
    
}

[System.Serializable]
public class BuildingTemplateJSON
{
    public int exteriorDoors;
    public string startRoom, startDoor;
    public RoomTemplateJSON[] roomSetups;
}

[System.Serializable]
public class RoomTemplateJSON
{
    public string roomType, doorType, floorType;
    public int[] roomMin, roomMax, roomObjectsProbabilities;
    public string[] roomsToMake, smallRoomsToMake;
    public int[] roomsToMakeProbabilities, smallRoomsToMakeProbabilities;
    public string[] roomObjects;
    public int[] roomObjectsCount;
    public bool canAddDoors;
}