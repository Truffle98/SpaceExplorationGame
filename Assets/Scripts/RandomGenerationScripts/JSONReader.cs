using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TemplateReader
{
    public BuildingSetup ReadBuildingTemplate(string templateName)
    {
        var json = Resources.Load<TextAsset>("BuildingAssets/BuildingTemplates/" + templateName);
        BuildingSetupJSON buildingTemplate = JsonUtility.FromJson<BuildingSetupJSON>(json.text);

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
        
        BuildingSetup buildingSetup = new BuildingSetup();
        buildingSetup.AcceptJSON(buildingTemplate, roomSetups);
        return buildingSetup;
    }

    public CityBlockSetup ReadCityBlockTemplate(string templateName)
    {

        var json = Resources.Load<TextAsset>("CityBlockAssets/CityBlockTemplates/" + templateName);
        CityBlockSetupJSON cityBlockTemplate = JsonUtility.FromJson<CityBlockSetupJSON>(json.text);

        List<BuildingTemplate> buildingSetups = new List<BuildingTemplate>();
        BuildingTemplate newSetup;
        BuildingTemplateJSON currentTemplate;

        for (int i = 0; i < cityBlockTemplate.potentialBuildings.Length; i++)
        {
            currentTemplate = cityBlockTemplate.potentialBuildings[i];
            newSetup = new BuildingTemplate();
            newSetup.AcceptJSON(currentTemplate);
            buildingSetups.Add(newSetup);
        }
        
        CityBlockSetup cityBlockSetup = new CityBlockSetup();
        cityBlockSetup.AcceptJSON(cityBlockTemplate, buildingSetups);
        return cityBlockSetup;

    }

    public CityMapSetup ReadCityMapTemplate(string templateName)
    {
        var json = Resources.Load<TextAsset>("CityMapAssets/CityMapTemplates/" + templateName);
        CityMapSetupJSON cityMapTemplate = JsonUtility.FromJson<CityMapSetupJSON>(json.text);

        List<CitySectorTemplate> citySectorSetups = new List<CitySectorTemplate>();
        CitySectorTemplate newSetup;
        CitySectorTemplateJSON currentTemplate;

        for (int i = 0; i < cityMapTemplate.citySectorTemplates.Length; i++)
        {
            currentTemplate = cityMapTemplate.citySectorTemplates[i];
            newSetup = new CitySectorTemplate();
            newSetup.AcceptJSON(currentTemplate);
            citySectorSetups.Add(newSetup);
        }

        CityMapSetup cityMapSetup = new CityMapSetup();
        cityMapSetup.AcceptJSON(cityMapTemplate, citySectorSetups);
        return cityMapSetup;
    }
    
}

[System.Serializable]
public class CityMapSetupJSON
{
    public int width;
    public CitySectorTemplateJSON[] citySectorTemplates;
    public string[] ensureSpawn, groundTiles, tags;
    public int[] groundRanges;
}

[System.Serializable]
public class CitySectorTemplateJSON
{
    public int ringCount;
    public RingJSON[] rings;
    public string[] roadTiles;
    public int[] roadTilesProbabilities;
    public string sidewalk;
    public string[] tags;
}

[System.Serializable]
public class RingJSON
{
    public string[] templates, priorityTemplates;
    public int[] limits, priorityLimits;
}

[System.Serializable]
public class CityBlockSetupJSON
{
    public int minBuildings;
    public BuildingTemplateJSON[] potentialBuildings;
    public string[] enemies, groundTypes;
    public int[] enemiesProbabilities, enemiesGroupCount, groundTypesProbabilities;
    public string[] tags;
}

[System.Serializable]
public class BuildingTemplateJSON
{
    public string buildingType;
    public int[] minSize, maxSize;
    public int count, probability;
    public string[] tags;
}

[System.Serializable]
public class BuildingSetupJSON
{
    public int exteriorDoors;
    public string startRoom, startDoor;
    public string[] enemies;
    public int[] enemiesProbabilities;
    public RoomTemplateJSON[] roomSetups;
}

[System.Serializable]
public class RoomTemplateJSON
{
    public string roomType, doorType;
    public int[] roomMin, roomMax, roomObjectsProbabilities, floorTypesProbabilities;
    public string[] roomsToMake, smallRoomsToMake, priorityRoomsToMake, floorTypes;
    public int[] roomsToMakeProbabilities, smallRoomsToMakeProbabilities, priorityRoomsToMakeProbabilities;
    public string[] roomObjects;
    public int[] roomObjectsCount;
    public string[] enemies;
    public int[] enemiesProbabilities;
    public string[] tags;
}