using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
            building.DrawSelf(frontMap, backMap);
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
                        buildings.Add(new Building(potentialLocations[Random.Range(0, potentialLocations.Count)], tile, curTemplate.buildingType, buildingParent));
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

    List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight, int maxWidth, int maxHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);
        while(roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if(room.size.y >= minHeight && room.size.x >= minWidth)
            {
                if (Random.value > ((roomsList.Count * 4) - 32) / 100f || room.size.y > maxHeight || room.size.x > maxWidth)
                {
                    if(Random.value < 0.5f)
                    {
                        if(room.size.y >= minHeight * 2)
                        {
                            SplitHorizontally(minHeight, roomsQueue, room);
                        }else if(room.size.x >= minWidth * 2)
                        {
                            SplitVertically(minWidth, roomsQueue, room);
                        }else if (CheckRoomValid(room))
                        {
                            roomsList.Add(room);
                        }
                    }
                    else
                    {
                        if (room.size.x >= minWidth * 2)
                        {
                            SplitVertically(minWidth, roomsQueue, room);
                        }
                        else if (room.size.y >= minHeight * 2)
                        {
                            SplitHorizontally(minHeight, roomsQueue, room);
                        }
                        else if (CheckRoomValid(room))
                        {
                            roomsList.Add(room);
                        }
                    }
                }
                else if (CheckRoomValid(room))
                {
                    roomsList.Add(room);
                }
            }
        }
        return roomsList;
    }

    void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
            new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
            new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private int clearDistance = 20;
    bool CheckRoomValid(BoundsInt room)
    {
        if (room.Contains(new Vector3Int(0,0,0)))
        {
            return false;
        }
        else if (room.Contains(new Vector3Int(clearDistance, clearDistance, 0)))
        {
            return false;
        }
        else if (room.Contains(new Vector3Int(clearDistance, -clearDistance, 0)))
        {
            return false;
        }
        else if (room.Contains(new Vector3Int(-clearDistance, clearDistance, 0)))
        {
            return false;
        }
        else if (room.Contains(new Vector3Int(-clearDistance, -clearDistance, 0)))
        {
            return false;
        }
        return true;
    }


}
