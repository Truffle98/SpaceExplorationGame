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
        cityMap = new CityBlock[width, width];
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
                    if (buildingTypeNum == 0)
                    {
                        buildingType = "block";
                    }
                    else if (buildingTypeNum == 1)
                    {
                        buildingType = "empty";
                    }

                    cityMap[i, j] = new CityBlock(new BoundsInt(new Vector3Int(bottomLeft.x + roadLength * (j + 1) + blockLength * j, bottomLeft.y + roadLength * (i + 1) + blockLength * i, 0),
                                    new Vector3Int(blockLength, blockLength, 0)), buildingWall, buildingType);
                }
                else
                {
                    cityMap[i, j] = new CityBlock(new BoundsInt(new Vector3Int(bottomLeft.x + roadLength * (j + 1) + blockLength * j, bottomLeft.y + roadLength * (i + 1) + blockLength * i, 0),
                                    new Vector3Int(blockLength, blockLength, 0)), buildingWall, "empty");
                }


            }

        }

        frontMap.SetTile(new Vector3Int(bottomLeft.x, bottomLeft.y, 0), buildingWall);
        frontMap.SetTile(new Vector3Int(topRight.x, topRight.y, 0), buildingWall);

        for (int i = 0; i < width; i++)
        {

            for (int j = 0; j < width; j++)
            {

                cityMap[i, j].DrawSelf(frontMap);

            }

        }

        frontMap.SetTile(new Vector3Int(bottomLeft.x, bottomLeft.y, 0), null);
        frontMap.SetTile(new Vector3Int(topRight.x, topRight.y, 0), null);

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


public class CityBlock
{

    private BoundsInt bounds;
    private TileBase tile;
    private string blockType;
    private List<Building> buildings = new List<Building>();

    public CityBlock(BoundsInt boundsTemp, TileBase tileTemp, string typeTemp)
    {
        bounds = boundsTemp;
        tile = tileTemp;
        blockType = typeTemp;
        SelfSetup();
    }

    public void DrawSelf(Tilemap map)
    {

        if (blockType == "block")
        {

        }
        else if (blockType == "empty")
        {
            
        }

        foreach (Building building in buildings)
        {
            building.DrawSelf(map);
        }

    }

    private void SelfSetup()
    {

        if (blockType == "block")
        {
            buildings.Add(new Building(new BoundsInt(new Vector3Int(bounds.min.x, bounds.min.y, 0), new Vector3Int(bounds.size.x, bounds.size.y, 0)), tile, "block"));
        }



    }

    



}

public class Building
{

    private BoundsInt bounds;
    private TileBase tile;
    private string buildingType;
    private List<Room> rooms = new List<Room>();

    public Building(BoundsInt boundsTemp, TileBase tileTemp, string typeTemp)
    {

        bounds = boundsTemp;
        tile = tileTemp;
        buildingType = typeTemp;
        rooms = SplitBuilding(bounds, 10, 10, 30, 30);

    }

    public void DrawSelf(Tilemap map)
    {
        map.BoxFill(Vector3Int.FloorToInt(bounds.center), tile, bounds.min.x, bounds.min.y, bounds.max.x, bounds.max.y);

        foreach (Room room in rooms)
        {
            room.DrawSelf(map);
        }
    }

    private List<Room> SplitBuilding(BoundsInt spaceToSplit, int minWidth, int minHeight, int maxWidth, int maxHeight)
    {

        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> boundsList = new List<BoundsInt>();
        List<Room> roomsList = new List<Room>();
        roomsQueue.Enqueue(spaceToSplit);
        while(roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if(room.size.y >= minHeight && room.size.x >= minWidth)
            {
                if (Random.value > 0.2f || room.size.y > maxHeight || room.size.x > maxWidth)
                {
                    if(Random.value < 0.5f)
                    {
                        if(room.size.y >= minHeight * 2)
                        {
                            SplitRoomHorizontally(minHeight, roomsQueue, room);
                        }else if(room.size.x >= minWidth * 2)
                        {
                            SplitRoomVertically(minWidth, roomsQueue, room);
                        }else
                        {
                            boundsList.Add(room);
                        }
                    }
                    else
                    {
                        if (room.size.x >= minWidth * 2)
                        {
                            SplitRoomVertically(minWidth, roomsQueue, room);
                        }
                        else if (room.size.y >= minHeight * 2)
                        {
                            SplitRoomHorizontally(minHeight, roomsQueue, room);
                        }
                        else
                        {
                            boundsList.Add(room);
                        }
                    }
                }
                else
                {
                    boundsList.Add(room);
                }
            }
        }

        foreach (BoundsInt bound in boundsList)
        {
            roomsList.Add(new Room(bound, "empty"));
        }

        return roomsList;
    }

    void SplitRoomVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(minWidth, room.size.x - minWidth);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
            new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    void SplitRoomHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(minHeight, room.size.y - minHeight);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
            new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

}

public class Room
{

    private BoundsInt bounds;
    private string roomType;

    public Room(BoundsInt boundsTemp, string typeTemp)
    {

        bounds = boundsTemp;
        roomType = typeTemp; 

    }

    public void DrawSelf(Tilemap map)
    {
        map.BoxFill(Vector3Int.FloorToInt(bounds.center), null, bounds.min.x + 1, bounds.min.y + 1, bounds.max.x - 1, bounds.max.y - 1);
    }

}