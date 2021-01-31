using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Prototyping;

public class SwordItem : ItemBehavior
{
    float knockStrength = 20.0f;

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
        if (!IsHeld()) { return; }

        bool left = holdingHand == HoldingHand.Left;

        // position sword out in front of the user & freeze it
        if (IsActive())
        {
            transform.localPosition = new Vector3(0, 0, 2);
            transform.localEulerAngles = new Vector3(90, 0, 0);
        }
        else
        {
            // put to the side
            transform.localPosition = left ? new Vector3(-1, -1, 1) : new Vector3(1, -1, 1);
            transform.localEulerAngles = left ? new Vector3(0, -135, 0) : new Vector3(0, -45, 0);
        }

        // freeze it!
        myRigidBody.velocity = Vector3.zero;
        myRigidBody.angularVelocity = Vector3.zero;
    }

     void OnCollisionStay(Collision col)
     {
        //if (!IsActive()) { return; }

        var go = col.gameObject;
        if (go.tag == "Player")
        {
            // a rigidbody tagged as "Player" hit the player
            print("SwordItem.OnCollisionEnter() w/ Player");

            var fpc = go.GetComponent<FPC>();
            fpc.Hurt(1.0f);
            fpc.Stun();
            var knockDir = (fpc.transform.position - gameObject.transform.position).normalized;
            fpc.KnockBack(knockDir * knockStrength);
        }
     }
}
