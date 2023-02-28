using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    private GameObject player;
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

        Vector3 diff = agent.velocity.normalized;
        rotation = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotation - 90);

        CheckForward();
    }

    void CheckForward()
    {

        var rayDirection = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation));
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rayDirection, 20);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == "Wall")
            {
                break;
            } 
            else
            {
                
            }
        }

    }
}
