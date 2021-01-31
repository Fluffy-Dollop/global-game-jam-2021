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

    public bool isKinematic = false;

    // Start is called before the first frame update
    void Start()
    {
        // setup components
        myCollider = GetComponent<Collider>();
        myRigidBody = gameObject.AddComponent<Rigidbody>();
        myRigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        myRigidBody.isKinematic = isKinematic;
        myNetworkedTransform = gameObject.AddComponent<NetworkedTransform>();
        tag = "item";
    }

    virtual public void PickUp()
    {
        myRigidBody.useGravity = false;
        myCollider.enabled = false;
        myRigidBody.freezeRotation = true;
    }

    virtual public void Drop()
    {
        myRigidBody.useGravity = true;
        myCollider.enabled = true;
        myRigidBody.freezeRotation = false;
    }

    virtual public void Use()
    {
        Debug.Log("place holder: use item");
    }
}
