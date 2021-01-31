// GENERATED AUTOMATICALLY FROM 'Assets/Profiles/PlayerController.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerController : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerController()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerController"",
    ""maps"": [
        {
            ""name"": ""GamePlay"",
            ""id"": ""666b97c0-a2c3-471b-b0c8-f152b3065b77"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""675f6fe6-5342-48e6-931c-24fb1ffe6627"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RotationX"",
                    ""type"": ""Value"",
                    ""id"": ""6157fe36-fc5f-4c8c-8f8d-c498813cf18f"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RotationY"",
                    ""type"": ""Value"",
                    ""id"": ""6dfacb8f-2271-491c-a3db-c2e942a02838"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LeftItem"",
                    ""type"": ""Button"",
                    ""id"": ""59a585d7-e897-4452-8e2d-50305aa3c292"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightItem"",
                    ""type"": ""Button"",
                    ""id"": ""7221cb01-902e-4b40-9de3-b6ec153347b4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DropItem"",
                    ""type"": ""Button"",
                    ""id"": ""e9e4cc24-157b-441c-8cd5-08b4084f9a53"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Direction"",
                    ""id"": ""cb54ff83-08a7-42d9-94fe-920feb7a4309"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""31f33a1d-1ebb-4fb7-affa-506f1536b7a9"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""56c3aea9-5c4d-4dc5-a53d-318d9ae8569f"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""60148ec8-ff57-407c-bfeb-ca7f4d32e878"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b128bbc0-1d18-49f6-92c1-4553e07fc7da"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""8c9a6631-9886-491b-bfc5-68f495d130b6"",
                    ""path"": ""<Mouse>/delta/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotationX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""741c62c8-f906-4391-90fd-571468a3184b"",
                    ""path"": ""<Mouse>/delta/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotationY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1beab7c5-52db-4d8b-bb17-3dbf02abac41"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8e04b9ff-13cb-44f4-898a-7c9272a8c674"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dff34337-41bf-40ac-ad55-175b66a73441"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DropItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // GamePlay
        m_GamePlay = asset.FindActionMap("GamePlay", throwIfNotFound: true);
        m_GamePlay_Movement = m_GamePlay.FindAction("Movement", throwIfNotFound: true);
        m_GamePlay_RotationX = m_GamePlay.FindAction("RotationX", throwIfNotFound: true);
        m_GamePlay_RotationY = m_GamePlay.FindAction("RotationY", throwIfNotFound: true);
        m_GamePlay_LeftItem = m_GamePlay.FindAction("LeftItem", throwIfNotFound: true);
        m_GamePlay_RightItem = m_GamePlay.FindAction("RightItem", throwIfNotFound: true);
        m_GamePlay_DropItem = m_GamePlay.FindAction("DropItem", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // GamePlay
    private readonly InputActionMap m_GamePlay;
    private IGamePlayActions m_GamePlayActionsCallbackInterface;
    private readonly InputAction m_GamePlay_Movement;
    private readonly InputAction m_GamePlay_RotationX;
    private readonly InputAction m_GamePlay_RotationY;
    private readonly InputAction m_GamePlay_LeftItem;
    private readonly InputAction m_GamePlay_RightItem;
    private readonly InputAction m_GamePlay_DropItem;
    public struct GamePlayActions
    {
        private @PlayerController m_Wrapper;
        public GamePlayActions(@PlayerController wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_GamePlay_Movement;
        public InputAction @RotationX => m_Wrapper.m_GamePlay_RotationX;
        public InputAction @RotationY => m_Wrapper.m_GamePlay_RotationY;
        public InputAction @LeftItem => m_Wrapper.m_GamePlay_LeftItem;
        public InputAction @RightItem => m_Wrapper.m_GamePlay_RightItem;
        public InputAction @DropItem => m_Wrapper.m_GamePlay_DropItem;
        public InputActionMap Get() { return m_Wrapper.m_GamePlay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GamePlayActions set) { return set.Get(); }
        public void SetCallbacks(IGamePlayActions instance)
        {
            if (m_Wrapper.m_GamePlayActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnMovement;
                @RotationX.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnRotationX;
                @RotationX.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnRotationX;
                @RotationX.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnRotationX;
                @RotationY.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnRotationY;
                @RotationY.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnRotationY;
                @RotationY.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnRotationY;
                @LeftItem.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnLeftItem;
                @LeftItem.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnLeftItem;
                @LeftItem.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnLeftItem;
                @RightItem.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnRightItem;
                @RightItem.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnRightItem;
                @RightItem.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnRightItem;
                @DropItem.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnDropItem;
                @DropItem.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnDropItem;
                @DropItem.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnDropItem;
            }
            m_Wrapper.m_GamePlayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @RotationX.started += instance.OnRotationX;
                @RotationX.performed += instance.OnRotationX;
                @RotationX.canceled += instance.OnRotationX;
                @RotationY.started += instance.OnRotationY;
                @RotationY.performed += instance.OnRotationY;
                @RotationY.canceled += instance.OnRotationY;
                @LeftItem.started += instance.OnLeftItem;
                @LeftItem.performed += instance.OnLeftItem;
                @LeftItem.canceled += instance.OnLeftItem;
                @RightItem.started += instance.OnRightItem;
                @RightItem.performed += instance.OnRightItem;
                @RightItem.canceled += instance.OnRightItem;
                @DropItem.started += instance.OnDropItem;
                @DropItem.performed += instance.OnDropItem;
                @DropItem.canceled += instance.OnDropItem;
            }
        }
    }
    public GamePlayActions @GamePlay => new GamePlayActions(this);
    public interface IGamePlayActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnRotationX(InputAction.CallbackContext context);
        void OnRotationY(InputAction.CallbackContext context);
        void OnLeftItem(InputAction.CallbackContext context);
        void OnRightItem(InputAction.CallbackContext context);
        void OnDropItem(InputAction.CallbackContext context);
    }
}
