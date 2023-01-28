using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Camera cam;
    const float moveSpeed = 0.25f;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        FaceCursor();
        MovePlayer();
        cam.transform.position = transform.position - new Vector3 (0, 0, 10);
    }

    void FaceCursor()
    {
        Vector2 mouseScreenPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseScreenPosition - (Vector2) transform.position).normalized;
        transform.up = direction;
    }

    void MovePlayer()
    {
        if (Input.GetKey("w"))
        {
            transform.position += new Vector3(0, moveSpeed, 0);
        }

        if (Input.GetKey("s"))
        {
            transform.position += new Vector3(0, -moveSpeed, 0);
        }

        if (Input.GetKey("d"))
        {
            transform.position += new Vector3(moveSpeed, 0, 0);
        }

        if (Input.GetKey("a"))
        {
            transform.position += new Vector3(-moveSpeed, 0, 0);
        }
    }
}
