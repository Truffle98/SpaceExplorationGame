using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    private GameObject player, curDoor = null;
    private NavMeshAgent agent;
    private float rotation = 90f, lastSeenPlayer = 0f, attackCooldown = 0f, rotationCooldown, remainingRotationAngle, rotationDirection;
    // ROTAITION IS IN DEGREES BECAUSE ITS USED FOR THE TRANSFORM. DO NOT FORGET TO CONVERT.
    private int mode, rotationsRemaining, curLocationPriority, curPatrolIdx;
    public int defaultMode;
    public float sightRange, stopDistFromPlayer;
    //Modes are represented as follows: 0 - Idle/Hold position, 1 - Moving somewhere, 2 - Looking around, 3 - Pursuing player, 4 - Patrolling randomly, 5 - Patrolling streets? <- rough draft
    private bool sawPlayer, moving = false;
    public Vector2 guardLocation;
    private List<Vector2> patrolLocations;
    //private bool? isLeader = null;
    private List<EnemyScript> followers;
    private EnemyScript leader = null;
    private Vector2 lastHeardNoise;

    private void Start() 
    {
        if (defaultMode == 0 || defaultMode == 4)
        {
            SwitchModes(defaultMode);
        }
    }
    
    private void Update()
    {
        CheckForward();
        CheckPathForDoors();

        Vector3 diff;
        if (!sawPlayer && moving && curDoor == null && (mode == 0 || mode == 1 || mode == 3 || mode == 4 || mode == 5))
        {
            diff = agent.velocity.normalized;
            rotation = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotation - 90);
        }

        switch (mode)
        {
            case 0:

                if (moving)
                {
                    if (!agent.pathPending)
                    {
                        if (agent.remainingDistance <= 0.1f + agent.stoppingDistance)
                        {
                            ResetPath();
                            moving = false;
                            rotationCooldown = 2f;
                            remainingRotationAngle = FindRotateAngle();
                            rotationDirection = Mathf.Sign(remainingRotationAngle);
                            remainingRotationAngle = Mathf.Abs(remainingRotationAngle);
                        }
                    }
                }
                else
                {
                    if (rotationCooldown <= 0)
                    {   
                        if (remainingRotationAngle > 0)
                        {
                            rotation += rotationDirection * Mathf.Min(remainingRotationAngle, 90f * Time.deltaTime);
                            remainingRotationAngle -= Mathf.Min(remainingRotationAngle, 90f * Time.deltaTime);
                            transform.rotation = Quaternion.Euler(0,0, rotation - 90);
                        }
                        else
                        {
                            rotationCooldown = 5f;
                            remainingRotationAngle = FindRotateAngle();
                            rotationDirection = Mathf.Sign(remainingRotationAngle);
                            remainingRotationAngle = Mathf.Abs(remainingRotationAngle);
                        }
                    }
                    else
                    {
                        rotationCooldown -= Time.deltaTime;
                    }
                }


                break;

            case 1:

                if (!agent.pathPending)
                {
                    if (agent.remainingDistance <= 0.1f + agent.stoppingDistance)
                    {
                        SwitchModes(2);
                    }
                }

                break;

            case 2:

                if (rotationCooldown <= 0)
                {   
                    if (remainingRotationAngle > 0)
                    {
                       rotation += rotationDirection * Mathf.Min(remainingRotationAngle, 90f * Time.deltaTime);
                       remainingRotationAngle -= Mathf.Min(remainingRotationAngle, 90f * Time.deltaTime);
                       transform.rotation = Quaternion.Euler(0,0, rotation - 90);
                    }
                    else
                    {
                        rotationsRemaining--;
                        rotationCooldown = 2f;
                        remainingRotationAngle = FindRotateAngle();
                        rotationDirection = Mathf.Sign(remainingRotationAngle);
                        remainingRotationAngle = Mathf.Abs(remainingRotationAngle);
                        if (rotationsRemaining < 0)
                        {
                            SwitchModes(defaultMode);
                        }
                    }
                }
                else
                {
                    rotationCooldown -= Time.deltaTime;
                }

                break;

            case 3:
                agent.SetDestination(player.transform.position);

                if (attackCooldown <= 0)
                {
                    if (sawPlayer)
                    {
                        Debug.Log("Attacking");
                        attackCooldown = 5;
                    }
                }
                else
                {
                    attackCooldown -= Time.deltaTime;
                }

                if (sawPlayer)
                {
                    diff = player.transform.position - transform.position;
                    rotation = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, rotation - 90);
                }

                if (!agent.pathPending)
                {
                    if (agent.remainingDistance <= stopDistFromPlayer)
                    {
                        agent.isStopped = true;
                    }
                    else
                    {
                        agent.isStopped = false;
                    }
                }

                lastSeenPlayer -= Time.deltaTime;
                if (lastSeenPlayer <= 0)
                {
                    mode = 1;
                    curLocationPriority = 4;
                }

                break;

            case 4:

                break;

            case 5:

                break;
        }

    }

    void CheckForward()
    {

        int numRays = 11;
        float angle = (rotation) * Mathf.Deg2Rad, visionCone = (Mathf.PI * 5) / 12f, offSet = visionCone / numRays;
        Vector2 rayDirection, rayLine;
        RaycastHit2D[] hits;

        sawPlayer = false;
        for (int i = (int)Mathf.Round(-(numRays - 1) / 2f); i < (int)Mathf.Round((numRays + 1) / 2f); i++)
        {
            rayDirection = new Vector2(Mathf.Cos(angle + (i * offSet)), Mathf.Sin(angle + (i * offSet)));
            hits = Physics2D.RaycastAll(transform.position, rayDirection, sightRange);
            
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.tag == "Wall" || hit.collider.tag == "Door")
                {
                    break;
                } 
                else if (hit.collider.tag == "Player")
                {
                    SwitchModes(3);

                    float distance;
                    EnemyScript[] enemyScripts = FindObjectsOfType<EnemyScript>();
                    foreach (EnemyScript enemyScript in enemyScripts)
                    {
                        if (enemyScript != this)
                        {
                            distance = Vector3.Distance(enemyScript.transform.position, transform.position);
                            if (distance < 10)
                            {
                                enemyScript.SwitchModes(1, player.transform.position);
                            }
                        }
                    }
                    break;
                }
            }

            rayLine = new Vector2(sightRange * Mathf.Cos(angle + (i * offSet)), sightRange * Mathf.Sin(angle + (i * offSet)));
            Debug.DrawRay(transform.position, rayLine, Color.red);
        }

    }

    void CheckPathForDoors()
    {
        if (agent.pathPending)
        {
            return;
        }
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
                                //moving = false;
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
                //moving = true;
                curDoor.GetComponent<DoorScript>().inUse = false;
                curDoor.GetComponent<DoorScript>().Interact();
                curDoor.GetComponent<DoorScript>().SetLockDuration(2f);
                curDoor = null;
            }
        }

    }

    void ResetPath()
    {
        var curRotation = transform.rotation;
        agent.ResetPath();
        transform.rotation = curRotation;
    }

    public void SwitchModes(int newMode, Vector2? location = null)
    {

        switch(newMode)
        {
            case 0:
                mode = 0;
                agent.SetDestination(guardLocation);
                moving = true;
                break;

            case 1:
                mode = 1;
                Vector3 newLoc = (Vector2)location;
                agent.SetDestination(newLoc);
                moving = true;
                break;

            case 2:
                mode = 2;
                ResetPath();
                moving = false;

                rotationsRemaining = 4;
                remainingRotationAngle = FindRotateAngle();
                rotationDirection = Mathf.Sign(remainingRotationAngle);
                remainingRotationAngle = Mathf.Abs(remainingRotationAngle);
                rotationCooldown = 2f;
                break;

            case 3:
                mode = 3;
                lastSeenPlayer = 0.25f;
                curLocationPriority = 10;
                sawPlayer = true;
                moving = true;
                break;

            case 4:
                //mode = 4;

                // float lowDistance = Vector2.Distance(transform.position, patrolLocations[0]), curDistance;
                // Vector2 chosenLoc = patrolLocations[0];
                // for (int i = 1; i < patrolLocations.Count; i++)
                // {
                //     curDistance = Vector2.Distance(transform.position, patrolLocations[i]);
                //     if (curDistance < lowDistance)
                //     {
                //         lowDistance = curDistance;
                //         chosenLoc = patrolLocations[i];
                //     }
                // }

                Vector2 chosenLoc = patrolLocations[Random.Range(0, patrolLocations.Count)];

                if (leader != null)
                {
                    if (leader == this)
                    {
                        foreach (EnemyScript follower in followers)
                        {
                            follower.AcceptOrder(chosenLoc);
                        }
                    }
                }

                SwitchModes(1, chosenLoc);

                break;
        }
    }

    float FindRotateAngle()
    {
        List<float> potentialAngles = new List<float>();
        float curAngle, playerRotation = rotation * Mathf.Deg2Rad;

        for (int i = 0; i < 26; i++)
        {
            curAngle = i * (Mathf.PI / 12f) - Mathf.PI;
            if (!CheckLookingWall(curAngle + playerRotation))
            {
                potentialAngles.Add(curAngle);
            }
        }
        if (potentialAngles.Count > 0)
        {
            return potentialAngles[Random.Range(0, potentialAngles.Count)] * Mathf.Rad2Deg;
        }
        return 0;
    }

    bool CheckLookingWall(float angle)
    {

        var rayDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rayDirection, 2);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == "Wall" || hit.collider.tag == "Door")
            {
                return true;
            }
        }
        return false;

    }

    public void HearNoise(Vector2 noiseLocation, int noisePriority)
    {
        bool useNoise = false;
        if (lastHeardNoise == noiseLocation && noisePriority <= curLocationPriority)
        {
            return;
        }
        lastHeardNoise = noiseLocation;
        if (noisePriority >= curLocationPriority)
        {
            if (!agent.pathPending && noisePriority == curLocationPriority && Vector3.Distance(agent.destination, new Vector3(noiseLocation.x, noiseLocation.y, 0)) < 2)
            {
                useNoise = true;
            }
            else if (noisePriority > curLocationPriority)
            {
                useNoise = true;
            }
        }
        if (useNoise)
        {
            SwitchModes(1, noiseLocation);
            curLocationPriority = noisePriority;

            float distance;
            EnemyScript[] enemyScripts = FindObjectsOfType<EnemyScript>();
            foreach (EnemyScript enemyScript in enemyScripts)
            {
                if (enemyScript != this)
                {
                    distance = Vector3.Distance(enemyScript.transform.position, transform.position);
                    if (distance < 10)
                    {
                        enemyScript.HearNoise(noiseLocation, noisePriority);
                    }
                }
            }
        }
    }
    
    public void SetupEnemy(EnemySetup setup)
    {
        player = GameObject.FindWithTag("Player");

        agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;
		agent.updateUpAxis = false;

        if (setup.type == 0)
        {
            defaultMode = 0;
            guardLocation = setup.guardLocation;
        }
        else if (setup.type == 2)
        {
            defaultMode = 2;
            leader = setup.leader;
            patrolLocations = new List<Vector2>();
            foreach (Vector2 location in setup.patrolLocations)
            {
                patrolLocations.Add(location);
            }
        }
        else if (setup.type == 4)
        {
            defaultMode = 4;
            patrolLocations = new List<Vector2>();
            foreach (Vector2 location in setup.patrolLocations)
            {
                patrolLocations.Add(location);
            }
            if (patrolLocations.Count == 1)
            {
                defaultMode = 0;
                guardLocation = patrolLocations[0];
            }

            if (setup.leader == this)
            {
                leader = setup.leader;
                followers = new List<EnemyScript>();
                foreach (EnemyScript follower in setup.followers)
                {
                    followers.Add(follower);
                }
            }
        }
    }

    public void AcceptOrder(Vector2 location)
    {
        SwitchModes(1, location);
    }
}
