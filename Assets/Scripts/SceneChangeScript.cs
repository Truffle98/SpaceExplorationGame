using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeScript : MonoBehaviour
{

    public string newSceneName;
    public Vector2 newPlayerLocation;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.transform.position = new Vector3(newPlayerLocation.x, newPlayerLocation.y, 0);
            SceneManager.LoadScene(newSceneName);
        }

    }

}
