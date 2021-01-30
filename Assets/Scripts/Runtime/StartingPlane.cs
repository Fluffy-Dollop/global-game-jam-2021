using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingPlane : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // disable these on start
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshCollider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Respawn(GameObject target)
    {
        Debug.Log("Got target to respawn " + target.name);

        Vector3 newPos = new Vector3(
            transform.position.x + Random.Range(-transform.lossyScale.x, transform.lossyScale.x),
            transform.position.y,
            transform.position.z + Random.Range(-transform.lossyScale.z, transform.lossyScale.z)
        );

        Debug.Log("New Position: " + newPos);

        switch(target.tag)
        {
            case "Player":
                target.GetComponent<FPC>().Warp(newPos);
                break;
            case "item":
            default:
                target.transform.position = newPos;
                break;
        }
    }
}
