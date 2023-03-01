using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Holds info about a map such as all the types of cities it should build
public class CityMapSetup
{
    public int width;
    public List<CitySectorTemplate> citySectorTemplates;
    public string[] ensureSpawn, tags;

    public void AcceptJSON(CityMapSetupJSON templateJSON, List<CitySectorTemplate> templates)
    {
        width = templateJSON.width;
        citySectorTemplates = templates;

        ensureSpawn = new string[templateJSON.ensureSpawn.Length];
        for (int i = 0; i < templateJSON.ensureSpawn.Length; i++)
        {
            ensureSpawn[i] = templateJSON.ensureSpawn[i];
        }

        tags = new string[templateJSON.tags.Length];
        for (int i = 0; i < templateJSON.tags.Length; i++)
        {
            tags[i] = templateJSON.tags[i];
        }
    }

}