using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Contains info to generate the buildings inside a CityBlock
public class CityBlockSetup
{
    public int minBuildings;
    public List<BuildingTemplate> buildingTemplates;
    public string[] enemies;
    public int[] enemiesProbabilities, enemiesGroupCount;
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

        tags = new string[templateJSON.tags.Length];
        for (int i = 0; i < templateJSON.tags.Length; i++)
        {
            tags[i] = templateJSON.tags[i];
        }
    }

}