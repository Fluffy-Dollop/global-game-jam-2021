using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MLAPI;

//https://www.youtube.com/watch?v=vbILVirFV3A
// Fird Person Controller
public class FPC : NetworkedBehaviour
{
    [SerializeField] Transform cam;
    [SerializeField] float speed = 10f;
    [SerializeField] float angularSpeed;

    CharacterController Controller;
    NetworkedObject NetObj;
    Vector2 Move;
    float MouseX;
    float MouseY;
    public float rotateSpeed = 1.0F;
    Vector3 currentEulerAngles;

    void Awake()
    {
        Controller = GetComponent<CharacterController>();
        NetObj = GetComponent<NetworkedObject>();

        Transform cameraXform = this.gameObject.transform.GetChild(0); // camera
        cameraXform.position += Vector3.forward * -5.0f;
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

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            float deltaTime = Time.deltaTime;

            // handle rotation

            currentEulerAngles += new Vector3(-MouseY * rotateSpeed, MouseX * rotateSpeed, 0);
            transform.eulerAngles = currentEulerAngles;

            // handle translation (walking)

            float angDeg = transform.localEulerAngles.y;
            float angRad = angDeg * Mathf.PI / 180.0f;
            float cos = Mathf.Cos(angRad);
            float sin = Mathf.Sin(angRad);

            Vector3 relFwd = Move.y * (cos * Vector3.forward + sin * Vector3.right);
            Vector3 relRgt = Move.x * (cos * Vector3.right - sin * Vector3.forward);
            Vector3 moveDir = (relFwd + relRgt).normalized;
            Vector3 newMove = speed * deltaTime * moveDir;

            Controller.Move(newMove);
        }
    }
}
