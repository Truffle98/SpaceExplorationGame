using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TemplateReader
{

    public Dictionary<string, RoomSetup> ReadBuildingTemplate(string templateName)
    {
        var json = Resources.Load<TextAsset>("RoomAssets/RoomTemplateDescriptions/" + templateName);
        RoomTemplateList buildingTemplate = JsonUtility.FromJson<RoomTemplateList>(json.text);

        Dictionary<string, RoomSetup> roomSetups = new Dictionary<string, RoomSetup>();
        RoomSetup newSetup;
        TemplateJSON currentTemplate;

        for (int i = 0; i < buildingTemplate.roomSetups.Length; i++)
        {
            currentTemplate = buildingTemplate.roomSetups[i];
            newSetup = new RoomSetup();
            newSetup.AcceptJSON(currentTemplate);
            roomSetups.Add(currentTemplate.roomType, newSetup);
        }
        return roomSetups;
    }
    
}

[System.Serializable]
public class RoomTemplateList
{
    public TemplateJSON[] roomSetups;
}

[System.Serializable]
public class TemplateJSON
{

    public string roomType, doorType, floorType;
    public int[] roomMin, roomMax;
    public string[] roomsToMake, smallRoomsToMake;
    public int[] roomsToMakeProbabilities, smallRoomsToMakeProbabilities;
    public string[] centerObjects, wallObjects, cornerObjects;
    public bool canAddDoors;

}