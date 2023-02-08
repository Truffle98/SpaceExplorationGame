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
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(cam);
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        TurnPlayer();
        RaycastVision();
        MovePlayer();
        MoveCamera();
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

    }

    void MovePlayer()
    {
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
    }

    void MoveCamera()
    {
        cam.transform.position = transform.position + new Vector3(Mathf.Cos(angle) * 10, Mathf.Sin(angle) * 20, -10);
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
            var rayDirection = new Vector2(Mathf.Cos(angle + adjust), Mathf.Sin(angle + adjust));
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rayDirection, lineDistance);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject != gameObject)
                {
                    if (hit.collider.tag == "Wall")
                    {
                        break;
                    } else if (hit.collider.tag == "NeedsLOS")
                    {
                        var vs = hit.collider.gameObject.GetComponent<VisibilityScript>();
                        if (vs.isVisible)
                        {
                            inVision.Add(hit.collider.gameObject);
                            hit.collider.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                        }
                    }
                }
            }
            
            var rayLine = new Vector2(lineDistance * Mathf.Cos(angle+adjust), lineDistance * Mathf.Sin(angle+adjust));
            Debug.DrawRay(transform.position, rayLine, Color.red);
        }

        // Ray casting for behind
        numLines = 25;
        lineDistance = 3f;
        spread = 2f * Mathf.PI / numLines;
        for (int i = 0; i < numLines; i++)
        {
            Vector2 rayDirection = new Vector2(Mathf.Cos(spread*i), Mathf.Sin(spread*i));
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rayDirection, lineDistance);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject != gameObject)
                {
                    if (hit.collider.tag == "Wall")
                    {
                        break;
                    }
                    else if (hit.collider.tag == "NeedsLOS")
                    {
                        hit.collider.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                        newVisible.Add(hit.collider.gameObject);
                    }
                }
            }

            Vector2 rayLine = new Vector2(lineDistance * Mathf.Cos(spread*i), lineDistance * Mathf.Sin(spread*i));
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

    public void MovingRooms()
    {
        inVision.Clear();
        oldInVision.Clear();
    }

}
