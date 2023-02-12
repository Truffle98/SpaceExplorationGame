using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Holds info about a room in a building
public class Room
{

    public BoundsInt bounds;
    public RoomSetup template; 
    public string roomType;
    private List<GameObject> roomObjects;

    public Room(BoundsInt boundsTemp, RoomSetup roomTemplate)
    {

        bounds = boundsTemp;
        template = roomTemplate;
        roomType = roomTemplate.roomType;

    }

    public void DrawSelf(Tilemap frontMap, Tilemap backMap, List<Door> doors, GameObject buildingParent)
    {
        //Debug.Log(bounds.center);
        //Debug.Log(bounds.min);
        //Debug.Log(bounds.max);
        frontMap.BoxFill(Vector3Int.FloorToInt(bounds.center), null, bounds.min.x + 1, bounds.min.y + 1, bounds.max.x - 1, bounds.max.y - 1);
        TileBase floor = Resources.Load<TileBase>("RoomAssets/Floors/" + template.floorType);
        backMap.BoxFill(Vector3Int.FloorToInt(bounds.center), floor, bounds.min.x, bounds.min.y, bounds.max.x, bounds.max.y);

        GenerateFurniture(doors, buildingParent);
    }

    public void GenerateFurniture(List<Door> doors, GameObject buildingParent)
    {
        List<GameObject> roomObjects = new List<GameObject>();
        GameObject curObject;
        RoomObjectScript curObjectScript;
        Vector2Int startCorner, endCorner, cornerOffsetDirection, offset, offsetDistance;
        int xLength, yLength, sideLength;
        float orientationAngle, orientationOffset, halfPIAdj;
        List<Vector2Int> potentialStartCorners, potentialEndCorners;
        List<float> potentialOrientations;
        List<int> potentialIdx, priorityIdx;
        int chosenIdx;
        GameObject newObject;

        for (int i = 0; i < template.roomObjects.Length; i++)
        {
            curObject = Resources.Load<GameObject>("RoomAssets/RoomObjects/" + template.roomObjects[i]);
            curObjectScript = curObject.GetComponent<RoomObjectScript>();
            
            for (int j = 0; j < template.roomObjectsCount[i]; j++)
            {
                if (Random.Range(0, 100) < template.roomObjectsProbabilities[i])
                {
                    if (curObjectScript.roomPositioning == "corner")
                    {
                        
                    }
                    else if (curObjectScript.roomPositioning == "wall")
                    {

                        orientationAngle = 0;
                        startCorner = new Vector2Int(bounds.max.x - 1, bounds.min.y + 1);
                        potentialStartCorners = new List<Vector2Int>();
                        potentialEndCorners =  new List<Vector2Int>();
                        potentialOrientations = new List<float>();
                        potentialIdx = new List<int>();
                        priorityIdx = new List<int>();

                        for (int k = 0; k < 4; k++)
                        {
                            xLength = (int)Mathf.Round(Mathf.Abs(Mathf.Cos(orientationAngle) * curObjectScript.size.x + Mathf.Sin(orientationAngle) * curObjectScript.size.y));
                            yLength = (int)Mathf.Round(Mathf.Abs(Mathf.Cos(orientationAngle) * curObjectScript.size.y + Mathf.Sin(orientationAngle) * curObjectScript.size.x));
                            orientationOffset = orientationAngle + (Mathf.PI * 5/4);
                            cornerOffsetDirection = new Vector2Int((int)Mathf.Round(Mathf.Cos(orientationOffset) / 0.707f), (int)Mathf.Round(Mathf.Sin(orientationOffset) / 0.707f));

                            endCorner = new Vector2Int(startCorner.x + (xLength - 1) * cornerOffsetDirection.x, startCorner.y + (yLength - 1) * cornerOffsetDirection.y);
                            halfPIAdj = orientationAngle + (Mathf.PI / 2);
                            sideLength = (int)Mathf.Round(Mathf.Abs(bounds.size.x * Mathf.Cos(halfPIAdj)) + Mathf.Abs(bounds.size.y * Mathf.Sin(halfPIAdj)));
                            offset = new Vector2Int((int)Mathf.Round(Mathf.Cos(halfPIAdj)), (int)Mathf.Round(Mathf.Sin(halfPIAdj)));
                            //Debug.Log("Start corner: " + startCorner);
                            //Debug.Log("End corner: " + endCorner);

                            for (int l = 0; l < sideLength - 2; l++)
                            {
                                if (CheckObjectLocationValid(startCorner, endCorner, curObjectScript, doors, roomObjects))
                                {
                                    if (CheckObjectLocationPriority(startCorner, endCorner, curObjectScript, roomObjects))
                                    {
                                        priorityIdx.Add(potentialStartCorners.Count);
                                    }
                                    else
                                    {
                                        potentialIdx.Add(potentialStartCorners.Count);
                                    }

                                    potentialStartCorners.Add(startCorner);
                                    potentialEndCorners.Add(endCorner);
                                    potentialOrientations.Add(orientationAngle);
                                }

                                startCorner += offset;
                                endCorner += offset;

                            }
                            orientationAngle += (Mathf.PI / 2);
                        }

                        chosenIdx = -1;
                        if (priorityIdx.Count > 0)
                        {
                            chosenIdx = priorityIdx[Random.Range(0, priorityIdx.Count)];
                        }
                        else if (potentialIdx.Count > 0)
                        {
                            chosenIdx = potentialIdx[Random.Range(0, potentialIdx.Count)];
                        }

                        if (chosenIdx != -1)
                        {
                            startCorner = potentialStartCorners[chosenIdx];
                            endCorner = potentialEndCorners[chosenIdx];
                            orientationAngle = potentialOrientations[chosenIdx] * Mathf.Rad2Deg;
                            newObject = GameObject.Instantiate(curObject, new Vector3(startCorner.x + 0.5f, startCorner.y + 0.5f, 0), Quaternion.Euler(0,0, orientationAngle), buildingParent.transform);
                            newObject.GetComponent<RoomObjectScript>().AcceptBoundaries(startCorner, endCorner, potentialOrientations[chosenIdx]);
                            if (Mathf.Abs(Random.Range(0, 100) * Mathf.Cos(potentialOrientations[chosenIdx])) > 50)
                            {
                                newObject.GetComponent<RoomObjectScript>().FlipXAxis();
                            }
                            else if (Mathf.Abs(Random.Range(0, 100) * Mathf.Sin(potentialOrientations[chosenIdx])) > 50)
                            {
                                newObject.GetComponent<RoomObjectScript>().FlipYAxis();
                            }

                            roomObjects.Add(newObject);
                        }

                    }
                    else if (curObjectScript.roomPositioning == "center")
                    {

                        orientationAngle = 0;
                        potentialStartCorners = new List<Vector2Int>();
                        potentialEndCorners =  new List<Vector2Int>();
                        potentialOrientations = new List<float>();
                        potentialIdx = new List<int>();
                        priorityIdx = new List<int>();

                        xLength = (int)Mathf.Round(Mathf.Abs(Mathf.Cos(orientationAngle) * curObjectScript.size.x + Mathf.Sin(orientationAngle) * curObjectScript.size.y));
                        yLength = (int)Mathf.Round(Mathf.Abs(Mathf.Cos(orientationAngle) * curObjectScript.size.y + Mathf.Sin(orientationAngle) * curObjectScript.size.x));    

                        orientationOffset = orientationAngle + (Mathf.PI * 5/4);
                        cornerOffsetDirection = new Vector2Int((int)Mathf.Round(Mathf.Cos(orientationOffset) / 0.707f), (int)Mathf.Round(Mathf.Sin(orientationOffset) / 0.707f));
                        offsetDistance = new Vector2Int((xLength - 1) * cornerOffsetDirection.x, (yLength - 1) * cornerOffsetDirection.y);

                        for (int x = bounds.min.x + 1; x < bounds.max.x - 1; x++)
                        {
                            for (int y = bounds.min.y + 1; y < bounds.max.y - 1; y++)
                            {
                                startCorner = new Vector2Int(x, y);
                                endCorner = startCorner + offsetDistance;

                                if (CheckObjectLocationValid(startCorner, endCorner, curObjectScript, doors, roomObjects))
                                {
                                    if (CheckObjectLocationPriority(startCorner, endCorner, curObjectScript, roomObjects))
                                    {
                                        priorityIdx.Add(potentialStartCorners.Count);
                                    }
                                    else
                                    {
                                        potentialIdx.Add(potentialStartCorners.Count);
                                    }

                                    potentialStartCorners.Add(startCorner);
                                    potentialEndCorners.Add(endCorner);
                                    potentialOrientations.Add(orientationAngle);
                                }
                            }
                        }

                        orientationAngle += (Mathf.PI / 2);
                        xLength = (int)Mathf.Round(Mathf.Abs(Mathf.Cos(orientationAngle) * curObjectScript.size.x + Mathf.Sin(orientationAngle) * curObjectScript.size.y));
                        yLength = (int)Mathf.Round(Mathf.Abs(Mathf.Cos(orientationAngle) * curObjectScript.size.y + Mathf.Sin(orientationAngle) * curObjectScript.size.x));    

                        orientationOffset = orientationAngle + (Mathf.PI * 5/4);
                        cornerOffsetDirection = new Vector2Int((int)Mathf.Round(Mathf.Cos(orientationOffset) / 0.707f), (int)Mathf.Round(Mathf.Sin(orientationOffset) / 0.707f));
                        offsetDistance = new Vector2Int((xLength - 1) * cornerOffsetDirection.x, (yLength - 1) * cornerOffsetDirection.y);


                        for (int x = bounds.min.x + 1; x < bounds.max.x - 1; x++)
                        {
                            for (int y = bounds.min.y + 1; y < bounds.max.y - 1; y++)
                            {
                                startCorner = new Vector2Int(x, y);
                                endCorner = startCorner + offsetDistance;

                                if (CheckObjectLocationValid(startCorner, endCorner, curObjectScript, doors, roomObjects))
                                {
                                    if (CheckObjectLocationPriority(startCorner, endCorner, curObjectScript, roomObjects))
                                    {
                                        priorityIdx.Add(potentialStartCorners.Count);
                                    }
                                    else
                                    {
                                        potentialIdx.Add(potentialStartCorners.Count);
                                    }

                                    potentialStartCorners.Add(startCorner);
                                    potentialEndCorners.Add(endCorner);
                                    potentialOrientations.Add(orientationAngle);
                                }
                            }
                        }

                        chosenIdx = -1;
                        if (priorityIdx.Count > 0)
                        {
                            chosenIdx = priorityIdx[Random.Range(0, priorityIdx.Count)];
                        }
                        else if (potentialIdx.Count > 0)
                        {
                            chosenIdx = potentialIdx[Random.Range(0, potentialIdx.Count)];
                        }

                        if (chosenIdx != -1)
                        {
                            startCorner = potentialStartCorners[chosenIdx];
                            endCorner = potentialEndCorners[chosenIdx];
                            orientationAngle = potentialOrientations[chosenIdx] * Mathf.Rad2Deg;
                            newObject = GameObject.Instantiate(curObject, new Vector3(startCorner.x + 0.5f, startCorner.y + 0.5f, 0), Quaternion.Euler(0,0, orientationAngle), buildingParent.transform);
                            newObject.GetComponent<RoomObjectScript>().AcceptBoundaries(startCorner, endCorner, potentialOrientations[chosenIdx]);
                            if (Random.Range(0, 100) > 50)
                            {
                                newObject.GetComponent<RoomObjectScript>().FlipXAxis();
                            }
                            else if (Random.Range(0, 100) > 50)
                            {
                                newObject.GetComponent<RoomObjectScript>().FlipYAxis();
                            }

                            roomObjects.Add(newObject);
                        }
                    }
                }
            }
        }
    }

    bool CheckObjectLocationValid(Vector2Int startCorner, Vector2Int endCorner, RoomObjectScript roomObjectScript, List<Door> doors, List<GameObject> gos)
    {

        Vector2Int bottomLeft, topRight;

        bottomLeft = new Vector2Int(Mathf.Min(startCorner.x, endCorner.x), Mathf.Min(startCorner.y, endCorner.y));
        topRight = new Vector2Int(Mathf.Max(startCorner.x, endCorner.x), Mathf.Max(startCorner.y, endCorner.y));

        if (bottomLeft.x <= bounds.min.x || bottomLeft.y <= bounds.min.y || topRight.x >= bounds.max.x || topRight.y >= bounds.max.y)
        {
            return false;
        }

        int clearSpace = 0;
        if (!roomObjectScript.allowObjectsNear)
        {
            clearSpace = 2;
        }

        BoundsInt GOBounds;
        foreach(GameObject go in gos)
        {

            GOBounds = go.GetComponent<RoomObjectScript>().bounds;

            if (clearSpace != 2 && !go.GetComponent<RoomObjectScript>().allowObjectsNear)
            {
                clearSpace = 2;
            }

            for (int i = bottomLeft.x - clearSpace; i <= topRight.x + clearSpace; i++)
            {
                for (int j = bottomLeft.y - clearSpace; j <= topRight.y + clearSpace; j++)
                {
                    if ((GOBounds.min.x <= i && i <= GOBounds.max.x) && (GOBounds.min.y <= j && j <= GOBounds.max.y))
                    {
                        return false;
                    }
                }
            }
        }

        foreach (Door door in doors)
        {

            for (int i = bottomLeft.x - clearSpace; i <= topRight.x + clearSpace; i++)
            {
                for (int j = bottomLeft.y - clearSpace; j <= topRight.y + clearSpace; j++)
                {

                    if (Mathf.Abs(door.location.x - i) < 2 && Mathf.Abs(door.location.y - j) < 2)
                    {
                        return false;
                    }

                }
            }

        }

        return true;
    }

    bool CheckObjectLocationPriority(Vector2Int startCorner, Vector2Int endCorner, RoomObjectScript roomObjectScript, List<GameObject> gos)
    {

        Vector2Int bottomLeft, topRight;

        bottomLeft = new Vector2Int(Mathf.Min(startCorner.x, endCorner.x), Mathf.Min(startCorner.y, endCorner.y));
        topRight = new Vector2Int(Mathf.Max(startCorner.x, endCorner.x), Mathf.Max(startCorner.y, endCorner.y));

        int clearSpace = 0;
        if (!roomObjectScript.allowObjectsNear)
        {
            clearSpace = 2;
        }

        BoundsInt GOBounds;
        foreach(GameObject go in gos)
        {

            GOBounds = go.GetComponent<RoomObjectScript>().bounds;

            if (clearSpace != 2 && !go.GetComponent<RoomObjectScript>().allowObjectsNear)
            {
                clearSpace = 2;
            }

            for (int i = bottomLeft.x - clearSpace - 1; i <= topRight.x + clearSpace + 1; i++)
            {
                for (int j = bottomLeft.y - clearSpace - 1; j <= topRight.y + clearSpace + 1; j++)
                {
                    if ((GOBounds.min.x <= i && i <= GOBounds.max.x) && (GOBounds.min.y <= j && j <= GOBounds.max.y))
                    {
                        return true;
                    }
                }
            }
        }

        return false;

    }
}

