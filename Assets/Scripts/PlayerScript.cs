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
        TurnPlayer();
        MovePlayer();
        cam.transform.position = transform.position - new Vector3 (0, 0, 10);
    }

    void TurnPlayer()
    {
        Vector2 mouseScreenPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseScreenPosition - (Vector2) transform.position).normalized;
        transform.up = direction;
        float angle = Mathf.Atan2(direction.y, direction.x);
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up);
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.gameObject);
        }

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
