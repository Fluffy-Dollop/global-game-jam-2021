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
    }

    public override void OnDeactivate()
    {
        // disable collider again
        myCollider.enabled = false;
    }

    // called once per frame
    void Update()
    {
        // position shield out in front of the user & freeze it
        if (IsActive())
        {
            transform.localPosition = new Vector3(0, -1, 2);
            transform.localEulerAngles = new Vector3(0, -90, 0);
        }
        else
        {
            // put to the side
            transform.localPosition = new Vector3(-1, -1, 1);
            transform.localEulerAngles = new Vector3(0, -135, 0);
        }

        if (IsHeld())
        {
            // freeze it!
            myRigidBody.velocity = Vector3.zero;
            myRigidBody.angularVelocity = Vector3.zero;
        }
    }
}
