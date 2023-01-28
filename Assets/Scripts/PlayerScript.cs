using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Camera cam;
    public float speed;
    private Rigidbody2D body;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        TurnPlayer();
        MovePlayer();
        Raycast();
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
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
    }

    void Raycast()
    {
        //Length of the ray
        float laserLength = 50f;
        Vector2 mouseScreenPosition = cam.ScreenToWorldPoint(Input.mousePosition);

        //Get the first object hit by the ray
        RaycastHit2D hit = Physics2D.Raycast(transform.position, mouseScreenPosition, laserLength);

        //If the collider of the object hit is not NUll
        if (hit.collider != null)
        {
            //Hit something, print the tag of the object
            Debug.Log("Hitting: " + hit.collider.tag);
        }

        //Method to draw the ray in scene for debug purpose
        Debug.DrawRay(transform.position, mouseScreenPosition, Color.red);
    }

}
