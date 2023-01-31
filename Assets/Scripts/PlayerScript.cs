using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Camera cam;
    public float speed, angle;
    private float playerRotation = Mathf.PI / 2f;
    private Rigidbody2D body;
    private List<GameObject> newVisible = new List<GameObject>(), oldVisible = new List<GameObject>(), 
    inVision = new List<GameObject>(), oldInVision = new List<GameObject>();

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        TurnPlayer();
        RaycastVision();
        MovePlayer();
        cam.transform.position = transform.position - new Vector3 (0, 0, 10);
    }

    void TurnPlayer()
    {
        // These variables/section primarily deal with rotating the player. We find the mouse's world position and
        // rotate the player to the position angle
        Vector2 mouseScreenPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseScreenPosition - (Vector2) transform.position).normalized;
        float desiredAngle = Mathf.Atan2(direction.y, direction.x);

        if (desiredAngle > Mathf.PI / 2 && playerRotation < -Mathf.PI / 2)
        {
            playerRotation += Mathf.PI * 2;
        }
        else if (desiredAngle < -Mathf.PI / 2 && playerRotation > Mathf.PI / 2)
        {
            desiredAngle += Mathf.PI * 2;
        }

        angle = (desiredAngle - playerRotation) * 0.1f + playerRotation;

        if (angle > Mathf.PI)
        {
            angle -= Mathf.PI * 2;
        }
        else if (angle < -Mathf.PI)
        {
            angle += Mathf.PI * 2;
        }

        playerRotation = angle;
        transform.up = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        // Local vision raycasting section
        int numLines = 25; // 25 raycasting lines
        float lineDistance = 2f; // How far local vision extends (2 units)
        float leftAngle = angle + Mathf.PI / 6f; // Left angle represents where we should start drawing the rays
        float spread = 2f * Mathf.PI / numLines; // Defines the spread between each ray (evenly spaced over 2PI by 25 lines)

        // Updating the oldVisible array
        oldVisible.Clear();
        foreach (GameObject go in newVisible)
        {
            oldVisible.Add(go);
        }
        newVisible.Clear();

        // Creating rays for local vision and changing visibility on objects appropriately
        for (int i = 0; i < numLines; i++)
        {
            Vector2 rayDirection = new Vector2(Mathf.Cos(leftAngle+spread*i), Mathf.Sin(leftAngle+spread*i));
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rayDirection, lineDistance);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject != gameObject) // Make sure the object we detect isn't the player
                {
                    if (hit.collider.tag == "Wall")
                    {
                        break;
                    }

                    newVisible.Add(hit.collider.gameObject);
                    if (!oldVisible.Contains(hit.collider.gameObject))
                    {
                        hit.collider.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    }
                }
            }

            Vector2 rayLine = new Vector2(lineDistance * Mathf.Cos(leftAngle+spread*i), lineDistance * Mathf.Sin(leftAngle+spread*i));
            Debug.DrawRay(transform.position, rayLine, Color.red);
        }

        // Disable no longer visible objects
        foreach (GameObject go in oldVisible)
        {
            if (!newVisible.Contains(go))
            {
                go.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    void MovePlayer()
    {
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
    }

    void RaycastVision()
    {
        float lineDistance = 50;
        float spread = 0.025f;
        int numLines = 25;

        inVision.Clear();
        for (int i = 0; i < numLines; i++)
        {
            float adjust = (i * spread) - (spread * (numLines / 2));
            Vector2 rayDirection = new Vector2(Mathf.Cos(angle + adjust), Mathf.Sin(angle + adjust));
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rayDirection, lineDistance);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject != gameObject)
                {
                    if (hit.collider.tag == "Wall")
                    {
                        break;
                    } else {
                        VisibilityScript vs = hit.collider.gameObject.GetComponent<VisibilityScript>();
                        if (vs != null)
                        {
                            inVision.Add(hit.collider.gameObject);
                            if (vs.isVisible)
                            {
                                hit.collider.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                            }
                        }
                    }
                }
            }
            
            Vector2 rayLine = new Vector2(lineDistance * Mathf.Cos(angle+adjust), lineDistance * Mathf.Sin(angle+adjust));
            Debug.DrawRay(transform.position, rayLine, Color.red);
        }

        foreach (GameObject go in oldInVision)
        {
            if (!inVision.Contains(go))
            {
                go.GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        foreach (GameObject go in inVision)
        {
            oldInVision.Add(go);
        }
    }

}
