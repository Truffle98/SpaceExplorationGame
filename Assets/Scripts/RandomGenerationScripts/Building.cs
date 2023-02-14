using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Building
{

    private BoundsInt bounds;
    private TileBase tile;
    private string buildingType;
    private List<Room> rooms = new List<Room>();
    private List<Door> doors = new List<Door>();
    private Dictionary<string, RoomSetup> roomSetups;
    private GameObject buildingParent;

    public Building(BoundsInt boundsTemp, TileBase tileTemp, string typeTemp, GameObject parentTemp)
    {

        bounds = boundsTemp;
        tile = tileTemp;
        buildingType = typeTemp;
        buildingParent = parentTemp;
        
        TemplateReader reader = new TemplateReader();
        roomSetups = reader.ReadBuildingTemplate(buildingType);

    }

    public void DrawSelf(Tilemap frontMap, Tilemap backMap)
    {
        frontMap.BoxFill(Vector3Int.FloorToInt(bounds.center), tile, bounds.min.x, bounds.min.y, bounds.max.x, bounds.max.y);
        GenerateLayout();

        foreach (Room room in rooms)
        {
            room.DrawSelf(frontMap, backMap, doors, buildingParent);
        }

        foreach (Door door in doors)
        {
            door.DrawSelf(frontMap, backMap, buildingParent);
        }
    }

    //Generates the full room layout of the building
    private void GenerateLayout()
    {

        int startSide = Random.Range(0, 4);
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

        RoomSetup startRoom = roomSetups["testingStartRoom"];
        Room newRoom = GenerateRoomFromDoor(doors[0], startRoom);
        rooms.Add(newRoom);

        Queue<BranchingPoint> generationQueue = new Queue<BranchingPoint>();
        Queue<BranchingPoint> finalGenerationQueue = new Queue<BranchingPoint>();
        generationQueue.Enqueue(new BranchingPoint(newRoom.bounds, startRoom.GenerateRoomSetupQueue(roomSetups)));
        
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
                        generationQueue.Enqueue(new BranchingPoint(newRoom.bounds, newRoomToMake.GenerateRoomSetupQueue(roomSetups)));
                        finalGenerationQueue.Enqueue(new BranchingPoint(newRoom.bounds, newRoomToMake.GenerateSmallRoomSetupQueue(roomSetups)));
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

        List<Door> potentialExteriorDoors = new List<Door>();
        Door potentialDoor;

        foreach(Room room in rooms)
        {
            if (room.template.canAddDoors)
            {
                potentialDoor = GenerateExteriorDoor(room);
                if (potentialDoor != null)
                {
                    potentialExteriorDoors.Add(potentialDoor);
                }
            }
        }

        int maxExtraExteriorDoors = Mathf.Min(3, potentialExteriorDoors.Count);
        int randomIdx;
        for (int doorCount = 0; doorCount < maxExtraExteriorDoors; doorCount++)
        {
            randomIdx = Random.Range(0, potentialExteriorDoors.Count);
            doors.Add(potentialExteriorDoors[randomIdx]);
            potentialExteriorDoors.RemoveAt(randomIdx);
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
            doorAttempt = new Door(locationAttempt, 0, roomToMake.doorType);
            spaceAroundDoor = SpaceAroundDoor(doorAttempt);
            
            if (spaceAroundDoor.size.x >= roomToMake.minSize.x && spaceAroundDoor.size.y >= roomToMake.minSize.y)
            {
                potentialDoorLocations.Add(doorAttempt);
            }

        }

        for (int i = branchingPoint.bounds.min.x + 2; i <= branchingPoint.bounds.max.x - 2; i++)
        {

            locationAttempt = new Vector2Int(i, branchingPoint.bounds.max.y);
            doorAttempt = new Door(locationAttempt, 1, roomToMake.doorType);
            spaceAroundDoor = SpaceAroundDoor(doorAttempt);
            
            if (spaceAroundDoor.size.x >= roomToMake.minSize.x && spaceAroundDoor.size.y >= roomToMake.minSize.y)
            {
                potentialDoorLocations.Add(doorAttempt);
            }

        }

        for (int i = branchingPoint.bounds.min.y + 2; i <= branchingPoint.bounds.max.y - 2; i++)
        {

            locationAttempt = new Vector2Int(branchingPoint.bounds.min.x, i);
            doorAttempt = new Door(locationAttempt, 2, roomToMake.doorType);
            spaceAroundDoor = SpaceAroundDoor(doorAttempt);
            
            if (spaceAroundDoor.size.x >= roomToMake.minSize.x && spaceAroundDoor.size.y >= roomToMake.minSize.y)
            {
                potentialDoorLocations.Add(doorAttempt);
            }

        }

        for (int i = branchingPoint.bounds.min.x + 2; i <= branchingPoint.bounds.max.x - 2; i++)
        {

            locationAttempt = new Vector2Int(i, branchingPoint.bounds.min.y);
            doorAttempt = new Door(locationAttempt, 3, roomToMake.doorType);
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
                        potentialDoorLocations.Add(new Door(potentialLocation, 0, room1.template.doorType));
                    }
                }
            }
        }

        if (potentialDoorLocations.Count > 0)
        {
            return potentialDoorLocations[Random.Range(0, potentialDoorLocations.Count)];
        }

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
                        potentialDoorLocations.Add(new Door(potentialLocation, 1, room1.template.doorType));
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
                        potentialDoorLocations.Add(new Door(potentialLocation, 2, room1.template.doorType));
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

    Door GenerateExteriorDoor(Room room)
    {
        List<Door> potentialDoors = new List<Door>();
        Door potentialDoor;

        if (room.bounds.max.x >= bounds.max.x - 3)
        {
            potentialDoor = new Door(new Vector2Int(room.bounds.max.x, Random.Range(room.bounds.min.y + 1, room.bounds.max.y)), 0, "longSingle");
            potentialDoors.Add(potentialDoor);
        }

        if (room.bounds.max.y >= bounds.max.y - 3)
        {
            potentialDoor = new Door(new Vector2Int(Random.Range(room.bounds.min.x + 1, room.bounds.max.x), room.bounds.max.y), 1, "longSingle");
            potentialDoors.Add(potentialDoor);
        }

        if (room.bounds.min.x <= bounds.min.x + 3)
        {
            potentialDoor = new Door(new Vector2Int(room.bounds.min.x, Random.Range(room.bounds.min.y + 1, room.bounds.max.y)), 2, "longSingle");
            potentialDoors.Add(potentialDoor);
        }

        if (room.bounds.min.y <= bounds.min.y + 3)
        {
            potentialDoor = new Door(new Vector2Int(Random.Range(room.bounds.min.x + 1, room.bounds.max.x), room.bounds.min.y), 3, "longSingle");
            potentialDoors.Add(potentialDoor);
        }

        if (potentialDoors.Count > 0)
        {
            return potentialDoors[Random.Range(0, potentialDoors.Count)];
        }
        return null;
    }

    //Function to find the maximum room size around a door
    BoundsInt SpaceAroundDoor(Door door)
    {
        Vector3Int startLoc = new Vector3Int(door.location.x, door.location.y, 0);
        Vector3Int bottomLeft = startLoc, topRight = startLoc;
        float curDirection = door.orientationAngle;
        Vector3Int curChange = new Vector3Int((int)Mathf.Round(Mathf.Cos(curDirection)), (int)Mathf.Round(Mathf.Sin(curDirection)), 0);
        bool shouldBreak = false, oneSpace = false;
        BoundsInt testBounds;

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

    //Determines if room overlaps another
    bool RoomOverlaps(BoundsInt newRoom, Room existingRoom)
    {
        //This doesn't work for some reason ?
        /*
        if ((Mathf.Abs(newRoom.center.x - existingRoom.bounds.center.x) > newRoom.size.x + existingRoom.bounds.size.x) && (Mathf.Abs(newRoom.center.y - existingRoom.bounds.center.y) > newRoom.size.y + existingRoom.bounds.size.y))
        {
            return false;
        }
        */

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

    //Returns true if the room leaves the building or is too close to the edge
    bool RoomOutOfBounds(BoundsInt newRoom)
    {
        if (newRoom.min.x <= bounds.min.x || newRoom.min.y <= bounds.min.y || newRoom.max.x >= bounds.max.x || newRoom.max.y >= bounds.max.y)
        {
            return true;
        }
        return false;
    }

}