using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomGenerationScript : MonoBehaviour
{

    public int width;
    public TileBase[] testColors;

    //public Vector2Int bottomLeft, topRight;
    public GameObject buildingPrefab;
    public Tilemap backMap, frontMap;
    public TileBase ground, buildingWall, floor;

    private CityBlock[,] cityMap;
    private int blockLength = 60, roadLength = 15;


    void Start()
    {
        ResetCity();
        DrawCity();

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
            ResetCity();
            DrawCity();
        }
    }

    void DrawCity()
    {

        int totalWidth = width * blockLength + (width + 1) * roadLength;

        var bottomLeft = new Vector2Int(-totalWidth / 2, -totalWidth / 2);
        var topRight = new Vector2Int(totalWidth / 2, totalWidth / 2);

        int buildingTypeNum;
        string buildingType = "";

        backMap.SetTile(new Vector3Int(bottomLeft.x, bottomLeft.y, 0), ground);
        backMap.SetTile(new Vector3Int(topRight.x, topRight.y, 0), ground);
        backMap.FloodFill(new Vector3Int(0, 0, 0), ground);

        for (int i = 0; i < width; i++)
        {

            for (int j = 0; j < width; j++)
            {

                if (i != Mathf.Floor(width / 2) || j != Mathf.Floor(width / 2))
                {
                    buildingTypeNum = Random.Range(0, 2);
                    buildingTypeNum = 0;
                    if (buildingTypeNum == 0)
                    {
                        buildingType = "block";
                    }
                    else if (buildingTypeNum == 1)
                    {
                        buildingType = "empty";
                    }

                    cityMap[i, j] = new CityBlock(new BoundsInt(new Vector3Int(bottomLeft.x + roadLength * (j + 1) + blockLength * j, bottomLeft.y + roadLength * (i + 1) + blockLength * i, -1),
                                    new Vector3Int(blockLength, blockLength, 2)), buildingWall, buildingType);
                }
                else
                {
                    cityMap[i, j] = new CityBlock(new BoundsInt(new Vector3Int(bottomLeft.x + roadLength * (j + 1) + blockLength * j, bottomLeft.y + roadLength * (i + 1) + blockLength * i, -1),
                                    new Vector3Int(blockLength, blockLength, 2)), buildingWall, "empty");
                }

            }

        }

        frontMap.SetTile(new Vector3Int(bottomLeft.x, bottomLeft.y, 0), buildingWall);
        frontMap.SetTile(new Vector3Int(topRight.x, topRight.y, 0), buildingWall);

        cityMap[0,0].DrawSelf(frontMap, backMap);
        for (int i = 0; i < width; i++)
        {

            for (int j = 0; j < width; j++)
            {

                //cityMap[i, j].DrawSelf(frontMap);

            }

        }

        frontMap.SetTile(new Vector3Int(bottomLeft.x, bottomLeft.y, 0), null);
        frontMap.SetTile(new Vector3Int(topRight.x, topRight.y, 0), null);

    }

    public void ResetCity()
    {
        cityMap = new CityBlock[width, width];
        frontMap.ClearAllTiles();
        backMap.ClearAllTiles();
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
