using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObjectScript : MonoBehaviour
{

    //Room positioning means either center, wall, or corner
    //Maybe not corner might just do center or wall
    public string objectName, roomPositioning;
    [HideInInspector]
    public BoundsInt bounds;
    [HideInInspector]
    public float orientationAngle;
    public Vector2 size;
    public bool allowObjectsNear;

    public void AcceptBoundaries(Vector2Int startCorner, Vector2Int endCorner, float orientation)
    {

        orientationAngle = orientation;
        Vector2Int bottomLeft, topRight;

        bottomLeft = new Vector2Int(Mathf.Min(startCorner.x, endCorner.x), Mathf.Min(startCorner.y, endCorner.y));
        topRight = new Vector2Int(Mathf.Max(startCorner.x, endCorner.x), Mathf.Max(startCorner.y, endCorner.y));

        bounds = new BoundsInt(new Vector3Int(bottomLeft.x, bottomLeft.y, 0), new Vector3Int(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y, 0));

    }

    public void FlipXAxis()
    {
        float orientationOffset = orientationAngle + (5 * Mathf.PI / 4);
        Vector2Int direction = new Vector2Int((int)Mathf.Round(Mathf.Cos(orientationOffset) / 0.707f), (int)Mathf.Round(Mathf.Sin(orientationOffset) / 0.707f));
        Vector3 objectCenter = transform.position + new Vector3(-0.5f * direction.x, -0.5f * direction.y, 0);
        
        int xLength = (int)Mathf.Round(Mathf.Abs(Mathf.Cos(orientationAngle) * size.x + Mathf.Sin(orientationAngle) * size.y));
        int yLength = (int)Mathf.Round(Mathf.Abs(Mathf.Cos(orientationAngle) * size.y + Mathf.Sin(orientationAngle) * size.x));

        objectCenter += new Vector3(direction.x * xLength / 2f, direction.y * yLength / 2f, 0);
        transform.RotateAround(objectCenter, Vector3.right, 180);
    }

    public void FlipYAxis()
    {
        float orientationOffset = orientationAngle + (5 * Mathf.PI / 4);
        Vector2Int direction = new Vector2Int((int)Mathf.Round(Mathf.Cos(orientationOffset) / 0.707f), (int)Mathf.Round(Mathf.Sin(orientationOffset) / 0.707f));
        Vector3 objectCenter = transform.position + new Vector3(-0.5f * direction.x, -0.5f * direction.y, 0);

        int xLength = (int)Mathf.Round(Mathf.Abs(Mathf.Cos(orientationAngle) * size.x + Mathf.Sin(orientationAngle) * size.y));
        int yLength = (int)Mathf.Round(Mathf.Abs(Mathf.Cos(orientationAngle) * size.y + Mathf.Sin(orientationAngle) * size.x));

        objectCenter += new Vector3(direction.x * xLength / 2f, direction.y * yLength / 2f, 0);
        transform.RotateAround(objectCenter, Vector3.up, 180);
    }

}