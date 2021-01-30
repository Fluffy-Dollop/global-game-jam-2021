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

    void PickUpItems()
    {
        // determine triggering
        bool leftItemTriggered = (LeftItemUsed > 0.0f) && (prevLeftItemUsed <= 0.0f);
        bool rightItemTriggered = (RightItemUsed > 0.0f) && (prevRightItemUsed <= 0.0f);

        var pickUpRange = 3.0f;

        // if hit a left mouse click, then want to pick up object in left hand
        if (leftItemTriggered)
        {
            if (leftHandItem)
            {
                // todo: use that item!

                // for now: drop it
                leftHandItem.transform.parent = transform.parent;
                leftHandItem.GetComponent<ItemBehavior>().Drop();
                leftHandItem = null;
            }
            else
            {
                // try to pick up an item in the left hand
                // want to pick up object in left hand
                GameObject found = gameManager.FindClosestItem(transform.position, pickUpRange, rightHandItem);
                if (found)
                {
                    // pick up left object
                    RequestOwnership(found);
                    found.transform.parent = transform;
                    leftHandItem = found;
                    leftHandItem.GetComponent<ItemBehavior>().PickUp();
                }
                else
                {
                    // no object within range
                }
            }
        }

        // if hit a right mouse click, then want to pick up object in right hand
        if (rightItemTriggered)
        {
            if (rightHandItem)
            {
                // todo: use that item!

                // for now: drop it
                rightHandItem.transform.parent = transform.parent;
                rightHandItem = null;
            }
            else
            {
                // try to pick up an item in the right hand
                // want to pick up object in right hand
                GameObject found = gameManager.FindClosestItem(transform.position, pickUpRange, leftHandItem);
                if (found)
                {
                    // pick up right object
                    RequestOwnership(found);
                    found.transform.parent = transform;
                    rightHandItem = found;
                }
                else
                {
                    // no object within range
                }
            }
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
