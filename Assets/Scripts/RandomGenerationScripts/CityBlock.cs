using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

//Contains info for a City Block, such as its bounds, buildings, and type
//Has functions to generate it's buildings
public class CityBlock
{
    public BoundsInt bounds;
    public string blockType;
    private TileBase tile;
    private List<Building> buildings = new List<Building>();
    private GameObject blockParent;
    CityBlockSetup template;

    public CityBlock(BoundsInt boundsTemp, TileBase tileTemp, string typeTemp, GameObject parentTemp)
    {
        bounds = boundsTemp;
        tile = tileTemp;
        blockType = typeTemp;
        blockParent = parentTemp;

        TemplateReader reader = new TemplateReader();
        template = reader.ReadCityBlockTemplate(blockType);

        SelfSetup();
    }

    public void DrawSelf(Tilemap frontMap, Tilemap backMap)
    {
        foreach (Building building in buildings)
        {
            if (!building.failed)
            {
                building.DrawSelf(frontMap, backMap);
            }
        }
    }

    private void SelfSetup()
    {
        GenerateBuildings();
    }

    void GenerateBuildings()
    {
        buildings = new List<Building>();
        BuildingTemplate curTemplate;
        Building newBuilding;
        Vector2Int size;
        List<BoundsInt> potentialLocations = new List<BoundsInt>();
        BoundsInt curBounds;
        GameObject buildingParent;

        for (int templateIdx = 0; templateIdx < template.buildingTemplates.Count; templateIdx++)
        {
            curTemplate = template.buildingTemplates[templateIdx];
            for (int buildingCount = 0; buildingCount < curTemplate.count; buildingCount++)
            {
                if (Random.Range(0, 100) < curTemplate.probability || curTemplate.count - buildingCount < template.minBuildings - buildings.Count)
                {
                    size = new Vector2Int(Random.Range(curTemplate.minSize.x, curTemplate.maxSize.x + 1), 
                                        Random.Range(curTemplate.minSize.y, curTemplate.maxSize.y + 1));

                    potentialLocations.Clear();
                    for (int i = bounds.min.x; i <= bounds.max.x - size.x + 1; i++)
                    {
                        for (int j = bounds.min.y; j <= bounds.max.y - size.y + 1; j++)
                        {
                            curBounds = new BoundsInt(new Vector3Int(i, j, 0), new Vector3Int(size.x, size.y, 0));
                            if (CheckBuildingLocationValid(curBounds))
                            {
                                potentialLocations.Add(curBounds);
                            }
                        }
                    }

                    if (potentialLocations.Count > 0)
                    {
                        buildingParent = new GameObject("Building Parent " + (buildings.Count));
                        buildingParent.transform.parent = blockParent.transform;
                        newBuilding = new Building(potentialLocations[Random.Range(0, potentialLocations.Count)], tile, curTemplate.buildingType, buildingParent);
                        newBuilding.AcceptTags(curTemplate.tags);
                        buildings.Add(newBuilding);
                    }
                }
            }
        }

        if (buildings.Count == 0)
        {
            Debug.Log("No buildings...");
        }
    }

    bool CheckBuildingLocationValid(BoundsInt testBuilding)
    {

        if (testBuilding.min.x < bounds.min.x || testBuilding.min.y < bounds.min.y || testBuilding.max.x > bounds.max.x || testBuilding.max.y > bounds.max.y)
        {
            return false;
        }

        foreach (Building building in buildings)
        {
            for (int i = testBuilding.min.x - 1; i <= testBuilding.max.x + 1; i++)
            {
                for (int j = testBuilding.min.y - 1; j <= testBuilding.max.y + 1; j++)
                {
                    if ((building.bounds.min.x <= i && i <= building.bounds.max.x) && (building.bounds.min.y <= j && j <= building.bounds.max.y))
                    {
                        return false;
                    }
                }
            }
        }
        return true;

    }

    public void SpawnEnemies()
    {
        List<Vector2> patrolLocations = new List<Vector2>();
        Door curDoor;
        Vector2 curLocation;
        float curOrientationAngle;
        foreach (Building building in buildings)
        {

            if (building.failed)
            {
                continue;
            }

            building.SpawnEnemies();

            if (building.template.tags.Contains("noPatrol"))
            {
                continue;
            }

            curDoor = building.ReturnFrontDoor();
            curLocation = curDoor.location;
            curOrientationAngle = curDoor.orientationAngle + Mathf.PI;
            patrolLocations.Add(curLocation + new Vector2(2f * Mathf.Cos(curOrientationAngle) + 0.5f, 2f * Mathf.Sin(curOrientationAngle) + 0.5f));
        }

        GameObject curEnemy, newEnemy;
        List<EnemyScript> followers;
        EnemyScript leader;
        EnemySetup curSetup;
        int groupSize;

        for (int i = 0; i < template.enemies.Length; i++)
        {
            groupSize = 0;
            for (int j = 0; j < template.enemiesGroupCount[i]; j++)
            {
                if (Random.Range(0, 100) < template.enemiesProbabilities[i])
                {
                    groupSize++;
                }
            }

            if (groupSize == 1 || patrolLocations.Count == 1)
            {
                if (patrolLocations.Count > 1)
                {
                    curLocation = patrolLocations[Random.Range(0, patrolLocations.Count)];
                    curSetup = new EnemySetup();
                    curSetup.type = 4;
                    curSetup.patrolLocations = patrolLocations;

                    curEnemy = Resources.Load<GameObject>("EnemyAssets/" + template.enemies[i]);
                    newEnemy = GameObject.Instantiate(curEnemy, curLocation, Quaternion.Euler(0,0,0), blockParent.transform);
                    newEnemy.GetComponent<EnemyScript>().SetupEnemy(curSetup);
                }
                else if (patrolLocations.Count == 1)
                {
                    curLocation = patrolLocations[0];
                    curSetup = new EnemySetup();
                    curSetup.type = 0;
                    curSetup.guardLocation = curLocation;

                    curEnemy = Resources.Load<GameObject>("EnemyAssets/" + template.enemies[i]);
                    newEnemy = GameObject.Instantiate(curEnemy, curLocation, Quaternion.Euler(0,0,0), blockParent.transform);
                    newEnemy.GetComponent<EnemyScript>().SetupEnemy(curSetup);
                }
            }
            else if (groupSize > 1 && patrolLocations.Count > 1)
            {
                
                curLocation = patrolLocations[Random.Range(0, patrolLocations.Count)];
                curEnemy = Resources.Load<GameObject>("EnemyAssets/" + template.enemies[i]);
                newEnemy = GameObject.Instantiate(curEnemy, curLocation, Quaternion.Euler(0,0,0), blockParent.transform);
                leader = newEnemy.GetComponent<EnemyScript>();

                followers = new List<EnemyScript>();
                for (int j = 0; j < groupSize - 1; j++)
                {
                    curSetup = new EnemySetup();
                    curSetup.type = 2;
                    curSetup.patrolLocations = patrolLocations;
                    curSetup.leader = leader;
                    newEnemy = GameObject.Instantiate(curEnemy, curLocation, Quaternion.Euler(0,0,0), blockParent.transform);
                    newEnemy.GetComponent<EnemyScript>().SetupEnemy(curSetup);
                    followers.Add(newEnemy.GetComponent<EnemyScript>());
                }

                curSetup = new EnemySetup();
                curSetup.type = 4;
                curSetup.patrolLocations = patrolLocations;
                curSetup.leader = leader;
                curSetup.followers = followers;
                leader.GetComponent<EnemyScript>().SetupEnemy(curSetup);
            }

        }
    }
}
