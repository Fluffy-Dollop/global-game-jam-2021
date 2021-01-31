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

    [SerializeField] GameObject myCamera;

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
    bool shouldJump;
    GameManager gameManager;
    NetworkMenu networkMenu;
    GameObject leftHandItem, rightHandItem;
    StartingPlane startingPlane;

    // for jumping/falling
    float velY = 0.0f;
    float jumpForce = 5.0f;

    // networked vars
    public NetworkedVar<string> playerName = new NetworkedVar<string>(new NetworkedVarSettings { WritePermission = NetworkedVarPermission.OwnerOnly }, "[Unnamed]");

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
        startingPlane = GameObject.FindGameObjectWithTag("StartingPlatform").GetComponent<StartingPlane>();

        if (IsLocalPlayer)
        {
            // playerName.OnValueChanged += NameChange;
            // GetComponentInChildren<Camera>().enabled = true;
            // GetComponentInChildren<AudioListener>().enabled = true;
            playerName.Value = networkMenu.playerName;
        }
        else if (myCamera && myCamera.gameObject)
        {
            myCamera.SetActive(false);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
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

    public void OnJump(InputAction.CallbackContext context)
    {
        shouldJump = context.ReadValue<float>() > 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        switch(gameManager.gameState)
        {
            case GameState.GameCountdown:
            case GameState.GameLobby:
                if (!startingPlane.IsInside(transform.position))
                {
                    startingPlane.Respawn(gameObject);
                }
                break;
        }

        if (IsLocalPlayer)
        {
            DoMovement();
            HandleItems();

            // note: Controller.isGrounded may be true ONLY after Controller.Move() is called!
            if (shouldJump && Controller.isGrounded)
            {
                velY = jumpForce;
            }
        }

        if (playerNameTag != null && playerName != null)
        {
            playerNameTag.text = playerName.Value;
        }
    }

    void DoMovement()
    {
        float deltaTime = Time.deltaTime;

        // handle rotation
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 playerForward = Controller.transform.forward;
        float angle = Vector3.Angle(camForward, playerForward);

        currentEulerAngles += new Vector3(0, MouseX * rotateSpeed, 0);
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

        // update velocity
        velY += Physics.gravity.y * deltaTime;

        // update position
        Vector3 newMove = (speed * moveDir + velY * Vector3.up) * deltaTime;
        Controller.Move(newMove);

        if (Controller.isGrounded)
        {
            velY = 0;
        }
    }

    GameObject UpdateHand(GameObject itemInHand, GameObject itemInOtherHand, bool itemTriggered, bool itemReleased, ItemBehavior.HoldingHand whichHand)
    {
        var pickUpRange = 3.0f;

        if (itemTriggered)
        {
            if (itemInHand)
            {
                if (shouldDropItem)
                {
                    // drop the item
                    itemInHand.transform.parent = transform.parent;
                    itemInHand.GetComponent<ItemBehavior>().Drop();
                    itemInHand = null;
                }
                else
                {
                    // use that item!
                    itemInHand.GetComponent<ItemBehavior>().Activate();
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
                    itemInHand.GetComponent<ItemBehavior>().PickUp(gameObject, whichHand);
                }
                else
                {
                    // no object within range
                }
            }
        }
        else if (itemReleased && itemInHand)
        {
            itemInHand.GetComponent<ItemBehavior>().Deactivate();
        }

        return itemInHand;
    }

    void HandleItems()
    {
        // determine triggering & releasing
        bool leftItemTriggered = (LeftItemUsed > 0.0f) && (prevLeftItemUsed <= 0.0f);
        bool rightItemTriggered = (RightItemUsed > 0.0f) && (prevRightItemUsed <= 0.0f);
        bool leftItemReleased = (prevLeftItemUsed > 0.0f) && (LeftItemUsed <= 0.0f);
        bool rightItemReleased = (prevRightItemUsed > 0.0f) && (RightItemUsed <= 0.0f);

       // update left and right items
        leftHandItem = UpdateHand(leftHandItem, rightHandItem, leftItemTriggered, leftItemReleased, ItemBehavior.HoldingHand.Left);
        rightHandItem = UpdateHand(rightHandItem, leftHandItem, rightItemTriggered, rightItemReleased, ItemBehavior.HoldingHand.Right);

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

    public void NextGameState(InputAction.CallbackContext context)
    {
        if ((IsServer || IsHost) && IsLocalPlayer)
        {
            if (context.performed)
            {
                // aliasing from player object, we just want one controller in one spot for now...
                gameManager.NextGameState(this);
            }
        }
    }
}
