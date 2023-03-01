using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    private GameObject player, curDoor = null;
    private NavMeshAgent agent;
    private float rotation;

    private void Start() 
    {
        player = GameObject.FindWithTag("Player");

        agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;
		agent.updateUpAxis = false;
    }
    
    private void Update()
    {
        agent.SetDestination(player.transform.position);

        if (!agent.isStopped)
        {
            Vector3 diff = agent.velocity.normalized;
            rotation = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotation - 90);
        }

        //CheckForward();
        CheckPathForDoors();
    }

    void CheckForward()
    {

        float angle = rotation * Mathf.Deg2Rad;
        var rayDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rayDirection, 20);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == "Wall")
            {
                break;
            } 
            else if (hit.collider.tag == "Door")
            {
                if (hit.distance <= 1.5f)
                {
                    if (curDoor != hit.collider.gameObject)
                    {
                        curDoor = hit.collider.gameObject;
                        curDoor.GetComponent<DoorScript>().Interact();
                        agent.isStopped = true;
                    }         
                }
            }
        }

        var rayLine = new Vector2(20 * Mathf.Cos(angle), 20 * Mathf.Sin(angle));
        Debug.DrawRay(transform.position, rayLine, Color.red);

    }

    void CheckPathForDoors()
    {
        var corners = agent.path.corners;

        //Debug.Log(corners.Length);

        if (curDoor == null && corners.Length >= 2)
        {
            for (int i = 0; i < corners.Length - 1; i++)
            {
                float distance = Vector3.Distance(corners[i + 1], corners[i]);
                var rayDirection = new Vector2(corners[i + 1].x - corners[i].x, corners[i + 1].y - corners[i].y);
                RaycastHit2D[] hits = Physics2D.RaycastAll(corners[i], rayDirection, distance);

                foreach (RaycastHit2D hit in hits)
                {
                    // if (hit.collider.tag == "Wall")
                    // {
                    //     break;
                    // } 
                    if (hit.collider.tag == "Door")
                    {
                        // if (hit.distance <= 1.5f)
                        // {
                        //     if (curDoor != hit.collider.gameObject)
                        //     {
                        //         curDoor = hit.collider.gameObject;
                        //         curDoor.GetComponent<DoorScript>().Interact();
                        //         agent.isStopped = true;
                        //     }         
                        // }
                        if (Vector3.Distance(hit.transform.position, transform.position) <= 1.5f)
                        {
                            //Debug.Log("THERES A DOOR");
                            curDoor = hit.collider.gameObject;
                            if (!curDoor.GetComponent<DoorScript>().open && !curDoor.GetComponent<DoorScript>().IsOpening())
                            {
                                agent.isStopped = true;
                                if (!curDoor.GetComponent<DoorScript>().inUse)
                                {
                                    curDoor.GetComponent<DoorScript>().Interact();
                                    curDoor.GetComponent<DoorScript>().inUse = true;
                                }
                            }
                            break;
                        }
                    }
                }            

                //var rayLine = new Vector2(corners[1].x - corners[0].x, corners[1].y - corners[0].y);
                Debug.DrawRay(corners[i], rayDirection, Color.red);

                if (curDoor != null)
                {
                    break;
                }
            }
        }
        else if (curDoor != null)
        {
            if (!curDoor.GetComponent<DoorScript>().IsOpening())
            {
                agent.isStopped = false;
                curDoor.GetComponent<DoorScript>().inUse = false;
                curDoor.GetComponent<DoorScript>().Interact();
                curDoor.GetComponent<DoorScript>().SetLockDuration(2f);
                curDoor = null;
            }
        }

    }
}
