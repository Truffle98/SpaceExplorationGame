using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Used in conjunction with a CityMapSetup to generate all the CityBlock types
public class CitySectorTemplate
{
    public int ringCount;
    public List<List<string>> ringTemplates, priorityTemplates;
    public List<List<int>> ringTemplateLimits, priorityLimits;
    public string[] roadTiles;
    public int[] roadTilesProbabilities;
    public string sidewalk;
    public string[] tags;

    public void AcceptJSON(CitySectorTemplateJSON templateJSON)
    {
        ringCount = templateJSON.ringCount;

        ringTemplates = new List<List<string>>();
        ringTemplateLimits = new List<List<int>>();

        List<string> tempTemplates;
        List<int> tempLimits;
        for (int i = 0; i < templateJSON.rings.Length; i++)
        {
            tempTemplates = new List<string>();
            tempLimits = new List<int>();

            for (int j = 0; j < templateJSON.rings[i].templates.Length; j++)
            {
                tempTemplates.Add(templateJSON.rings[i].templates[j]);
                tempLimits.Add(templateJSON.rings[i].limits[j]);
            }

            ringTemplates.Add(tempTemplates);
            ringTemplateLimits.Add(tempLimits);
        }

        priorityTemplates = new List<List<string>>();
        priorityLimits = new List<List<int>>();

        for (int i = 0; i < templateJSON.rings.Length; i++)
        {
            tempTemplates = new List<string>();
            tempLimits = new List<int>();

            for (int j = 0; j < templateJSON.rings[i].priorityTemplates.Length; j++)
            {
                tempTemplates.Add(templateJSON.rings[i].priorityTemplates[j]);
                tempLimits.Add(templateJSON.rings[i].priorityLimits[j]);
            }

            priorityTemplates.Add(tempTemplates);
            priorityLimits.Add(tempLimits);
        }

        roadTiles = new string[templateJSON.roadTiles.Length];
        roadTilesProbabilities = new int[templateJSON.roadTiles.Length];

        for (int i = 0; i < templateJSON.roadTiles.Length; i++)
        {
            roadTiles[i] = templateJSON.roadTiles[i];
            roadTilesProbabilities[i] = templateJSON.roadTilesProbabilities[i];
        }

        sidewalk = templateJSON.sidewalk;

        tags = new string[templateJSON.tags.Length];
        for (int i = 0; i < templateJSON.tags.Length; i++)
        {
            tags[i] = templateJSON.tags[i];
        }
    }
}