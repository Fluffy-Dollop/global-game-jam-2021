using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using TMPro;

// https://www.youtube.com/watch?v=vbILVirFV3A
// Fird Person Controller
public class FPC : NetworkedBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float angularSpeed;
    [SerializeField] TMPro.TMP_Text playerNameTag;

    CharacterController Controller;
    NetworkedObject NetObj;
    Vector2 Move;
    float MouseX;
    float MouseY;
    public float rotateSpeed = 1.0F;
    Vector3 currentEulerAngles;

    // buttons
    float prevLeftItemUsed, prevRightItemUsed;
    float LeftItemUsed, RightItemUsed; // 1.0f is depressed, 0.0f not depressed
    bool LeftItemTriggered, RightItemTriggered;
    bool shouldDropItem;
    GameManager gameManager;
    NetworkMenu networkMenu;
    GameObject leftHandItem, rightHandItem;

    // networked vars
    private NetworkedVar<string> playerName = new NetworkedVar<string>(new NetworkedVarSettings { WritePermission = NetworkedVarPermission.OwnerOnly }, "[Unnamed]");

    void Awake()
    {
        Controller = GetComponent<CharacterController>();
        NetObj = GetComponent<NetworkedObject>();
    }

    private void NameChange(string prevName, string newName)
    {
        Debug.Log("Changed player name from " + prevName + " to " + newName);
    }

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        networkMenu = GameObject.Find("NetworkMenu").GetComponent<NetworkMenu>();

        if (IsLocalPlayer)
        {
            // playerName.OnValueChanged += NameChange;
            GetComponentInChildren<Camera>().enabled = true;
            GetComponentInChildren<AudioListener>().enabled = true;
            playerName.Value = networkMenu.playerName;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
        //Debug.Log(Move);
    }

    public void OnRotationX(InputAction.CallbackContext context)
    {
        MouseX = context.ReadValue<float>();
    }

    public void OnRotationY(InputAction.CallbackContext context)
    {
        MouseY = context.ReadValue<float>();
    }

    public void OnLeftItem(InputAction.CallbackContext context)
    {
        LeftItemUsed = context.ReadValue<float>();

        //Debug.Log("-----------");
        //Debug.Log("OnLeftItem()");
        //Debug.Log("LeftItemUsed " + LeftItemUsed);
        //Debug.Log("LeftItemTriggered " + LeftItemTriggered);
    }

    public void OnRightItem(InputAction.CallbackContext context)
    {
        RightItemUsed = context.ReadValue<float>();
        //Debug.Log("Left Item" + RightItemUsed);
        //RightItemTriggered = (RightItemUsed > 0.0f) && (prevRightItemUsed <= 0.0f);
    }

    public void OnDropItem(InputAction.CallbackContext context)
    {
        shouldDropItem = context.ReadValue<float>() > 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            DoMovement();
            PickUpItems();
        }

        playerNameTag.text = playerName.Value;
    }

    void DoMovement()
    {
        float deltaTime = Time.deltaTime;

        // handle rotation

        currentEulerAngles += new Vector3(-MouseY * rotateSpeed, MouseX * rotateSpeed, 0);
        currentEulerAngles.x = Mathf.Clamp(currentEulerAngles.x, -90, 90);
        transform.eulerAngles = currentEulerAngles;

        // handle translation (walking)

        // convert the angle to radian and compute sine and cosine values once
        float angDeg = transform.localEulerAngles.y;
        float angRad = angDeg * Mathf.PI / 180.0f;
        float cos = Mathf.Cos(angRad);
        float sin = Mathf.Sin(angRad);

        // compute relative forward and right as weighted compositions of absolute forward and right
        // how much to weigh by? Follow the sine/cosine curves as you turn!
        Vector3 relFwd = Move.y * (cos * Vector3.forward + sin * Vector3.right);
        Vector3 relRgt = Move.x * (-sin * Vector3.forward + cos * Vector3.right);
        Vector3 moveDir = (relFwd + relRgt).normalized;
        Vector3 newMove = (speed * moveDir + Physics.gravity) * deltaTime;

        Controller.Move(newMove);
    }

    GameObject UpdateHand(GameObject itemInHand, GameObject itemInOtherHand)
    {
        var pickUpRange = 3.0f;

        if (itemInHand)
        {
            if (shouldDropItem)
            {
                // for now: drop it
                itemInHand.transform.parent = transform.parent;
                itemInHand.GetComponent<ItemBehavior>().Drop();
                itemInHand = null;
            }
            else
            {
                // todo: use that item!
                itemInHand.GetComponent<ItemBehavior>().Use();
            }
        }
        else
        {
            // try to pick up an item in this hand
            GameObject found = gameManager.FindClosestItem(transform.position, pickUpRange, itemInOtherHand);
            if (found)
            {
                // pick up this object in this hand
                RequestOwnership(found);
                found.transform.parent = transform;
                itemInHand = found;
                itemInHand.GetComponent<ItemBehavior>().PickUp();
            }
            else
            {
                // no object within range
            }
        }

        return itemInHand;
    }

    void PickUpItems()
    {
        // determine triggering
        bool leftItemTriggered = (LeftItemUsed > 0.0f) && (prevLeftItemUsed <= 0.0f);
        bool rightItemTriggered = (RightItemUsed > 0.0f) && (prevRightItemUsed <= 0.0f);

        // if hit a left mouse click, then want to pick up object in left hand
        if (leftItemTriggered)
        {
            leftHandItem = UpdateHand(leftHandItem, rightHandItem);
        }

        // if hit a right mouse click, then want to pick up object in right hand
        if (rightItemTriggered)
        {
            rightHandItem = UpdateHand(rightHandItem, leftHandItem);
        }

        // set up for next time
        prevLeftItemUsed = LeftItemUsed;
        prevRightItemUsed = RightItemUsed;
    }

    public void RequestOwnership(GameObject go)
    {
        ulong objNetworkID = go.GetComponent<NetworkedObject>().NetworkId;
        ulong ourClientID = GetComponent<NetworkedObject>().OwnerClientId;
        InvokeServerRpc(RequestOwnershipRPC, ourClientID, objNetworkID);
    }

    [ServerRPC]
    private void RequestOwnershipRPC(ulong clientID, ulong objNetworkID)
    {
        GetNetworkedObject(objNetworkID).ChangeOwnership(clientID);
    }

    // warp is a function so that we can set a warp point and late update it, thanks to the character controller that doesn't gaf
    public void Warp(Vector3 newPos)
    {
        Controller.enabled = false;
        transform.position = newPos;
        Controller.enabled = true;
    }
}
