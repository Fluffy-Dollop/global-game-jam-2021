using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using TMPro;

// https://www.youtube.com/watch?v=vbILVirFV3A
// Fird Person Controller
public class FPC : NetworkedBehaviour
{
    [SerializeField] public float speed = 10f;
    [SerializeField] float angularSpeed;
    [SerializeField] TMPro.TMP_Text playerNameTag;
    [SerializeField] GameObject myCamera;

    CharacterController Controller;
    NetworkedObject NetObj;
    Vector2 Move;
    float MouseX;
    float MouseY;
    public float rotateSpeed = 1.0F; // overridden in player prefab!
    Vector3 currentEulerAngles;
    HelpDisplay helpMenu;

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

    // sound effects
    public AudioSource jumpSound;

    // networked vars
    public NetworkedVar<string> playerName = new NetworkedVar<string>(new NetworkedVarSettings { WritePermission = NetworkedVarPermission.OwnerOnly }, "[Unnamed]");

    // on-screen hearts
    GameObject[] hearts = new GameObject[3];
    float maxHealth = 3.0f;
    float health = 2.0f; // between 0.0f and maxHealth

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
        helpMenu = GameObject.Find("HelpMenu").GetComponent<HelpDisplay>();
        startingPlane = GameObject.FindGameObjectWithTag("StartingPlatform").GetComponent<StartingPlane>();

        if (IsLocalPlayer)
        {
            // playerName.OnValueChanged += NameChange;
            // GetComponentInChildren<Camera>().enabled = true;
            // GetComponentInChildren<AudioListener>().enabled = true;
            playerName.Value = networkMenu.playerName;

            // find hearts
            hearts[0] = GameObject.Find("Heart1");
            hearts[1] = GameObject.Find("Heart2");
            hearts[2] = GameObject.Find("Heart3");

            DrawHealth();
        }
        else if (myCamera && myCamera.gameObject)
        {
            myCamera.SetActive(false);
        }
    }

    void DrawHealth()
    {
        float min = 0.25f;
        float heart0 = Mathf.Clamp(health - 0.0f, min, 1.0f);
        float heart1 = Mathf.Clamp(health - 1.0f, min, 1.0f);
        float heart2 = Mathf.Clamp(health - 2.0f, min, 1.0f);
        hearts[0].GetComponent<Image>().color = new Color(1, 1, 1, heart0);
        hearts[1].GetComponent<Image>().color = new Color(1, 1, 1, heart1);
        hearts[2].GetComponent<Image>().color = new Color(1, 1, 1, heart2);
    }

    public void Heal(float health)
    {
        this.health = Mathf.Clamp(this.health + health, 0.0f, maxHealth);
        if (this.health <= 0.0f)
        {
            // todo: die
        }

        // update on-screen health display
        DrawHealth();
    }

    public void Harm(float health) { Heal(-health); }
    public void Hurt(float health) { Harm(health); }

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

    public void OnHelpMenu(InputAction.CallbackContext context)
    {
        if (IsLocalPlayer)
        {
            helpMenu.Toggle();
        }
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
                if (jumpSound)
                {
                    jumpSound.Play();
                }
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

    public void ReleaseHoldOfHand(ItemBehavior.HoldingHand whichHand)
    {
        GameObject itemInHand = null;

        switch (whichHand)
        {
        case ItemBehavior.HoldingHand.Left:
            itemInHand = leftHandItem;
            leftHandItem = null;
            break;
        case ItemBehavior.HoldingHand.Right:
            itemInHand = rightHandItem;
            rightHandItem = null;
            break;
        default:
            break;
        }

        if (itemInHand)
        {
            // ready to commence drop, Captain
            itemInHand.GetComponent<ItemBehavior>().Drop();

            // fix up our scene graph
            itemInHand.transform.parent = transform.parent;
        }
    }

    public IEnumerator TakeHoldOfInHand(ItemBehavior.HoldingHand whichHand, GameObject item)
    {
        // pick up this object in this hand
        ulong itemNetID = item.GetComponent<NetworkedObject>().NetworkId;
        ulong ourClientID = GetComponent<NetworkedObject>().OwnerClientId;
        RpcResponse<bool> response = InvokeServerRpc(RequestOwnershipRPC, ourClientID, itemNetID);

        while (!response.IsDone)
        {
            yield return null;
        }
        // assume response was true (ignore it)

        // complete the handshake process
        item.transform.parent = transform;

        switch (whichHand)
        {
        case ItemBehavior.HoldingHand.Left:
            leftHandItem = item;
            break;
        case ItemBehavior.HoldingHand.Right:
            rightHandItem = item;
            break;
        default:
            break;
        }

        item.GetComponent<ItemBehavior>().PickUp(gameObject, whichHand);
    }

    void UpdateHand(GameObject itemInHand, GameObject itemInOtherHand, bool itemTriggered, bool itemReleased, ItemBehavior.HoldingHand whichHand)
    {
        var pickUpRange = 3.0f;

        if (itemTriggered)
        {
            if (itemInHand)
            {
                if (shouldDropItem)
                {
                    // drop the item
                    ReleaseHoldOfHand(whichHand);
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
                    StartCoroutine(TakeHoldOfInHand(whichHand, found));
                }
                else
                {
                    // no object within range (whoosh)
                }
            }
        }
        else if (itemReleased && itemInHand)
        {
            itemInHand.GetComponent<ItemBehavior>().Deactivate();
        }
    }

    void HandleItems()
    {
        // determine triggering & releasing
        bool leftItemTriggered = (LeftItemUsed > 0.0f) && (prevLeftItemUsed <= 0.0f);
        bool rightItemTriggered = (RightItemUsed > 0.0f) && (prevRightItemUsed <= 0.0f);
        bool leftItemReleased = (prevLeftItemUsed > 0.0f) && (LeftItemUsed <= 0.0f);
        bool rightItemReleased = (prevRightItemUsed > 0.0f) && (RightItemUsed <= 0.0f);

       // update left and right items
        UpdateHand(leftHandItem, rightHandItem, leftItemTriggered, leftItemReleased, ItemBehavior.HoldingHand.Left);
        UpdateHand(rightHandItem, leftHandItem, rightItemTriggered, rightItemReleased, ItemBehavior.HoldingHand.Right);

        // set up for next time
        prevLeftItemUsed = LeftItemUsed;
        prevRightItemUsed = RightItemUsed;
    }

    [ServerRPC] // to be used in a co-routine this has to return something, even if ignored
    private bool RequestOwnershipRPC(ulong clientID, ulong itemNetID)
    {
        GetNetworkedObject(itemNetID).ChangeOwnership(clientID);

        // for now assume success
        return true;
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
