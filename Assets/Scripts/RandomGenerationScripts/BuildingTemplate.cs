using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingTemplate
{

    public string buildingType;
    public Vector2Int minSize, maxSize;
    public int count, probability;
    public string[] tags;

    public void AcceptJSON(BuildingTemplateJSON templateJSON)
    {

        buildingType = templateJSON.buildingType;
        minSize = new Vector2Int(templateJSON.minSize[0], templateJSON.minSize[1]);
        maxSize = new Vector2Int(templateJSON.maxSize[0], templateJSON.maxSize[1]);
        count = templateJSON.count;
        probability = templateJSON.probability;
        
        tags = new string[templateJSON.tags.Length];
        for (int i = 0; i < templateJSON.tags.Length; i++)
        {
            tags[i] = templateJSON.tags[i];
        }

    }

}