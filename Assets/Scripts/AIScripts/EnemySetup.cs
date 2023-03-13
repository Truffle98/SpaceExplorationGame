using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySetup
{

    public int type; //0 is guard location, 4 is patrol locations, 2 is follower
    public Vector2 guardLocation;
    public List<Vector2> patrolLocations;
    public EnemyScript leader = null;
    public List<EnemyScript> followers;

}