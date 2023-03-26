using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;
using NavMeshPlus.Components;
using System.Linq;

//Controls all the random generation for a map, only script that's attached to an object
//Will build the map from a string that represent's the type
public class RandomGenerationScript : MonoBehaviour
{
    //public Vector2Int bottomLeft, topRight;
    public string cityType;
    public Tilemap backMap, frontMap;
    public TileBase ground, buildingWall, floor;
    private int blockLength = 60, roadLength = 15;
    private GameObject cityParent;
    private NavMeshSurface Surface2D;


    void Start()
    {
        Surface2D = GameObject.Find("NavMesh").GetComponent<NavMeshSurface>();
        //ResetCity();
        //DrawCity();
        //pathfinder = GameObject.Find("A*").GetComponent<AstarPath>();
        //StartCoroutine(GeneratePaths());

        /*
        var boundaries = new BoundsInt(new Vector3Int(bottomLeft.x, bottomLeft.y, -1), new Vector3Int(topRight.x-bottomLeft.x, topRight.y-bottomLeft.y, 2));
        List<BoundsInt> buildingBounds = BinarySpacePartitioning(boundaries, 10, 10, 70, 70);
        backMap.SetTile(new Vector3Int(bottomLeft.x, bottomLeft.y, 0), ground);
        backMap.SetTile(new Vector3Int(topRight.x, topRight.y, 0), ground);
        backMap.FloodFill(new Vector3Int(0, 0, 0), ground);
        foreach(BoundsInt building in buildingBounds)
        {
            frontMap.BoxFill(Vector3Int.FloorToInt(building.center), buildingWall, building.min.x, building.min.y, building.max.x, building.max.y);
            //frontMap.BoxFill(Vector3Int.FloorToInt(building.center), null, building.min.x + 1, building.min.y + 1, building.max.x - 1, building.max.y - 1);
        }
        */
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("Ran here");
            GenerateCity(cityType, 0);
            //Debug.Log("Time taken: " + Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            //Surface2D.BuildNavMeshAsync();
            //StartCoroutine(GeneratePaths());
            //Surface2D.
        }
    }

    public void GenerateCity(string cityMapType, int count)
    {
        ResetCity();
        TemplateReader reader = new TemplateReader();
        var template = reader.ReadCityMapTemplate(cityMapType);

        cityParent = new GameObject("City Parent");

        int totalWidth = template.width * blockLength + (template.width + 1) * roadLength;

        var bottomLeft = new Vector2Int(-totalWidth / 2, -totalWidth / 2);
        var topRight = new Vector2Int(totalWidth / 2, totalWidth / 2);

        // backMap.SetTile(new Vector3Int(bottomLeft.x, bottomLeft.y, 0), ground);
        // backMap.SetTile(new Vector3Int(topRight.x, topRight.y, 0), ground);
        // backMap.FloodFill(new Vector3Int(0, 0, 0), ground);

        List<Vector2Int> citySectorCenters;
        citySectorCenters = GenerateCityCenters(template);

        CityBlock[,] cityMap = DetermineCityBlocks(template, citySectorCenters);
        Vector2Int landingZone = DetermineLandingZone(cityMap, template);

        DrawBackground(template, backMap);
        DrawRoads(template, citySectorCenters, cityMap, backMap);

        frontMap.SetTile(new Vector3Int(bottomLeft.x, bottomLeft.y, 0), buildingWall);
        frontMap.SetTile(new Vector3Int(topRight.x, topRight.y, 0), buildingWall);

        for (int i = 0; i < template.width; i++)
        {
            for (int j = 0; j < template.width; j++)
            {
                if (cityMap[i, j] != null)
                {
                    cityMap[i, j].DrawSelf(frontMap, backMap);
                }
            }
        }

        DrawLandingZone(landingZone, template);

        frontMap.SetTile(new Vector3Int(bottomLeft.x, bottomLeft.y, 0), null);
        frontMap.SetTile(new Vector3Int(topRight.x, topRight.y, 0), null);

        Surface2D.BuildNavMesh();

        //StartCoroutine(WaitMoment());

        for (int i = 0; i < template.width; i++)
        {
            for (int j = 0; j < template.width; j++)
            {
                if (cityMap[i, j] != null)
                {
                    cityMap[i, j].SpawnEnemies();
                }
            }
        }

        if (!CheckEnsureSpawn(template))
        {
            if (count < 5)
            {
                Debug.Log("Remade map");
                GenerateCity(cityType, count + 1);
            }
            else
            {
                Debug.Log("IMPORTANT OBJECTS NOT SPAWNED, RECURSION LIMIT HIT");
            }
        }
    }

    List<Vector2Int> GenerateCityCenters(CityMapSetup template)
    {
        List<Vector2Int> centers = new List<Vector2Int>(), potentialCenters = new List<Vector2Int>(), priorityCenters = new List<Vector2Int>();
        Vector2Int proposedCenter, nullVector = new Vector2Int(-1,-1);

        foreach (CitySectorTemplate citySectorTemplate in template.citySectorTemplates)
        {
            potentialCenters.Clear();
            for (int i = citySectorTemplate.ringCount; i < template.width - citySectorTemplate.ringCount; i++)
            {

                for (int j = citySectorTemplate.ringCount; j < template.width - citySectorTemplate.ringCount; j++)
                {
                    proposedCenter = new Vector2Int(i, j);
                    if (CheckCenterLocationValid(proposedCenter, citySectorTemplate, centers, template))
                    {
                        if (citySectorTemplate.tags.Contains("prioritizeNear"))
                        {
                            if (CheckCenterLocationPriority(proposedCenter, citySectorTemplate, centers, template))
                            {
                                priorityCenters.Add(proposedCenter);
                            }
                            else
                            {
                                potentialCenters.Add(proposedCenter);
                            }
                        }
                        else
                        {
                            potentialCenters.Add(proposedCenter);
                        }
                    }
                }
            }
            if (priorityCenters.Count > 0)
            {
                centers.Add(priorityCenters[Random.Range(0, priorityCenters.Count)]);
            }
            else if (potentialCenters.Count > 0)
            {
                centers.Add(potentialCenters[Random.Range(0, potentialCenters.Count)]);
            }
            else
            {
                centers.Add(nullVector);
            }
        }

        return centers;
    }

    bool CheckCenterLocationValid(Vector2Int proposedLocation, CitySectorTemplate curTemplate, List<Vector2Int> centers, CityMapSetup curSetup)
    {
        int maxDiff, extraDistanceBase, extraDistance;
        Vector2Int nullVector = new Vector2Int(-1,-1);

        if (curTemplate.tags.Contains("needsSpace"))
        {
            extraDistanceBase = 2;
        }
        else
        {
            extraDistanceBase = 0;
        }

        for(int i = 0; i < centers.Count; i++)
        {
            if (centers[i] == nullVector)
            {
                continue;
            }

            if (curSetup.citySectorTemplates[i].tags.Contains("needsSpace"))
            {
                extraDistance = extraDistanceBase + 2;
            }
            else
            {
                extraDistance = extraDistanceBase;
            }

            maxDiff = Mathf.Max(Mathf.Abs(centers[i].x - proposedLocation.x), Mathf.Abs(centers[i].y - proposedLocation.y));
            
            if (maxDiff <= curTemplate.ringCount + curSetup.citySectorTemplates[i].ringCount - 2 + extraDistance)
            {
                return false;
            }
        }
        return true;
    }

    bool CheckCenterLocationPriority(Vector2Int proposedLocation, CitySectorTemplate curTemplate, List<Vector2Int> centers, CityMapSetup curSetup)
    {
        int maxDiff, extraDistanceBase, extraDistance;
        Vector2Int nullVector = new Vector2Int(-1,-1);

        if (curTemplate.tags.Contains("needsSpace"))
        {
            extraDistanceBase = 2;
        }
        else
        {
            extraDistanceBase = 0;
        }

        for(int i = 0; i < centers.Count; i++)
        {
            if (centers[i] == nullVector)
            {
                continue;
            }

            if (curSetup.citySectorTemplates[i].tags.Contains("needsSpace"))
            {
                extraDistance = extraDistanceBase + 2;
            }
            else
            {
                extraDistance = extraDistanceBase;
            }

            maxDiff = Mathf.Max(Mathf.Abs(centers[i].x - proposedLocation.x), Mathf.Abs(centers[i].y - proposedLocation.y));
            
            if (maxDiff <= curTemplate.ringCount + curSetup.citySectorTemplates[i].ringCount - 1 + extraDistance)
            {
                return true;
            }
        }
        return false;

    }

    CityBlock[,] DetermineCityBlocks(CityMapSetup template, List<Vector2Int> centers)
    {
        CityBlock[,] cityMap = GenerateEmptyMap(template.width);
        List<Vector2Int> curBlockLocations = new List<Vector2Int>();
        Vector2Int curLocation, curDirection, chosenLocation;
        float curDirectionAngle;
        int totalBlocks, chosenLocationIdx, templateIdx;
        List<string> potentialTemplates = new List<string>();
        string chosenTemplate;
        List<int> templateCounts = new List<int>(), templateLimits = new List<int>();

        int totalWidth = template.width * blockLength + (template.width + 1) * roadLength;

        var bottomLeft = new Vector2Int(-totalWidth / 2, -totalWidth / 2);
        var topRight = new Vector2Int(totalWidth / 2, totalWidth / 2);

        GameObject blockParent;

        Vector2Int nullVector = new Vector2Int(-1,-1);
        for (int centerIdx = 0; centerIdx < centers.Count; centerIdx++)
        {
            if (centers[centerIdx] == nullVector)
            {
                continue;
            }
            for (int ringIdx = 0; ringIdx < template.citySectorTemplates[centerIdx].ringCount; ringIdx++)
            {
                curBlockLocations.Clear();
                if (ringIdx == 0)
                {
                    curBlockLocations.Add(centers[centerIdx]);
                }
                else
                {
                    curLocation = new Vector2Int(centers[centerIdx].x + ringIdx, centers[centerIdx].y - ringIdx);
                    curDirectionAngle = (Mathf.PI / 2f);
                    for (int i = 0; i < 4; i++)
                    {
                        curDirection = new Vector2Int((int)Mathf.Round(Mathf.Cos(curDirectionAngle) / 0.707f), (int)Mathf.Round(Mathf.Sin(curDirectionAngle) / 0.707f));
                        for (int j = 0; j < ringIdx * 2; j++)
                        {
                            curLocation += curDirection;
                            curBlockLocations.Add(curLocation);
                        }
                        curDirectionAngle += (Mathf.PI / 2f);
                    }
                }

                //PRIORITY QUEUEING
                potentialTemplates.Clear();
                templateCounts.Clear();
                templateLimits.Clear();

                for (int i = 0; i < template.citySectorTemplates[centerIdx].priorityTemplates[ringIdx].Count; i++)
                {
                    potentialTemplates.Add(template.citySectorTemplates[centerIdx].priorityTemplates[ringIdx][i]);
                    templateCounts.Add(0);
                    templateLimits.Add(template.citySectorTemplates[centerIdx].priorityLimits[ringIdx][i]);
                }

                totalBlocks = curBlockLocations.Count;
                for (int i = 0; i < totalBlocks; i++)
                {
                    if (potentialTemplates.Count == 0)
                    {
                        break;
                    }
                    chosenLocationIdx = Random.Range(0, curBlockLocations.Count);
                    chosenLocation = curBlockLocations[chosenLocationIdx];
                    templateIdx = Random.Range(0, potentialTemplates.Count);
                    chosenTemplate = potentialTemplates[templateIdx];

                    if (chosenTemplate != "empty")
                    {
                        blockParent = new GameObject("Block Parent " + chosenLocation.x + " " + chosenLocation.y);
                        blockParent.transform.parent = cityParent.transform;

                        cityMap[chosenLocation.x, chosenLocation.y] = new CityBlock(new BoundsInt(new Vector3Int(bottomLeft.x + roadLength * (chosenLocation.y + 1) + blockLength * chosenLocation.y,
                                                                                    bottomLeft.y + roadLength * (chosenLocation.x + 1) + blockLength * chosenLocation.x, 0),
                                                                                    new Vector3Int(blockLength, blockLength, 0)), buildingWall, chosenTemplate, blockParent);
                    }

                    curBlockLocations.RemoveAt(chosenLocationIdx);
                    templateCounts[templateIdx]++;
                    if (templateCounts[templateIdx] >= templateLimits[templateIdx])
                    {
                        potentialTemplates.RemoveAt(templateIdx);
                        templateCounts.RemoveAt(templateIdx);
                        templateLimits.RemoveAt(templateIdx);
                    }
                }

                //NORMAL QUEUEING
                potentialTemplates.Clear();
                templateCounts.Clear();
                templateLimits.Clear();

                for (int i = 0; i < template.citySectorTemplates[centerIdx].ringTemplates[ringIdx].Count; i++)
                {
                    potentialTemplates.Add(template.citySectorTemplates[centerIdx].ringTemplates[ringIdx][i]);
                    templateCounts.Add(0);
                    templateLimits.Add(template.citySectorTemplates[centerIdx].ringTemplateLimits[ringIdx][i]);
                }

                totalBlocks = curBlockLocations.Count;
                for (int i = 0; i < totalBlocks; i++)
                {
                    if (potentialTemplates.Count == 0)
                    {
                        break;
                    }

                    chosenLocationIdx = Random.Range(0, curBlockLocations.Count);
                    chosenLocation = curBlockLocations[chosenLocationIdx];
                    templateIdx = Random.Range(0, potentialTemplates.Count);
                    chosenTemplate = potentialTemplates[templateIdx];

                    if (chosenTemplate != "empty")
                    {
                        blockParent = new GameObject("Block Parent " + chosenLocation.x + " " + chosenLocation.y);
                        blockParent.transform.parent = cityParent.transform;

                        cityMap[chosenLocation.x, chosenLocation.y] = new CityBlock(new BoundsInt(new Vector3Int(bottomLeft.x + roadLength * (chosenLocation.y + 1) + blockLength * chosenLocation.y,
                                                                                    bottomLeft.y + roadLength * (chosenLocation.x + 1) + blockLength * chosenLocation.x, 0),
                                                                                    new Vector3Int(blockLength, blockLength, 0)), buildingWall, chosenTemplate, blockParent);
                    }

                    curBlockLocations.RemoveAt(chosenLocationIdx);
                    templateCounts[templateIdx]++;
                    if (templateCounts[templateIdx] >= templateLimits[templateIdx])
                    {
                        potentialTemplates.RemoveAt(templateIdx);
                        templateCounts.RemoveAt(templateIdx);
                        templateLimits.RemoveAt(templateIdx);
                    }
                }

            }
        }

        return cityMap;
    }

    CityBlock[,] GenerateEmptyMap(int width)
    {
        CityBlock[,] cityMap = new CityBlock[width, width];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < width; j++)
            {
                cityMap[i,j] = null;
            }
        }
        return cityMap;
    }

    Vector2Int DetermineLandingZone(CityBlock[,] cityMap, CityMapSetup template)
    {
        List<Vector2Int> potentialLocations = new List<Vector2Int>(), priorityLocations = new List<Vector2Int>();
        Vector2Int proposedLocation;

        for (int i = 0; i < template.width; i++)
        {
            for (int j = 0; j < template.width; j++)
            {
                if (cityMap[i, j] == null)
                {
                    proposedLocation = new Vector2Int(i, j);
                    if (PriorityLandingZone(proposedLocation, cityMap, template))
                    {
                        priorityLocations.Add(proposedLocation);
                    }
                    else
                    {
                        potentialLocations.Add(new Vector2Int(i, j));
                    }
                }
            }
        }

        if (priorityLocations.Count > 0)
        {
            return priorityLocations[Random.Range(0, priorityLocations.Count)];
        }
        else if (potentialLocations.Count > 0)
        {
            return potentialLocations[Random.Range(0, potentialLocations.Count)];
        }
        else
        {
            proposedLocation = new Vector2Int(Random.Range(0, template.width), Random.Range(0, template.width));
            cityMap[proposedLocation.x, proposedLocation.y] = null;
            return proposedLocation;
        }
    }

    bool PriorityLandingZone(Vector2Int location, CityBlock[,] cityMap, CityMapSetup template)
    {
        var bottomLeft = new Vector2Int(Mathf.Max(0, location.x - 1), Mathf.Max(0, location.y - 1));
        var topRight = new Vector2Int(Mathf.Min(template.width - 1, location.x + 1), Mathf.Min(template.width - 1, location.y + 1));

        for (int i = bottomLeft.x; i <= topRight.x; i++)
        {
            for (int j = bottomLeft.y; j <= topRight.y; j++)
            {
                if (cityMap[i, j] != null)
                {
                    return true;
                }
            }
        }

        return false;

    }

    void DrawLandingZone(Vector2Int landingZone, CityMapSetup template)
    {
        GameObject player = GameObject.Find("Player");

        int totalWidth = template.width * blockLength + (template.width + 1) * roadLength;

        var bottomLeft = new Vector2Int(-totalWidth / 2, -totalWidth / 2);
        var topRight = new Vector2Int(totalWidth / 2, totalWidth / 2);

        player.transform.position = new Vector3(bottomLeft.x + roadLength * (landingZone.y + 1) + blockLength * landingZone.y + blockLength / 2f,
                                                bottomLeft.y + roadLength * (landingZone.x + 1) + blockLength * landingZone.x + blockLength / 2f, 0);
                                                
        return;
    }

    void DrawBackground(CityMapSetup template, Tilemap backmap)
    {
        var offset = new Vector2(Random.Range(-200, 200), Random.Range(-200, 200));
        Vector2 curPoint;
        float noise;

        int totalWidth = template.width * blockLength + (template.width + 1) * roadLength;

        var bottomLeft = new Vector2Int(-totalWidth / 2, -totalWidth / 2);
        var topRight = new Vector2Int(totalWidth / 2, totalWidth / 2);

        TileBase[] tiles = new TileBase[template.groundTiles.Length];
        for (int i = 0; i < template.groundTiles.Length; i++)
        {
            tiles[i] = Resources.Load<TileBase>("Tiles/" + template.groundTiles[i]);
        }

        for (int i = (int)bottomLeft.x; i <= topRight.x; i++)
        {
            for (int j = (int)bottomLeft.y; j <= topRight.y; j++)
            {
                curPoint = new Vector2(i / 50f, j / 50f) + offset;
                noise = (int)(Mathf.PerlinNoise(curPoint.x, curPoint.y) * 100);

                for (int k = 0; k < tiles.Length; k++)
                {
                    if (noise < template.groundRanges[k])
                    {
                        backMap.SetTile(new Vector3Int(i, j, 0), tiles[k]);
                        break;
                    }
                }
            }
        }
    }

    void DrawRoads(CityMapSetup template, List<Vector2Int> centers, CityBlock[,] cityMap, Tilemap backMap)
    {
        Vector2Int topRightIdx, bottomLeftIdx, centerTile, curDirection, tileAmounts, fillDirection, startCorner, endCorner, bottomLeftCorner, topRightCorner;
        float curAngle, tileAmountAngle, fillAngle;
        TileBase[] roadTiles;
        TileBase sidewalk;
        int randomNum;

        int totalWidth = template.width * blockLength + (template.width + 1) * roadLength;

        var bottomLeft = new Vector2Int(-totalWidth / 2, -totalWidth / 2);
        var topRight = new Vector2Int(totalWidth / 2, totalWidth / 2);

        Vector2Int nullVector = new Vector2Int(-1,-1);
        for (int i = centers.Count - 1; i >= 0; i--)
        {
            if (centers[i] == nullVector)
            {
                continue;
            }

            roadTiles = new TileBase[template.citySectorTemplates[i].roadTiles.Length];
            for (int j = 0; j < template.citySectorTemplates[i].roadTiles.Length; j++)
            {
                roadTiles[j] = Resources.Load<TileBase>("Tiles/" + template.citySectorTemplates[i].roadTiles[j]);
            }
            sidewalk = Resources.Load<TileBase>("Tiles/" + template.citySectorTemplates[i].sidewalk);

            bottomLeftIdx = new Vector2Int(centers[i].x - template.citySectorTemplates[i].ringCount + 1, centers[i].y - template.citySectorTemplates[i].ringCount + 1);
            topRightIdx = new Vector2Int(centers[i].x + template.citySectorTemplates[i].ringCount - 1, centers[i].y + template.citySectorTemplates[i].ringCount - 1);

            for (int j = bottomLeftIdx.x; j <= topRightIdx.x; j++)
            {
                for (int k = bottomLeftIdx.y; k <= topRightIdx.y; k++)
                {
                    if (cityMap[j,k] == null)
                    {
                        continue;
                    }

                    centerTile = new Vector2Int((int)(bottomLeft.x + roadLength * (k + 1) + blockLength * (k + 0.5f)), (int)(bottomLeft.y + roadLength * (j + 1) + blockLength * (j + 0.5f)));
                    //Debug.Log("Center tile: " + centerTile);
                    
                    for (int l = 0; l < 4; l++)
                    {
                        curAngle = l * (Mathf.PI / 2) + (Mathf.PI) / 4;
                        curDirection = new Vector2Int((int)Mathf.Round(Mathf.Cos(curAngle) / 0.707f), (int)Mathf.Round(Mathf.Sin(curAngle) / 0.707f));

                        //Theres another -1 in the y values for when cos > 0, not sure why, nvm I think they both just need to back off from the building
                        tileAmountAngle = l * (Mathf.PI / 2) + (Mathf.PI);
                        tileAmounts = new Vector2Int((int)Mathf.Round(Mathf.Abs(Mathf.Cos(tileAmountAngle) * (blockLength + roadLength) + Mathf.Sin(tileAmountAngle) * (roadLength - 4))), 
                                                     (int)Mathf.Round(Mathf.Abs(Mathf.Cos(tileAmountAngle) * (roadLength - 4) + Mathf.Sin(tileAmountAngle) * (blockLength + roadLength))));

                        fillAngle = l * (Mathf.PI / 2) + (5 * Mathf.PI / 4f);
                        fillDirection = new Vector2Int((int)Mathf.Round(Mathf.Cos(fillAngle) / 0.707f), (int)Mathf.Round(Mathf.Sin(fillAngle) / 0.707f));

                        startCorner = new Vector2Int(centerTile.x + curDirection.x * (blockLength / 2 + roadLength - 2), centerTile.y + curDirection.y * (blockLength / 2 + roadLength - 2));
                        endCorner = new Vector2Int(startCorner.x + tileAmounts.x * fillDirection.x, startCorner.y + tileAmounts.y * fillDirection.y);

                        bottomLeftCorner = new Vector2Int(Mathf.Min(startCorner.x, endCorner.x), Mathf.Min(startCorner.y, endCorner.y));
                        topRightCorner = new Vector2Int(Mathf.Max(startCorner.x, endCorner.x), Mathf.Max(startCorner.y, endCorner.y));

                        for (int x = bottomLeftCorner.x; x <= topRightCorner.x; x++)
                        {
                            for (int y = bottomLeftCorner.y; y <= topRightCorner.y; y++)
                            {
                                randomNum = Random.Range(0, 100);
                                for (int tileIdx = 0; tileIdx < roadTiles.Length; tileIdx++)
                                {
                                    if (randomNum < template.citySectorTemplates[i].roadTilesProbabilities[tileIdx])
                                    {
                                        backMap.SetTile(new Vector3Int(x, y, 0), roadTiles[tileIdx]);
                                        break;
                                    }
                                    randomNum -= template.citySectorTemplates[i].roadTilesProbabilities[tileIdx];
                                }
                            }
                        }

                        //Sidewalk generation
                        startCorner -= new Vector2Int(curDirection.x * (roadLength - 3), curDirection.y * (roadLength - 3));

                        fillAngle = l * (Mathf.PI / 2) + (Mathf.PI / 2f);
                        //fillDirection = new Vector2Int((int)Mathf.Round(Mathf.Cos(fillAngle) / 0.707f), (int)Mathf.Round(Mathf.Sin(fillAngle) / 0.707f));

                        tileAmounts = new Vector2Int((int)Mathf.Round(Mathf.Cos(tileAmountAngle) * (blockLength + 1)), 
                                                     (int)Mathf.Round(Mathf.Sin(tileAmountAngle) * (blockLength + 1)));

                        endCorner = startCorner + tileAmounts;
                        
                        bottomLeftCorner = new Vector2Int(Mathf.Min(startCorner.x, endCorner.x), Mathf.Min(startCorner.y, endCorner.y));
                        topRightCorner = new Vector2Int(Mathf.Max(startCorner.x, endCorner.x), Mathf.Max(startCorner.y, endCorner.y));

                        for (int x = bottomLeftCorner.x; x <= topRightCorner.x; x++)
                        {
                            for (int y = bottomLeftCorner.y; y <= topRightCorner.y; y++)
                            {
                                backMap.SetTile(new Vector3Int(x, y, 0), sidewalk);
                            }
                        }
                    }
                }
            }
        }
    }

    public void ResetCity()
    {
        Destroy(cityParent);
        frontMap.ClearAllTiles();
        backMap.ClearAllTiles();
    }

    bool CheckEnsureSpawn(CityMapSetup template)
    {
        foreach (string obj in template.ensureSpawn)
        {
            if (GameObject.Find(obj + "(Clone)") == null)
            {
                return false;
            }
        }

        return true;
    }

}