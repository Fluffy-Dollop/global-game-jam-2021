using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Prototyping;

public class ItemBehavior : MonoBehaviour
{
    Rigidbody myRigidBody;
    Collider myCollider;
    NetworkedTransform myNetworkedTransform;

    // Start is called before the first frame update
    void Start()
    {
        // setup components
        myCollider = GetComponent<Collider>();
        myRigidBody = gameObject.AddComponent<Rigidbody>();
        myRigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        myNetworkedTransform = gameObject.AddComponent<NetworkedTransform>();
        tag = "item";
    }

    public void PickUp()
    {
        myRigidBody.useGravity = false;
        myCollider.enabled = false;
        myRigidBody.freezeRotation = true;
    }

    public void Drop()
    {
        myRigidBody.useGravity = true;
        myCollider.enabled = true;
        myRigidBody.freezeRotation = false;
    }
}
