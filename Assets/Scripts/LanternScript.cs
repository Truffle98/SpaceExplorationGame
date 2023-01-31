using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternScript : MonoBehaviour
{
    public float visionLength = 2f;
    private int numLines;
    private float spread; 

    void Start()
    {
        numLines = 25;  // 25 raycasting lines
        spread = 2f * Mathf.PI / numLines; // Defines the spread between each ray (evenly spaced over 2PI by 25 lines)
        Raycast();
    }

    void Update()
    {
        Raycast();
    }

    void Raycast()
    {
        for (int i = 0; i < numLines; i++)
        {
            Vector2 rayDirection = new Vector2(Mathf.Cos(spread*i), Mathf.Sin(spread*i));
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rayDirection, visionLength);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject != gameObject) // Make sure the object we detect isn't the player
                {
                    if (hit.collider.tag == "Wall")
                    {
                        break;
                    }
                    
                    VisibilityScript vs = hit.collider.gameObject.GetComponent<VisibilityScript>();

                    if (vs != null)
                    {
                        hit.collider.gameObject.GetComponent<VisibilityScript>().isVisible = true;
                    }
                }
            }

            Vector2 rayLine = new Vector2(visionLength * Mathf.Cos(spread*i), visionLength * Mathf.Sin(spread*i));
            Debug.DrawRay(transform.position, rayLine, Color.red);
        }
    }
}
