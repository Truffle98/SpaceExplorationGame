using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyScript : MonoBehaviour
{
    private GameObject player;
    private AIDestinationSetter targetSetter;

    private void Start() 
    {
        player = GameObject.FindWithTag("Player");
        targetSetter = GetComponent<AIDestinationSetter>();
        targetSetter.target = player.transform;
    }
}
