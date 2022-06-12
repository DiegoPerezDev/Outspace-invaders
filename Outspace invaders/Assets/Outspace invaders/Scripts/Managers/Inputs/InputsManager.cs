using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

/// <summary>
/// <para> This class manages all of the inputs of the user. </para>
/// <para> This input manager works with Unitys input system, the one that uses 'input actions'. </para>
/// <para> This code separates the inputs actions between menu inputs and player inputs so we can disable them separately. </para>
/// <para> Set the players input callbacks in the player codes. </para>
/// </summary>
public sealed class InputsManager
{
    public delegate void InputsDelegates();
    public static        InputsDelegates OnMenuBackInput;
    public static        InputsActionsG input;
#if UNITY_ANDROID
    private static Vector2 fingerPos           = new Vector2();
    private static List<RaycastResult> results = new List<RaycastResult>();
    private static EventSystem eventSystem     = EventSystem.current;
#endif

    /// <summary>
    /// Start function for this class, kind of a constructor
    /// </summary>
    public static void SetInputManager()
    {
        // Set input action system if there is not set already
        if(input != null)
            return;
        input = new InputsActionsG();
        SetInputMask();
        SetMenuInputsCallbacks();

        // Set code functions for other codes delegates
        GameManager.OnLoseGame   += ChangePlayerActionMaps;
        GameManager.OnPauseByTime    += DisablePlayerActionMaps;
        GameManager.OnLevelPauseByFreezing += DisablePlayerActionMaps;
        GameManager.OnLevelStart += DisableMenuActionMaps;
    }
    public static void EnableMenuActionMaps() => input.MenuActionMap.Enable();
    private static void DisableMenuActionMaps() => input.MenuActionMap.Disable();
    /// <summary>
    /// Change the action maps between the player and the menu
    /// </summary>
    public static void EnablePlayerActionMaps(bool enablePlayer)
    {
        if (!enablePlayer)
        {
            input.PlayerActionMap.Disable();
            input.MenuActionMap.Enable();
        }
        else
        {
            input.MenuActionMap.Disable();
            input.PlayerActionMap.Enable();
        }
    }
    private static void DisablePlayerActionMaps(bool disablePlayer) => EnablePlayerActionMaps(!disablePlayer);

    /// <summary>
    /// Set input mask, the one that tells the game which device we are using for inputs, like an xbox controller for example
    /// </summary>
    private static void SetInputMask()
    {
#if UNITY_STANDALONE
        input.bindingMask = InputBinding.MaskByGroup("keyboard");
#endif

#if UNITY_ANDROID
        input.bindingMask = InputBinding.MaskByGroup("android");
#endif
    }
    /// <summary>
    /// Set the call backs of the input action maps for the menu management, this mean that we tell what to do when performing the inputs for the menu.
    /// </summary>
    private static void SetMenuInputsCallbacks() => input.MenuActionMap.back.performed += ctx => OnMenuBackInput?.Invoke();
    private static void ChangePlayerActionMaps() => EnablePlayerActionMaps(!GameManager.inPause);

}