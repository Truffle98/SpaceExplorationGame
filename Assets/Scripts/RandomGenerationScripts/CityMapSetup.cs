using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CityMapSetup
{

    public int width;
    public List<CitySectorTemplate> citySectorTemplates;
    public string[] tags;

    public void AcceptJSON(CityMapSetupJSON templateJSON, List<CitySectorTemplate> templates)
    {
        width = templateJSON.width;
        citySectorTemplates = templates;

        tags = new string[templateJSON.tags.Length];
        for (int i = 0; i < templateJSON.tags.Length; i++)
        {
            tags[i] = templateJSON.tags[i];
        }
    }

}