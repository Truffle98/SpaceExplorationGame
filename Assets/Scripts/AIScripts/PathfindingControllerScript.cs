using NavMeshPlus.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathfindingControllerScript : MonoBehaviour
{

    //public NavMeshSurface Surface2D;

    void Start()
    {

        //Surface2D.BuildNavMeshAsync();
        Component[] allComponents;
        allComponents = gameObject.GetComponents<Component>();
        foreach (Component component in allComponents) 
        {
            //Debug.Log(component.name);
        }



    }


}