using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{

    public Vector2Int location;
    public int orientation, openAngle, closedAngle;
    private float orientationAngle;
    private Vector2Int turningPointDirection;
    private Vector3 turningPoint;
    public bool open = false, reversed;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (open)
        {
            if (reversed)
            {
                transform.RotateAround(turningPoint, transform.forward, 90);
            }
            else
            {
                transform.RotateAround(turningPoint, transform.forward, -90);
            }

            open = false;
        }
        else
        {
            if (reversed)
            {
                transform.RotateAround(turningPoint, transform.forward, -90);
            }
            else
            {
                transform.RotateAround(turningPoint, transform.forward, 90);
            }
            open = true;
        }
    }

    public void Setup(Vector2Int locationTemp, int orientationTemp, bool reversedTemp)
    {
        location = locationTemp;
        orientation = orientationTemp;
        orientationAngle = orientation * (Mathf.PI / 2);
        
        reversed = reversedTemp;
        if (reversed)
        {
            turningPointDirection = new Vector2Int((int)Mathf.Round(Mathf.Cos(orientationAngle + (5 * Mathf.PI / 4)) / 0.707f), (int)Mathf.Round(Mathf.Sin(orientationAngle + (5 * Mathf.PI / 4)) / 0.707f));
        }
        else
        {
            turningPointDirection = new Vector2Int((int)Mathf.Round(Mathf.Cos(orientationAngle + (3 * Mathf.PI / 4)) / 0.707f), (int)Mathf.Round(Mathf.Sin(orientationAngle + (3 * Mathf.PI / 4)) / 0.707f));
        }

        turningPoint = new Vector3(location.x + 0.5f + turningPointDirection.x * 0.45f, location.y + 0.5f + turningPointDirection.y * 0.45f, 0);

        transform.position = new Vector3(location.x + 0.5f - Mathf.Cos(orientationAngle) * 0.45f, location.y + 0.5f - Mathf.Sin(orientationAngle) * 0.45f, 0);
        transform.Rotate(0,0, (orientationAngle + Mathf.PI / 2) * Mathf.Rad2Deg);
    }

}
