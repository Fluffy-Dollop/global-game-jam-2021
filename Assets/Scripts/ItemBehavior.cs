using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Prototyping;

public class ItemBehavior : MonoBehaviour
{
    protected Rigidbody myRigidBody;
    protected Collider myCollider;
    protected GameObject holdingPlayer;
    NetworkedTransform myNetworkedTransform;

    public bool isKinematic = false;
    private bool isActive = false;

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

    virtual public void PickUp(GameObject player)
    {
        holdingPlayer = player;
        myRigidBody.useGravity = false;
        myCollider.enabled = false;
        myRigidBody.freezeRotation = true;
    }

    virtual public void Drop()
    {
        holdingPlayer = null;
        myRigidBody.useGravity = true;
        myCollider.enabled = true;
        myRigidBody.freezeRotation = false;

        // be sure to deactivate
        Activate(false);
    }

    public bool IsActive() { return isActive; }

    public void Activate(bool active = true)
    {
        // sanity check
        if (!holdingPlayer) { return; }

        if (active && !IsActive())
        {
            OnActivate();
        }
        else if (!active && IsActive())
        {
            OnDeactivate();
        }
        isActive = active;
    }
    public void Deactivate() { Activate(false); }

    virtual public void OnActivate() {}
    virtual public void OnDeactivate() {}
}
