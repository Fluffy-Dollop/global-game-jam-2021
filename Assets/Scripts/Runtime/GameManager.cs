using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class GameManager : NetworkedBehaviour
{
    void Awake()
    {
    }

    private void Start()
    {
    }

    public GameObject FindClosestItem(Vector3 position, float range)
    {
        GameObject closest = null;

        Collider[] colliders;
        // Presuming the object you are testing also has a collider 0 otherwise
        if ((colliders = Physics.OverlapSphere(position, range)).Length > 1)
        {
            foreach (var collider in colliders)
            {
                var go = collider.gameObject;
                // ignore certain cases
                if (!go) { continue; }
                //Debug.Log("found object " + go.name);
                if (go.name != "torch") { continue; } // for now ignore non-torches!
                // if we made it this far, pick the closest
                if (!closest || (go.transform.position - position).sqrMagnitude < (closest.transform.position - position).sqrMagnitude)
                {
                    closest = go;
                }
            }
        }

        return closest;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
