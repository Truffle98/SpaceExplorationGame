using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CitySectorTemplate
{
    public int ringCount;
    public List<List<string>> ringTemplates;
    public List<List<int>> ringTemplateLimits;
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

        tags = new string[templateJSON.tags.Length];
        for (int i = 0; i < templateJSON.tags.Length; i++)
        {
            tags[i] = templateJSON.tags[i];
        }

    }
}