using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Camera cam;
    public float speed, angle, maxCooldown = 500;
    private float playerRotation = Mathf.PI / 2f, sprintTime, baseSpeed, sprintingSpeed, cooldownTime;
    private Rigidbody2D body;
    private List<GameObject> newVisible = new List<GameObject>(), oldVisible = new List<GameObject>(), 
    inVision = new List<GameObject>(), oldInVision = new List<GameObject>();
    public FOVScript[] fovs;

    void Start()
    {
        baseSpeed = speed;
        sprintingSpeed = speed * 2;
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
        CheckInteract();
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
        if (Input.GetKey(KeyCode.LeftShift) && sprintTime < maxCooldown && cooldownTime == 0)
        {
            speed = sprintingSpeed;
            sprintTime++;
            if (sprintTime>maxCooldown-1)
            {
                cooldownTime = maxCooldown;
            }
        } else {
            speed = baseSpeed;
            sprintTime = 0;
            if (cooldownTime > 0)
            {
                cooldownTime--;
            }
        }

        //body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (move.magnitude > 1.0f)
        {
            move.Normalize();
        }
        move *= speed;
        body.velocity = move;

        float distance;
        RaycastHit2D[] hits;
        Vector2 rayDirection;
        EnemyScript[] enemyScripts = FindObjectsOfType<EnemyScript>();
        foreach (EnemyScript enemyScript in enemyScripts)
        {
            distance = Vector3.Distance(enemyScript.transform.position, transform.position);
            rayDirection = (Vector2)enemyScript.transform.position - (Vector2)transform.position;

            hits = Physics2D.RaycastAll(transform.position, rayDirection, distance);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.tag == "Wall")
                {
                    distance *= 2;
                    break;
                }
            }

            if (distance < body.velocity.magnitude)
            {
                enemyScript.HearNoise(transform.position, 5);
            }
        }
    }

    Vector3 curOffset = new Vector3(0,0,0);
    void MoveCamera()
    {
        Vector3 desiredOffset;
        Vector3 cameraDiff = cam.transform.position - transform.position;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Vector2 cursorDifference = (Input.mousePosition / new Vector2(Screen.width / 2f, Screen.height / 2f) - new Vector2(1, 1));
            
            desiredOffset = new Vector3(10f * cursorDifference.x, 10f * cursorDifference.y, 0);
        }
        else
        {
            desiredOffset = new Vector3(0,0,0);
        }

        Vector3 desiredMove = (desiredOffset - curOffset) * 3f * Time.deltaTime;
        if (desiredMove.x != 0 && Mathf.Abs(desiredMove.x) < 0.1f * Time.deltaTime)
        {
            desiredMove.x = (desiredOffset - curOffset).x;
        }
        if (desiredMove.y != 0 && Mathf.Abs(desiredMove.y) < 0.1f * Time.deltaTime)
        {
            desiredMove.y = (desiredOffset - curOffset).y;
        }
        curOffset += desiredMove;
        curOffset.z = -10;

        cam.transform.position = transform.position + curOffset;
    }

    void RaycastVision()
    {
        foreach (FOVScript fov in fovs)
        {
            fov.UpdateFOV();
        }

        /*
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
                    }
                    else if (hit.collider.tag == "NeedsLOS")
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
        */
    }

    void CheckInteract()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Debug.Log("Checking door");
            var rayDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rayDirection, 1.5f);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.tag == "Wall")
                {
                    break;
                }
                else if (hit.collider.tag == "Door")
                {
                    //Debug.Log("FOUND DOOR");
                    hit.collider.gameObject.GetComponent<DoorScript>().Interact();
                }
            }
        }
    }

    public void MovingRooms()
    {
        inVision.Clear();
        oldInVision.Clear();
    }

}
