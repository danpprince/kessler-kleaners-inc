// GENERATED AUTOMATICALLY FROM 'Assets/Katamari_Input.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Katamari_Input : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Katamari_Input()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Katamari_Input"",
    ""maps"": [
        {
            ""name"": ""Mode"",
            ""id"": ""640161e3-6921-414e-b773-a74487af29b6"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""00dedc90-0cee-42f3-aa2f-5156e094018e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""446a1936-b5d0-45a5-8eec-058152cd5cad"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1022ed61-1075-4ff0-82e8-c983baa6b3ae"",
                    ""path"": ""<Keyboard>/m"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Mode
        m_Mode = asset.FindActionMap("Mode", throwIfNotFound: true);
        m_Mode_Newaction = m_Mode.FindAction("New action", throwIfNotFound: true);
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

    // Mode
    private readonly InputActionMap m_Mode;
    private IModeActions m_ModeActionsCallbackInterface;
    private readonly InputAction m_Mode_Newaction;
    public struct ModeActions
    {
        private @Katamari_Input m_Wrapper;
        public ModeActions(@Katamari_Input wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_Mode_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_Mode; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ModeActions set) { return set.Get(); }
        public void SetCallbacks(IModeActions instance)
        {
            if (m_Wrapper.m_ModeActionsCallbackInterface != null)
            {
                @Newaction.started -= m_Wrapper.m_ModeActionsCallbackInterface.OnNewaction;
                @Newaction.performed -= m_Wrapper.m_ModeActionsCallbackInterface.OnNewaction;
                @Newaction.canceled -= m_Wrapper.m_ModeActionsCallbackInterface.OnNewaction;
            }
            m_Wrapper.m_ModeActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Newaction.started += instance.OnNewaction;
                @Newaction.performed += instance.OnNewaction;
                @Newaction.canceled += instance.OnNewaction;
            }
        }
    }
    public ModeActions @Mode => new ModeActions(this);
    public interface IModeActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
}
