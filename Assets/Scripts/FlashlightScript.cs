using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightScript : MonoBehaviour
{
    public float visionLength = 10;
    private Light2D flashlight;
    private bool lightOn;
    private Camera cam;
    private float angle;
    private Rigidbody2D body;
    private PlayerScript playerScript;
    private List<GameObject> newVisible = new List<GameObject>(), oldVisible = new List<GameObject>();

    void Start()
    {
        playerScript = gameObject.transform.parent.gameObject.GetComponent<PlayerScript>();
        flashlight = gameObject.GetComponent<Light2D>();
        flashlight.pointLightOuterRadius = visionLength;
        lightOn = false;
        cam = playerScript.cam;
        angle = playerScript.angle; // Grabbing the viewing angle from parent player script
    }

    void Update()
    {
        LookForInput();
        DoFlashlightAnim();
        DoFlashlightRaycasting();
    }

    // Looks for input and turns the light on and off
    void LookForInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            lightOn = !lightOn;
        }
    }

    // Checks for appropriate time to do flashlight animation.
    void DoFlashlightAnim()
    {
        if (lightOn && flashlight.pointLightOuterRadius < visionLength)
        {
            flashlight.pointLightOuterRadius += .5f;
        } else if (!lightOn && flashlight.pointLightOuterRadius > 2)
        {
            flashlight.pointLightOuterRadius -= .5f;
        }
    }

    // Checks if light is turned on. If true, the flashlight looks for rotation.
    void DoFlashlightRaycasting()
    {
        if (lightOn)
        {
            Raycast(); // Only raycast when the light is on
        } else 
        {
            foreach (GameObject go in newVisible)
            {
                if (go != gameObject.transform.parent.gameObject)
                {
                    go.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
            newVisible.Clear();
        }
    }
    
    // Ray casts the flashlight
    void Raycast()
    {
        angle = playerScript.angle;
        
        float lineDistance = visionLength;
        float spread = 0.05f;
        int numLines = 18;

        oldVisible.Clear();
        foreach (GameObject go in newVisible)
        {
            oldVisible.Add(go);
        }
        newVisible.Clear();

        // Creating rays for flashlight vision and changing visibility appropriately
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
                    }

                    newVisible.Add(hit.collider.gameObject);
                    hit.collider.gameObject.GetComponent<SpriteRenderer>().enabled = true;
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
    }
}
