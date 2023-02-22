using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CityBlockSetup
{
    public int minBuildings;
    public List<BuildingTemplate> buildingTemplates;
    public string[] tags;

    public void AcceptJSON(CityBlockSetupJSON templateJSON, List<BuildingTemplate> templates)
    {
        buildingTemplates = templates;

        minBuildings = templateJSON.minBuildings;

        tags = new string[templateJSON.tags.Length];
        for (int i = 0; i < templateJSON.tags.Length; i++)
        {
            tags[i] = templateJSON.tags[i];
        }
    }

}