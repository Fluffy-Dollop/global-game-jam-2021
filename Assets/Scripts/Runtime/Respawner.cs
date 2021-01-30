using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    public StartingPlane startingPlane;

    // Start is called before the first frame update
    void Start()
    {
        startingPlane = GameObject.FindGameObjectWithTag("StartingPlatform").GetComponent<StartingPlane>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision from " + other.tag);
        if (other.CompareTag("Player") || other.CompareTag("item"))
        {
            Debug.Log("Triggering a respawn for " + other.name);
            startingPlane.Respawn(other.gameObject);
        }
    }
}
