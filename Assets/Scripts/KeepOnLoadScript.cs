using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepOnLoadScript : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
