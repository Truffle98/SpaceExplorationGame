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

        cityMap[0,0].DrawSelf(frontMap);
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
            buildings.Add(new Building(new BoundsInt(new Vector3Int(bounds.min.x, bounds.min.y, -1), new Vector3Int(bounds.size.x, bounds.size.y, 2)), tile, "block"));
        }



    }

    



}

public class Building
{

    private BoundsInt bounds;
    private TileBase tile;
    private string buildingType;
    private List<Room> rooms = new List<Room>();
    private List<Door> doors = new List<Door>();

    public Building(BoundsInt boundsTemp, TileBase tileTemp, string typeTemp)
    {

        bounds = boundsTemp;
        tile = tileTemp;
        buildingType = typeTemp;
        //rooms = SplitBuilding(new BoundsInt(new Vector3Int(bounds.min.x + 1, bounds.min.y + 1, 0), new Vector3Int(bounds.size.x - 2, bounds.size.y - 2, 0)), 5, 5, 30, 30);

    }

    public void DrawSelf(Tilemap map)
    {
        map.BoxFill(Vector3Int.FloorToInt(bounds.center), tile, bounds.min.x, bounds.min.y, bounds.max.x, bounds.max.y);
        GenerateLayout();

        foreach (Room room in rooms)
        {
            room.DrawSelf(map);
        }

        foreach (Door door in doors)
        {
            door.DrawSelf(map);
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
                if (Random.value > 0.4f || room.size.y > maxHeight || room.size.x > maxWidth)
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
            //This got changed and isn't adding rooms
            //roomsList.Add(new Room(bound, "empty"));
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

    //Generates the full room layout of the building
    private void GenerateLayout()
    {

        int startSide = Random.Range(0, 4);
        //startSide = 2;
        float startSideAngle = startSide * (Mathf.PI / 2);
        Vector2Int startDoorLoc, startDoorOffset;

        startDoorOffset = new Vector2Int((int)Mathf.Round((Mathf.Abs(Mathf.Sin(startSideAngle))) * (Random.Range((int)Mathf.Floor(0.25f * bounds.size.x), (int)Mathf.Floor(0.75f * bounds.size.x)) - (bounds.size.x / 2))),
                                         (int)Mathf.Round((Mathf.Abs(Mathf.Cos(startSideAngle))) * (Random.Range((int)Mathf.Floor(0.25f * bounds.size.y), (int)Mathf.Floor(0.75f * bounds.size.y)) - (bounds.size.y / 2))));

        startDoorLoc = new Vector2Int((int)Mathf.Round(-Mathf.Cos(startSideAngle) * (bounds.size.x / 2f) + Mathf.Cos(startSideAngle) + bounds.center.x) + startDoorOffset.x,
                                      (int)Mathf.Round(-Mathf.Sin(startSideAngle) * (bounds.size.y / 2f) + Mathf.Sin(startSideAngle) + bounds.center.y) + startDoorOffset.y);

        //Debug.Log(startDoorOffset);
        //Debug.Log(startDoorLoc);

        doors.Add(new Door(startDoorLoc, startSide, "wideDouble"));

        Vector2Int minSize = new Vector2Int(8, 8), maxSize = new Vector2Int(15, 15);
        Vector2Int roomSize = new Vector2Int(Random.Range(minSize.x, maxSize.x + 1), Random.Range(minSize.y, maxSize.y + 1));
        BoundsInt roomLoc = new BoundsInt(new Vector3Int(0,0,0), new Vector3Int(0,0,0));
        List<int> potentialLocations = new List<int>();

        RoomSetup startRoom = new RoomSetup(new Vector2Int(8, 8), new Vector2Int(16, 16), "startRoom");
        Room newRoom = GenerateRoomFromDoor(doors[0], startRoom);
        rooms.Add(newRoom);

        Queue<BranchingPoint> generationQueue = new Queue<BranchingPoint>();
        Queue<BranchingPoint> finalGenerationQueue = new Queue<BranchingPoint>();
        generationQueue.Enqueue(new BranchingPoint(newRoom.bounds, startRoom.GenerateRoomSetupQueue(), startRoom.doorType));
        
        RoomSetup newRoomToMake;
        Door newDoor;
        BranchingPoint curBranchingPoint;
        bool endGeneration = false;

        while (generationQueue.Count > 0)
        {
            curBranchingPoint = generationQueue.Dequeue();
            while (curBranchingPoint.roomsToMake.Count > 0)
            {
                newRoomToMake = curBranchingPoint.roomsToMake.Dequeue();
                
                newDoor = GenerateDoorLocation(curBranchingPoint, newRoomToMake);
                if (newDoor != null)
                {
                    
                    newRoom = GenerateRoomFromDoor(newDoor, newRoomToMake);
                    if (newRoom != null)
                    {
                        doors.Add(newDoor);
                        rooms.Add(newRoom);
                        generationQueue.Enqueue(new BranchingPoint(newRoom.bounds, newRoomToMake.GenerateRoomSetupQueue(), newRoomToMake.doorType));
                        finalGenerationQueue.Enqueue(new BranchingPoint(newRoom.bounds, newRoomToMake.GenerateSmallRoomSetupQueue(), newRoomToMake.doorType));
                    }
                    else
                    {
                        Debug.Log("Error in room generation");
                        endGeneration = true;
                        break;
                    }
                }
                else
                {
                    //Debug.Log("Couldn't make room");
                    continue;
                }
            }
            if (endGeneration)
            {
                break;
            }
        }

        while (finalGenerationQueue.Count > 0)
        {
            curBranchingPoint = finalGenerationQueue.Dequeue();
            while (curBranchingPoint.roomsToMake.Count > 0)
            {
                newRoomToMake = curBranchingPoint.roomsToMake.Dequeue();
                
                newDoor = GenerateDoorLocation(curBranchingPoint, newRoomToMake);
                if (newDoor != null)
                {
                    
                    newRoom = GenerateRoomFromDoor(newDoor, newRoomToMake);
                    if (newRoom != null)
                    {
                        doors.Add(newDoor);
                        rooms.Add(newRoom);
                    }
                    else
                    {
                        Debug.Log("Error in room generation");
                        endGeneration = true;
                        break;
                    }
                }
                else
                {
                    //Debug.Log("Couldn't make room");
                    continue;
                }
            }
            if (endGeneration)
            {
                break;
            }
        }

        foreach(Room room1 in rooms)
        {

            if (!room1.template.canAddDoors)
            {
                continue;
            }

            foreach(Room room2 in rooms)
            {
                if (!room2.template.canAddDoors || room1 == room2)
                {
                    continue;
                }
                
                newDoor = GenerateExtraDoor(room1, room2);

                if (newDoor != null)
                {
                    doors.Add(newDoor);
                }

            }


        }

    }

    //Once known that the room will fit it will randomly choose a valid location to place the room
    Room GenerateRoomFromDoor(Door door, RoomSetup roomTemplate)
    {

        BoundsInt availableSpace = SpaceAroundDoor(door);
        Vector2Int roomSizeMax = new Vector2Int(Mathf.Min(roomTemplate.maxSize.x, availableSpace.size.x), Mathf.Min(roomTemplate.maxSize.y, availableSpace.size.y));
        Vector2Int roomSize = new Vector2Int(Random.Range(roomTemplate.minSize.x, roomSizeMax.x), Random.Range(roomTemplate.minSize.y, roomSizeMax.y));

        //roomSize = new Vector2Int(10,10);
        //Debug.Log("Size info:");
        //Debug.Log(availableSpace.size);
        //Debug.Log(availableSpace.min);
        //Debug.Log(availableSpace.max);
        //Debug.Log(roomTemplate.minSize);
        //Debug.Log(roomSizeMax);
        //Debug.Log(roomSize);

        List<Room> priorityRoomLocations = new List<Room>();
        List<Room> potentialRoomLocations = new List<Room>();
        float curAngle = door.orientationAngle + (Mathf.PI / 2), fillAngle = door.orientationAngle - (Mathf.PI / 4);
        Vector2Int startingPoint = new Vector2Int(0,0);
        if (door.doorType == "single")
        {
            startingPoint = new Vector2Int((int)Mathf.Round(door.location.x + Mathf.Cos(curAngle) * (Mathf.Floor(roomSize.x) - 1f)), 
                                           (int)Mathf.Round(door.location.y + Mathf.Sin(curAngle) * (Mathf.Floor(roomSize.y) - 1f)));
        }
        else if (door.doorType == "wideDouble")
        {
            //Really not sure why the x is -2 and y is -1 but it works
            startingPoint = new Vector2Int((int)Mathf.Round(door.location.x + Mathf.Cos(curAngle) * (Mathf.Floor(roomSize.x) - 2f)), 
                                           (int)Mathf.Round(door.location.y + Mathf.Sin(curAngle) * (Mathf.Floor(roomSize.y) - 1f)));
        }

        Vector2Int curChange = new Vector2Int((int)Mathf.Round(-Mathf.Cos(curAngle)), (int)Mathf.Round(-Mathf.Sin(curAngle)));
        BoundsInt testBounds;
        int timesRun = 0;
        bool isValidRoom, isBordering;
        
        while (true)
        {
            //Debug.Log(startingPoint);
            testBounds = new BoundsInt(new Vector3Int(startingPoint.x, startingPoint.y, 0), new Vector3Int((int)Mathf.Round(Mathf.Cos(fillAngle) / 0.707f * roomSize.x), 
                                                                    (int)Mathf.Round(Mathf.Sin(fillAngle) / 0.707f * roomSize.y), 0));
            isValidRoom = true;

            if (RoomOutOfBounds(testBounds))
            {
                isValidRoom = false;
            }

            if (isValidRoom)
            {
                foreach (Room room in rooms)
                {
                    if (RoomOverlaps(testBounds, room))
                    {
                        isValidRoom = false;
                        break;
                    }
                }
            }

            if (isValidRoom)
            {
                isBordering = false;
                if (AgainstBuildingWall(testBounds))
                {
                    isBordering = true;
                }
                else
                {
                    foreach(Room room in rooms)
                    {
                        if (RoomsIdealLocation(testBounds, room))
                        {
                            isBordering = true;
                            break;
                        }
                    }
                }

                if (isBordering)
                {
                    priorityRoomLocations.Add(new Room(testBounds, roomTemplate));
                }
                else
                {
                    potentialRoomLocations.Add(new Room(testBounds, roomTemplate));
                }
                
            }

            startingPoint += curChange;
            timesRun++;
            if (door.doorType == "single")
            {
                if (startingPoint == door.location)
                {
                    break;
                }
            }
            else if (door.doorType == "wideDouble")
            {
                //Not sure why I have to add the x value here but it works
                if (startingPoint + curChange == door.location + new Vector2Int(curChange.x, 0))
                {
                    break;
                }
            }

        }

        if (priorityRoomLocations.Count > 0)
        {
            return priorityRoomLocations[Random.Range(0, priorityRoomLocations.Count)];
        }
        else if (potentialRoomLocations.Count > 0)
        {
            return potentialRoomLocations[Random.Range(0, potentialRoomLocations.Count)];
        }
        else
        {
            return null;
        }

    }

    //Finds a valid place to put the door knowing the room will fit after
    Door GenerateDoorLocation(BranchingPoint branchingPoint, RoomSetup roomToMake)
    {

        BoundsInt spaceAroundDoor;
        Vector2Int locationAttempt;
        Door doorAttempt;
        List<Door> potentialDoorLocations = new List<Door>();

        for (int i = branchingPoint.bounds.min.y + 2; i <= branchingPoint.bounds.max.y - 2; i++)
        {

            locationAttempt = new Vector2Int(branchingPoint.bounds.max.x, i);
            doorAttempt = new Door(locationAttempt, 0, branchingPoint.doorType);
            spaceAroundDoor = SpaceAroundDoor(doorAttempt);
            
            if (spaceAroundDoor.size.x >= roomToMake.minSize.x && spaceAroundDoor.size.y >= roomToMake.minSize.y)
            {
                potentialDoorLocations.Add(doorAttempt);
            }

        }

        for (int i = branchingPoint.bounds.min.x + 2; i <= branchingPoint.bounds.max.x - 2; i++)
        {

            locationAttempt = new Vector2Int(i, branchingPoint.bounds.max.y);
            doorAttempt = new Door(locationAttempt, 1, branchingPoint.doorType);
            spaceAroundDoor = SpaceAroundDoor(doorAttempt);
            
            if (spaceAroundDoor.size.x >= roomToMake.minSize.x && spaceAroundDoor.size.y >= roomToMake.minSize.y)
            {
                potentialDoorLocations.Add(doorAttempt);
            }

        }

        for (int i = branchingPoint.bounds.min.y + 2; i <= branchingPoint.bounds.max.y - 2; i++)
        {

            locationAttempt = new Vector2Int(branchingPoint.bounds.min.x, i);
            doorAttempt = new Door(locationAttempt, 2, branchingPoint.doorType);
            spaceAroundDoor = SpaceAroundDoor(doorAttempt);
            
            if (spaceAroundDoor.size.x >= roomToMake.minSize.x && spaceAroundDoor.size.y >= roomToMake.minSize.y)
            {
                potentialDoorLocations.Add(doorAttempt);
            }

        }

        for (int i = branchingPoint.bounds.min.x + 2; i <= branchingPoint.bounds.max.x - 2; i++)
        {

            locationAttempt = new Vector2Int(i, branchingPoint.bounds.min.y);
            doorAttempt = new Door(locationAttempt, 3, branchingPoint.doorType);
            spaceAroundDoor = SpaceAroundDoor(doorAttempt);
            
            if (spaceAroundDoor.size.x >= roomToMake.minSize.x && spaceAroundDoor.size.y >= roomToMake.minSize.y)
            {
                potentialDoorLocations.Add(doorAttempt);
            }

        }

        if (potentialDoorLocations.Count > 0)
        {
            return potentialDoorLocations[Random.Range(0, potentialDoorLocations.Count)];
        }
        else
        {
            return null;
        }
        
    }

    Door GenerateExtraDoor(Room room1, Room room2)
    {

        List<Door> potentialDoorLocations = new List<Door>();
        Vector2Int potentialLocation;
        bool isValidDoor;

        if (room1.bounds.max.y == room2.bounds.min.y)
        {
            for (int i = room1.bounds.min.x + 1; i < room1.bounds.max.x; i++)
            {
                if (room2.bounds.min.x < i && i < room2.bounds.max.x)
                {

                    potentialLocation = new Vector2Int(i, room1.bounds.max.y);
                    isValidDoor = true;
                    foreach (Door door in doors)
                    {
                        if (Mathf.Abs(door.location.x - potentialLocation.x) < 4 && Mathf.Abs(door.location.y - potentialLocation.y) < 4)
                        {
                            isValidDoor = false;
                            break;
                        }
                    }
                    if (isValidDoor)
                    {
                        potentialDoorLocations.Add(new Door(potentialLocation, 0, room1.template.doorType));
                    }
                }
            }
        }

        if (potentialDoorLocations.Count > 0)
        {
            return potentialDoorLocations[Random.Range(0, potentialDoorLocations.Count)];
        }
        
        if (room1.bounds.max.x == room2.bounds.min.x)
        {
            for (int i = room1.bounds.min.y + 1; i < room1.bounds.max.y; i++)
            {
                if (room2.bounds.min.y < i && i < room2.bounds.max.y)
                {
                    potentialLocation = new Vector2Int(room1.bounds.max.x, i);
                    isValidDoor = true;
                    foreach (Door door in doors)
                    {
                        if (Mathf.Abs(door.location.x - potentialLocation.x) < 4 && Mathf.Abs(door.location.y - potentialLocation.y) < 4)
                        {
                            isValidDoor = false;
                            break;
                        }
                    }
                    if (isValidDoor)
                    {
                        potentialDoorLocations.Add(new Door(potentialLocation, 1, room1.template.doorType));
                    }
                }
            }
        }

        if (potentialDoorLocations.Count > 0)
        {
            return potentialDoorLocations[Random.Range(0, potentialDoorLocations.Count)];
        }

        if (room1.bounds.min.y == room2.bounds.max.y)
        {
            for (int i = room1.bounds.min.x + 1; i < room1.bounds.max.x; i++)
            {
                if (room2.bounds.min.x < i && i < room2.bounds.max.x)
                {
                    potentialLocation = new Vector2Int(i, room1.bounds.min.y);
                    isValidDoor = true;
                    foreach (Door door in doors)
                    {
                        if (Mathf.Abs(door.location.x - potentialLocation.x) < 4 && Mathf.Abs(door.location.y - potentialLocation.y) < 4)
                        {
                            isValidDoor = false;
                            break;
                        }
                    }
                    if (isValidDoor)
                    {
                        potentialDoorLocations.Add(new Door(potentialLocation, 2, room1.template.doorType));
                    }
                }
            }
        }

        if (potentialDoorLocations.Count > 0)
        {
            return potentialDoorLocations[Random.Range(0, potentialDoorLocations.Count)];
        }

        if (room1.bounds.min.x == room2.bounds.max.x)
        {
            for (int i = room1.bounds.min.y + 1; i < room1.bounds.max.y; i++)
            {
                if (room2.bounds.min.y < i && i < room2.bounds.max.y)
                {
                    potentialLocation = new Vector2Int(room1.bounds.min.x, i);
                    isValidDoor = true;
                    foreach (Door door in doors)
                    {
                        if (Mathf.Abs(door.location.x - potentialLocation.x) < 4 && Mathf.Abs(door.location.y - potentialLocation.y) < 4)
                        {
                            isValidDoor = false;
                            break;
                        }
                    }
                    if (isValidDoor)
                    {
                        potentialDoorLocations.Add(new Door(potentialLocation, 3, room1.template.doorType));
                    }
                }
            }
        }

        if (potentialDoorLocations.Count > 0)
        {
            return potentialDoorLocations[Random.Range(0, potentialDoorLocations.Count)];
        }

        return null;

    }

    //Determines how big of a room can come from the door (has issues)
    /*
    BoundsInt AltSpaceAroundDoor(Door door)
    {

        Vector2Int startLoc = new Vector2Int((int)Mathf.Round(door.location.x + Mathf.Cos(door.orientationAngle) * 2), (int)Mathf.Round(door.location.y + Mathf.Sin(door.orientationAngle) * 2));
        //Debug.Log("Start loc:");
        //Debug.Log(startLoc);
        bool rightBoundFound = false, topBoundFound = false, leftBoundFound = false, bottomBoundFound = false;
        int rightDistanceAvailable = 0, topDistanceAvailable = 0, leftDistanceAvailable = 0, bottomDistanceAvailable = 0;

        while (!rightBoundFound || !topBoundFound || !leftBoundFound || !bottomBoundFound)
        {

            if (!rightBoundFound)
            {
                rightDistanceAvailable++;

                if (startLoc.x + rightDistanceAvailable >= bounds.max.x - 1)
                {
                    rightDistanceAvailable--;

                    rightBoundFound = true;
                }
                else
                {
                    foreach (Room room in rooms)
                    {
                        if (PointInRoom(new Vector2Int(startLoc.x + rightDistanceAvailable, startLoc.y), room.bounds))
                        {
                            rightBoundFound = true;
                            break;
                        }
                    }
                }

            }

            if (!topBoundFound)
            {
                topDistanceAvailable++;

                if (startLoc.y + topDistanceAvailable >= bounds.max.y - 1)
                {
                    topDistanceAvailable--;

                    topBoundFound = true;
                }
                else
                {
                    foreach (Room room in rooms)
                    {
                        if (PointInRoom(new Vector2Int(startLoc.x + topDistanceAvailable, startLoc.y), room.bounds))
                        {
                            topBoundFound = true;
                            break;
                        }
                    }
                }


            }

            if (!leftBoundFound)
            {
                leftDistanceAvailable++;

                if (startLoc.x - leftDistanceAvailable <= bounds.min.x + 1)
                {
                    leftDistanceAvailable--;

                    leftBoundFound = true;
                }
                else
                {
                    foreach (Room room in rooms)
                    {
                        if (PointInRoom(new Vector2Int(startLoc.x - leftDistanceAvailable, startLoc.y), room.bounds))
                        {
                            leftBoundFound = true;
                            break;
                        }
                    }
                }

            }

            if (!bottomBoundFound)
            {
                bottomDistanceAvailable++;

                if (startLoc.y - bottomDistanceAvailable <= bounds.min.y + 1)
                {
                    bottomDistanceAvailable--;

                    bottomBoundFound = true;
                }
                else
                {
                    foreach (Room room in rooms)
                    {
                        if (PointInRoom(new Vector2Int(startLoc.x - bottomDistanceAvailable, startLoc.y), room.bounds))
                        {
                            bottomBoundFound = true;
                            break;
                        }
                    }
                }

            }

        }

        return new BoundsInt(new Vector3Int(startLoc.x - leftDistanceAvailable, startLoc.y - bottomDistanceAvailable, 0), new Vector3Int(leftDistanceAvailable + rightDistanceAvailable + 1, bottomDistanceAvailable + topDistanceAvailable + 1, 0));

    }
    */

    //Alternative way to find the space around a door (probably better ?)
    BoundsInt SpaceAroundDoor(Door door)
    {
        //Vector3Int startLoc = new Vector3Int((int)Mathf.Round(door.location.x + Mathf.Cos(door.orientationAngle) * 2), (int)Mathf.Round(door.location.y + Mathf.Sin(door.orientationAngle) * 2), 0);
        Vector3Int startLoc = new Vector3Int(door.location.x, door.location.y, 0);
        Vector3Int bottomLeft = startLoc, topRight = startLoc;
        float curDirection = door.orientationAngle;
        Vector3Int curChange = new Vector3Int((int)Mathf.Round(Mathf.Cos(curDirection)), (int)Mathf.Round(Mathf.Sin(curDirection)), 0);
        bool shouldBreak = false, oneSpace = false;
        BoundsInt testBounds;

        /*
        foreach (Room room in rooms)
        {
            Debug.Log("Check location: " + new Vector2Int((int)Mathf.Round(startLoc.x + Mathf.Cos(door.orientationAngle) * 2f), (int)Mathf.Round(startLoc.y + Mathf.Sin(door.orientationAngle) * 2f)));
            if (PointInRoom(new Vector2Int((int)Mathf.Round(startLoc.x + Mathf.Cos(door.orientationAngle) * 2f), (int)Mathf.Round(startLoc.y + Mathf.Sin(door.orientationAngle) * 2f)), room))
            {
                return new BoundsInt(startLoc, new Vector3Int(0,0,0));
            }
        }
        */

        while (true)
        {
            if (curChange.x > 0 || curChange.y > 0)
            {
                topRight += curChange;
            }
            else
            {
                bottomLeft += curChange;
            }

            testBounds = new BoundsInt(new Vector3Int(bottomLeft.x, bottomLeft.y, 0), new Vector3Int(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y, 0));
            if (RoomOutOfBounds(testBounds))
            {
                if (curChange.x > 0 || curChange.y > 0)
                {
                    topRight -= curChange;
                }
                else
                {
                    bottomLeft -= curChange;
                }
                break;
            }

            foreach (Room room in rooms)
            {
                if (RoomOverlaps(testBounds, room))
                {
                    if (curChange.x > 0 || curChange.y > 0)
                    {
                        topRight -= curChange;
                    }
                    else
                    {
                        bottomLeft -= curChange;
                    }
                    
                    shouldBreak = true;
                    break;
                }
            }

            if (shouldBreak)
            {
                break;
            }
            oneSpace = true;
        }

        if (!oneSpace)
        {
            return new BoundsInt(startLoc, new Vector3Int(0,0,0));
        }

        curDirection += Mathf.PI / 2f;
        curChange = new Vector3Int((int)Mathf.Round(Mathf.Cos(curDirection)), (int)Mathf.Round(Mathf.Sin(curDirection)), 0);
        shouldBreak = false;
        oneSpace = false;

        while (true)
        {
            if (curChange.x > 0 || curChange.y > 0)
            {
                topRight += curChange;
            }
            else
            {
                bottomLeft += curChange;
            }

            testBounds = new BoundsInt(new Vector3Int(bottomLeft.x, bottomLeft.y, 0), new Vector3Int(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y, 0));
            if (RoomOutOfBounds(testBounds))
            {

                if (curChange.x > 0 || curChange.y > 0)
                {
                    topRight -= curChange;
                }
                else
                {
                    bottomLeft -= curChange;
                }
                break;
            }

            foreach (Room room in rooms)
            {
                if (RoomOverlaps(testBounds, room))
                {
                    if (curChange.x > 0 || curChange.y > 0)
                    {
                        topRight.x -= curChange.x;
                        topRight.y -= curChange.y;
                    }
                    else
                    {
                        bottomLeft.x -= curChange.x;
                        bottomLeft.y -= curChange.y;
                    }
                    
                    shouldBreak = true;
                    break;
                }
            }

            if (shouldBreak)
            {
                break;
            }
            oneSpace = true;
        }

        if (!oneSpace)
        {
            return new BoundsInt(startLoc, new Vector3Int(0,0,0));
        }

        curDirection += Mathf.PI;
        curChange = new Vector3Int((int)Mathf.Round(Mathf.Cos(curDirection)), (int)Mathf.Round(Mathf.Sin(curDirection)), 0);
        shouldBreak = false;
        oneSpace = false;

        while (true)
        {

            if (curChange.x > 0 || curChange.y > 0)
            {
                topRight += curChange;
            }
            else
            {
                bottomLeft += curChange;
            }

            testBounds = new BoundsInt(new Vector3Int(bottomLeft.x, bottomLeft.y, 0), new Vector3Int(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y, 0));
            if (RoomOutOfBounds(testBounds))
            {
                if (curChange.x > 0 || curChange.y > 0)
                {
                    topRight -= curChange;
                }
                else
                {
                    bottomLeft -= curChange;
                }
                break;
            }

            foreach (Room room in rooms)
            {
                if (RoomOverlaps(testBounds, room))
                {
                    if (curChange.x > 0 || curChange.y > 0)
                    {
                        topRight -= curChange;
                    }
                    else
                    {
                        bottomLeft -= curChange;
                    }
                    
                    shouldBreak = true;
                    break;
                }
            }

            if (shouldBreak)
            {
                break;
            }
            oneSpace = true;
        }

        if (!oneSpace)
        {
            return new BoundsInt(startLoc, new Vector3Int(0,0,0));
        }

        //Debug.Log(bottomLeft);
        //Debug.Log(topRight);
        return new BoundsInt(new Vector3Int(bottomLeft.x + 1, bottomLeft.y + 1, 0), new Vector3Int(topRight.x - bottomLeft.x - 1, topRight.y - bottomLeft.y - 1, 0));

    }

    //Determines if inside of a room hit anothers wall
    bool RoomInsideOverlaps(BoundsInt newRoom, Room existingRoom)
    {
        //This doesn't work for some reason ?
        if ((Mathf.Abs(newRoom.center.x - existingRoom.bounds.center.x) > newRoom.size.x + existingRoom.bounds.size.x) && (Mathf.Abs(newRoom.center.y - existingRoom.bounds.center.y) > newRoom.size.y + existingRoom.bounds.size.y))
        {
            //return false;
        }

        for (int i = newRoom.min.x; i <= newRoom.max.x; i++)
        {
            for (int j = newRoom.min.y; j <= newRoom.max.y; j++)
            {

                if ((existingRoom.bounds.min.x <= i && i <= existingRoom.bounds.max.x) && (existingRoom.bounds.min.y <= j && j <= existingRoom.bounds.max.y))
                {
                    return true;
                }

            }
        }
        return false;
    }

    //Determines if room overlaps another
    bool RoomOverlaps(BoundsInt newRoom, Room existingRoom)
    {
        //This doesn't work for some reason ?
        if ((Mathf.Abs(newRoom.center.x - existingRoom.bounds.center.x) > newRoom.size.x + existingRoom.bounds.size.x) && (Mathf.Abs(newRoom.center.y - existingRoom.bounds.center.y) > newRoom.size.y + existingRoom.bounds.size.y))
        {
            //return false;
        }

        for (int i = newRoom.min.x; i <= newRoom.max.x; i++)
        {
            for (int j = newRoom.min.y; j <= newRoom.max.y; j++)
            {

                if ((existingRoom.bounds.min.x < i && i < existingRoom.bounds.max.x) && (existingRoom.bounds.min.y < j && j < existingRoom.bounds.max.y))
                {
                    return true;
                }

            }
        }
        return false;
    }

    //Determines if a room is next to anothers wall, makes the room more mesh with one another
    bool RoomsIdealLocation(BoundsInt newRoom, Room existingRoom)
    {

        //This doesn't work for some reason ?
        if ((Mathf.Abs(newRoom.center.x - existingRoom.bounds.center.x) > newRoom.size.x + existingRoom.bounds.size.x) && (Mathf.Abs(newRoom.center.y - existingRoom.bounds.center.y) > newRoom.size.y + existingRoom.bounds.size.y))
        {
            //return false;
        }

        for (int i = newRoom.min.x; i <= newRoom.max.x; i++)
        {
            for (int j = newRoom.min.y; j <= newRoom.max.y; j++)
            {

                if ((existingRoom.bounds.min.x <= i && i <= existingRoom.bounds.max.x) && (existingRoom.bounds.min.y <= j && j <= existingRoom.bounds.max.y))
                {
                    return true;
                }

            }
        }

        return false;

    }

    //Determines if room touches building walls
    bool AgainstBuildingWall(BoundsInt newRoom)
    {

        if (newRoom.min.x == bounds.min.x + 1|| newRoom.min.y == bounds.min.y + 1|| newRoom.max.x == bounds.max.x - 1|| newRoom.max.y == bounds.max.y - 1)
        {
            return true;
        }
        return false;
    }

    //For finding out why rooms go over each other
    bool RoomInsideOverlapsInside(Room room1, Room room2)
    {
        //This doesn't work for some reason ?
        if ((Mathf.Abs(room1.bounds.center.x - room2.bounds.center.x) > room1.bounds.size.x + room2.bounds.size.x) && (Mathf.Abs(room1.bounds.center.y - room2.bounds.center.y) > room1.bounds.size.y + room2.bounds.size.y))
        {
            //return false;
        }

        for (int i = room1.bounds.min.x + 1; i <= room1.bounds.max.x - 1; i++)
        {
            for (int j = room1.bounds.min.y + 1; j <= room1.bounds.max.y - 1; j++)
            {

                if ((room2.bounds.min.x <= i && i <= room2.bounds.max.x) && (room2.bounds.min.y <= j && j <= room2.bounds.max.y))
                {
                    return true;
                }

            }
        }
        return false;
    }

    //Returns true if the room leaves the building or is too close to the edge
    bool RoomOutOfBounds(BoundsInt newRoom)
    {
        if (newRoom.min.x <= bounds.min.x || newRoom.min.y <= bounds.min.y || newRoom.max.x >= bounds.max.x || newRoom.max.y >= bounds.max.y)
        {
            return true;
        }
        return false;
    }

    //Determines if a point is in a room
    bool PointInRoom(Vector2Int point, Room room)
    {
        if ((room.bounds.min.x < point.x && point.x < room.bounds.max.x) && (room.bounds.min.y < point.y && point.y < room.bounds.max.y))
        {
            return true;
        }
        return false;
    }


}

//Holds info about a room in a building
public class Room
{

    public BoundsInt bounds;
    public RoomSetup template; 
    public string roomType;

    public Room(BoundsInt boundsTemp, RoomSetup roomTemplate)
    {

        bounds = boundsTemp;
        template = roomTemplate;
        roomType = roomTemplate.roomType;

    }

    public void DrawSelf(Tilemap map)
    {
        //Debug.Log(bounds.center);
        //Debug.Log(bounds.min);
        //Debug.Log(bounds.max);
        map.BoxFill(Vector3Int.FloorToInt(bounds.center), null, bounds.min.x + 1, bounds.min.y + 1, bounds.max.x - 1, bounds.max.y - 1);
    }

}

//Holds info about a door in a building
public class Door
{

    public Vector2Int location;
    public string doorType;
    public int orientation;
    public float orientationAngle;

    public Door(Vector2Int locationTemp, int orientationTemp, string typeTemp)
    {

        location = locationTemp;
        orientation = orientationTemp;
        orientationAngle = orientation * (Mathf.PI / 2f);
        doorType = typeTemp;

    }

    public void DrawSelf(Tilemap map)
    {
        
        Vector2Int cornerOne = new Vector2Int(0,0), cornerTwo = new Vector2Int(0,0);

        if (doorType == "single")
        {

            cornerOne = new Vector2Int(location.x, location.y);
            cornerTwo = new Vector2Int(location.x, location.y);

        }

        else if (doorType == "wideDouble")
        {
            cornerOne = new Vector2Int(location.x, location.y);
            cornerTwo = new Vector2Int((int)Mathf.Round(location.x + Mathf.Cos(orientationAngle - Mathf.PI * 0.75f) * Mathf.Sqrt(2f)), (int)Mathf.Round(location.y + Mathf.Sin(orientationAngle - Mathf.PI / 0.75f) * Mathf.Sqrt(2f)));
        
        }

        Vector2Int bottomLeft = new Vector2Int(Mathf.Min(cornerOne.x, cornerTwo.x), Mathf.Min(cornerOne.y, cornerTwo.y)), topRight = new Vector2Int(Mathf.Max(cornerOne.x, cornerTwo.x), Mathf.Max(cornerOne.y, cornerTwo.y));

        for (int i = bottomLeft.x; i <= topRight.x; i++)
        {
            for (int j = bottomLeft.y; j <= topRight.y; j++)
            {
                map.SetTile(new Vector3Int(i, j, 0), null);
            }
        }

    }

}

//Holds info about a room to be made
public class RoomSetup
{

    public Vector2Int minSize, maxSize;
    public string roomType, doorType;
    public bool canAddDoors;

    public RoomSetup(Vector2Int minSizeTemp, Vector2Int maxSizeTemp, string typeTemp, bool canAddDoorsTemp = true)
    {

        minSize = minSizeTemp;
        maxSize = maxSizeTemp;
        roomType = typeTemp;
        canAddDoors = canAddDoorsTemp;

        if (roomType == "startRoom")
        {
            doorType = "single";
        }
        else if (roomType == "testRoom")
        {
            doorType = "single";
        }

    }

    public Queue<RoomSetup> GenerateRoomSetupQueue()
    {

        Queue<RoomSetup> roomQueue = new Queue<RoomSetup>();
        RoomSetup testRoom = new RoomSetup(new Vector2Int(5, 5), new Vector2Int(10, 10), "testRoom");

        if (roomType == "startRoom")
        {
            roomQueue.Enqueue(testRoom);
            roomQueue.Enqueue(testRoom);
        }
        else if (roomType == "testRoom")
        {
            roomQueue.Enqueue(testRoom);
            roomQueue.Enqueue(testRoom);
            roomQueue.Enqueue(testRoom);
        }

        return roomQueue;

    }

    public Queue<RoomSetup> GenerateSmallRoomSetupQueue()
    {

        Queue<RoomSetup> roomQueue = new Queue<RoomSetup>();
        RoomSetup smallTestRoom = new RoomSetup(new Vector2Int(4, 4), new Vector2Int(6, 6), "smallTestRoom");

        if (roomType == "testRoom")
        {
            roomQueue.Enqueue(smallTestRoom);
        }

        return roomQueue;

    }

}

//Holds info about rooms that should branch from the current
public class BranchingPoint
{

    public BoundsInt bounds;
    public Queue<RoomSetup> roomsToMake;
    public string doorType;

    public BranchingPoint(BoundsInt boundsTemp, Queue<RoomSetup> roomsToMakeTemp, string doorTypeTemp)
    {

        bounds = boundsTemp;
        roomsToMake = roomsToMakeTemp;
        doorType = doorTypeTemp;

    }

}