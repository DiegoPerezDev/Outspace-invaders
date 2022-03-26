// GENERATED AUTOMATICALLY FROM 'Assets/Outspace invaders/Scripts/Inputs/InputsActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputsActionsG : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputsActionsG()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputsActions"",
    ""maps"": [
        {
            ""name"": ""Player Action Map"",
            ""id"": ""7aa156d8-c72a-4e46-adc5-20cc1c10599e"",
            ""actions"": [
                {
                    ""name"": ""right"",
                    ""type"": ""Button"",
                    ""id"": ""eccbcff5-cff6-43af-b4f6-93b0bb04eb9a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""left"",
                    ""type"": ""Button"",
                    ""id"": ""b701b53d-6b75-4bae-863f-cb4f502d51ba"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""shoot"",
                    ""type"": ""Button"",
                    ""id"": ""4de732bc-9449-4060-b9b6-d1cc07d78b5d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""d35c9cdd-2c7e-4e70-9ae6-b8978bf5b415"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": ""keyboard"",
                    ""action"": ""right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1a7f7e6f-02c0-4e03-84dd-21a95b8de60e"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": ""keyboard"",
                    ""action"": ""left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3ec1ac62-b1a3-4a72-bef2-44ee4d5ddeb8"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""keyboard"",
                    ""action"": ""shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""keyboard"",
            ""bindingGroup"": ""keyboard"",
            ""devices"": []
        },
        {
            ""name"": ""android"",
            ""bindingGroup"": ""android"",
            ""devices"": []
        }
    ]
}");
        // Player Action Map
        m_PlayerActionMap = asset.FindActionMap("Player Action Map", throwIfNotFound: true);
        m_PlayerActionMap_right = m_PlayerActionMap.FindAction("right", throwIfNotFound: true);
        m_PlayerActionMap_left = m_PlayerActionMap.FindAction("left", throwIfNotFound: true);
        m_PlayerActionMap_shoot = m_PlayerActionMap.FindAction("shoot", throwIfNotFound: true);
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

    // Player Action Map
    private readonly InputActionMap m_PlayerActionMap;
    private IPlayerActionMapActions m_PlayerActionMapActionsCallbackInterface;
    private readonly InputAction m_PlayerActionMap_right;
    private readonly InputAction m_PlayerActionMap_left;
    private readonly InputAction m_PlayerActionMap_shoot;
    public struct PlayerActionMapActions
    {
        private @InputsActionsG m_Wrapper;
        public PlayerActionMapActions(@InputsActionsG wrapper) { m_Wrapper = wrapper; }
        public InputAction @right => m_Wrapper.m_PlayerActionMap_right;
        public InputAction @left => m_Wrapper.m_PlayerActionMap_left;
        public InputAction @shoot => m_Wrapper.m_PlayerActionMap_shoot;
        public InputActionMap Get() { return m_Wrapper.m_PlayerActionMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActionMapActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActionMapActions instance)
        {
            if (m_Wrapper.m_PlayerActionMapActionsCallbackInterface != null)
            {
                @right.started -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnRight;
                @right.performed -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnRight;
                @right.canceled -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnRight;
                @left.started -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnLeft;
                @left.performed -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnLeft;
                @left.canceled -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnLeft;
                @shoot.started -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnShoot;
                @shoot.performed -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnShoot;
                @shoot.canceled -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnShoot;
            }
            m_Wrapper.m_PlayerActionMapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @right.started += instance.OnRight;
                @right.performed += instance.OnRight;
                @right.canceled += instance.OnRight;
                @left.started += instance.OnLeft;
                @left.performed += instance.OnLeft;
                @left.canceled += instance.OnLeft;
                @shoot.started += instance.OnShoot;
                @shoot.performed += instance.OnShoot;
                @shoot.canceled += instance.OnShoot;
            }
        }
    }
    public PlayerActionMapActions @PlayerActionMap => new PlayerActionMapActions(this);
    private int m_keyboardSchemeIndex = -1;
    public InputControlScheme keyboardScheme
    {
        get
        {
            if (m_keyboardSchemeIndex == -1) m_keyboardSchemeIndex = asset.FindControlSchemeIndex("keyboard");
            return asset.controlSchemes[m_keyboardSchemeIndex];
        }
    }
    private int m_androidSchemeIndex = -1;
    public InputControlScheme androidScheme
    {
        get
        {
            if (m_androidSchemeIndex == -1) m_androidSchemeIndex = asset.FindControlSchemeIndex("android");
            return asset.controlSchemes[m_androidSchemeIndex];
        }
    }
    public interface IPlayerActionMapActions
    {
        void OnRight(InputAction.CallbackContext context);
        void OnLeft(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
    }
}
