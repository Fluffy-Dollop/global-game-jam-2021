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

public class ItemBehavior : NetworkedBehaviour
{
    protected Rigidbody myRigidBody;
    protected Collider myCollider;
    protected GameObject holdingPlayer;
    protected HoldingHand holdingHand = HoldingHand.None;
    NetworkedTransform myNetworkedTransform;

    public bool isKinematic = false;
    public ItemType itemType;
    private bool isActive = false;

    public GameManager gameManager;

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
    protected virtual void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    virtual public void OnPickUp() {}
    virtual public void OnDrop() {}

    public bool IsHeld() { return holdingPlayer != null; }
    virtual public void PickUp(GameObject player, HoldingHand whichHand)
    {
        if (IsHeld()) { return; }

        holdingPlayer = player;
        holdingHand = whichHand;
        myRigidBody.useGravity = false;
        myCollider.enabled = false;
        myRigidBody.freezeRotation = true;
        myRigidBody.isKinematic = false;
        myRigidBody.velocity = Vector3.zero;
        myRigidBody.angularVelocity = Vector3.zero;

        OnPickUp();
    }

    virtual public void Drop()
    {
        if (!IsHeld()) { return; }

        // be sure to deactivate first
        Activate(false);

        OnDrop();

        // disconnect from player
        holdingPlayer = null;
        holdingHand = HoldingHand.None;

        myRigidBody.useGravity = true;
        myCollider.enabled = true;
        myRigidBody.freezeRotation = false;
        myRigidBody.isKinematic = isKinematic;
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
        OnDespawn();
    }

    public void Respawn()
    {
        if (holdingPlayer)
        {
            holdingPlayer.GetComponent<FPC>().ReleaseHoldOfHand(holdingHand);
        }
        OnDespawn();
        gameManager.Respawn(this);
    }

    virtual public void OnDespawn() {} // called for either unspawn or respawn
}
