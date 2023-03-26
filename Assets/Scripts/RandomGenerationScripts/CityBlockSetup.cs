using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Contains info to generate the buildings inside a CityBlock
public class CityBlockSetup
{
    public int minBuildings;
    public List<BuildingTemplate> buildingTemplates;
    public string[] enemies, groundTypes;
    public int[] enemiesProbabilities, enemiesGroupCount, groundTypesProbabilities;
    public string[] tags;

    public void AcceptJSON(CityBlockSetupJSON templateJSON, List<BuildingTemplate> templates)
    {
        buildingTemplates = templates;

        minBuildings = templateJSON.minBuildings;

        enemies = new string[templateJSON.enemies.Length];
        enemiesProbabilities = new int[templateJSON.enemiesProbabilities.Length];
        enemiesGroupCount = new int[templateJSON.enemiesProbabilities.Length];

        for (int i = 0; i < templateJSON.enemies.Length; i++)
        {
            enemies[i] = templateJSON.enemies[i];
            enemiesProbabilities[i] = templateJSON.enemiesProbabilities[i];
            enemiesGroupCount[i] = templateJSON.enemiesGroupCount[i];
        }

        groundTypes = new string[templateJSON.groundTypes.Length];
        groundTypesProbabilities = new int[templateJSON.groundTypes.Length];

        for (int i = 0; i < templateJSON.groundTypes.Length; i++)
        {
            groundTypes[i] = templateJSON.groundTypes[i];
            groundTypesProbabilities[i] = templateJSON.groundTypesProbabilities[i];
        }

        tags = new string[templateJSON.tags.Length];
        for (int i = 0; i < templateJSON.tags.Length; i++)
        {
            tags[i] = templateJSON.tags[i];
        }
    }

}