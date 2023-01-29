using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Camera cam;
    public float speed;
    private float playerRotation = Mathf.PI / 2f;
    private Rigidbody2D body;
    private List<GameObject> newVisible = new List<GameObject>(), oldVisible = new List<GameObject>();

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
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
        float desiredAngle = Mathf.Atan2(direction.y, direction.x);

        if (desiredAngle > Mathf.PI / 2 && playerRotation < -Mathf.PI / 2)
        {
            playerRotation += Mathf.PI * 2;
        }
        else if (desiredAngle < -Mathf.PI / 2 && playerRotation > Mathf.PI / 2)
        {
            desiredAngle += Mathf.PI * 2;
        }

        float angle = (desiredAngle - playerRotation) * 0.1f + playerRotation;

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
        
        float lineDistance = 10;
        float spread = 0.075f;
        int numLines = 13;

        oldVisible.Clear();
        foreach (GameObject go in newVisible)
        {
            oldVisible.Add(go);
        }
        newVisible.Clear();

        for (int i = 0; i < numLines; i++)
        {
            float adjust = (i * spread) - (spread * (numLines / 2));
            Vector2 rayDirection = new Vector2(lineDistance * Mathf.Cos(angle + adjust), lineDistance * Mathf.Sin(angle + adjust));
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rayDirection, 10);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject != gameObject)
                {
                    if (hit.collider.tag == "Wall")
                    {
                        break;
                    }

                    if (!newVisible.Contains(hit.collider.gameObject))
                    {
                        //Debug.Log("Can see: " + hit.collider.gameObject);
                        newVisible.Add(hit.collider.gameObject);
                    }
                }
            }

            Vector2 rayLine = new Vector2(lineDistance * Mathf.Cos(angle + adjust), lineDistance * Mathf.Sin(angle + adjust));
            Debug.DrawRay(transform.position, rayLine, Color.red);
        }

        foreach (GameObject go in oldVisible)
        {
            if (!newVisible.Contains(go))
            {
                go.GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        foreach (GameObject go in newVisible)
        {
            if (!oldVisible.Contains(go))
            {
                go.GetComponent<SpriteRenderer>().enabled = true;
            }
        }

    }

    void MovePlayer()
    {
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
    }

}
