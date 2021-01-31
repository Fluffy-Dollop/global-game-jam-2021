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
        if (other.CompareTag("Player"))
        {
            Debug.Log("Triggering a respawn for " + other.name);
            startingPlane.Respawn(other.gameObject);
        } else if (other.CompareTag("item"))
        {
            ItemBehavior item = other.GetComponent<ItemBehavior>();
            Debug.Log("Respawning item: " + item.name + " with type " + item.itemType);
            GameObject.Find("GameManager").GetComponent<GameManager>().Respawn(item);
        }
    }
}
