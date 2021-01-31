using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Prototyping;

public class ShieldItem : ItemBehavior
{
    public override void OnActivate()
    {
        myCollider.enabled = true;

        // position shield out in front of the user
        // for now just put it in a fixed position in space
        transform.position = new Vector3(0, 0, 3);
    }

    public override void OnDeactivate()
    {
        // disable collider again
        myCollider.enabled = false;
    }

    void Update()
    {
        // called once per frame
    }
}
