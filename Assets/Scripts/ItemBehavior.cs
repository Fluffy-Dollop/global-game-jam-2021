using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Prototyping;

public enum ItemType
{
    Normal,  // x 2
    Crown, // x 1
    Utility, // x 3
    Rare // x 1
}

public class ItemBehavior : MonoBehaviour
{
    protected Rigidbody myRigidBody;
    protected Collider myCollider;
    protected GameObject holdingPlayer;
    protected HoldingHand holdingHand = HoldingHand.None;
    NetworkedTransform myNetworkedTransform;

    public bool isKinematic = false;
    public ItemType itemType;
    private bool isActive = false;

    public enum HoldingHand
    {
        None,
        Left,
        Right
    }

    // this is called before Start()
    void Awake()
    {
        // setup components
        myCollider = GetComponent<Collider>();
        myRigidBody = gameObject.AddComponent<Rigidbody>();
        myRigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        myRigidBody.isKinematic = isKinematic;
        myNetworkedTransform = gameObject.AddComponent<NetworkedTransform>();
        tag = "item";
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public bool IsHeld() { return holdingPlayer != null; }
    virtual public void PickUp(GameObject player, HoldingHand whichHand)
    {
        holdingPlayer = player;
        holdingHand = whichHand;
        myRigidBody.useGravity = false;
        myCollider.enabled = false;
        myRigidBody.freezeRotation = true;
        myRigidBody.isKinematic = false;
        myRigidBody.velocity = Vector3.zero;
        myRigidBody.angularVelocity = Vector3.zero;
    }

    virtual public void Drop()
    {
        holdingPlayer = null;
        holdingHand = HoldingHand.None;
        myRigidBody.useGravity = true;
        myCollider.enabled = true;
        myRigidBody.freezeRotation = false;
        myRigidBody.isKinematic = isKinematic;

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

    public void Unspawn()
    {
        GetComponent<NetworkedObject>().UnSpawn();
    }
}
