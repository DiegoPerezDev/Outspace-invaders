using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// <para> This class manages all of the inputs of the player and take action for it by calling other codes methods. </para>
/// <para> This input manager works with Unitys input system, the one that uses 'input actions'. </para>
/// <para> This code separates the inputs actions between menu inputs and player inputs so we can disable them separately. </para>
/// </summary>
public sealed class InputsManager
{
    public static InputsActionsG input;
    private static Player playerCode;
    public delegate void InputsDelegates();
    public static InputsDelegates MenuBackInput, pauseGameInput;

#if UNITY_ANDROID
    private static Vector2 fingerPos = new Vector2();
    private static List<RaycastResult> results = new List<RaycastResult>();
    private static EventSystem eventSystem = EventSystem.current;
#endif


    /// <summary>
    /// Start function of this class, kind of a constructor
    /// </summary>
    public static void SetInputManager()
    {
        input = new InputsActionsG();
        SetInputMask();
        GameManager.OnLevelStart += LevelStart;
        GameManager.OnLoseGame += LoseLevel;
        GameManager.OnPausing += ChangePlayerActionMaps;
    }
    /// <summary>
    /// Set input mask, the one that tells the game which device we are using for inputs, xbox controller for example
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

    #region Player input actions functions

    /// <summary>
    /// Set the call backs of the input action maps, this mean that we tell what to do when performing the inputs.
    /// </summary>
    private static void SetInputsCallbacks()
    {
#if UNITY_STANDALONE
        input.MenuActionMap.back.performed += ctx => MenuBackInput?.Invoke();

        input.PlayerActionMap.pause.performed += ctx => pauseGameInput?.Invoke();
        input.PlayerActionMap.right.started += ctx => playerCode.movingRight = true;
        input.PlayerActionMap.right.canceled += ctx => playerCode.movingRight = false;
        input.PlayerActionMap.left.started += ctx => playerCode.movingLeft = true;
        input.PlayerActionMap.left.canceled += ctx => playerCode.movingLeft = false;
        input.PlayerActionMap.shoot.performed += ctx => playerCode.shootAttempt = true;
#endif
    }
    /// <summary>
    /// Enable or disable the action maps that manages the player itself, not the menu
    /// </summary>
    private static void ChangePlayerActionMaps()
    {
        if (GameManager.inPause)
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
    public static void EnablePlayerActionMaps(bool enable)
    {
        if (!enable)
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

    #endregion

    #region  Level functions

    /// <summary>
    /// Set data and get components needed from a level scene at the start of this one
    /// </summary>
    private static void LevelStart()
    {
        if (input != null)
            input.PlayerActionMap.Enable();

        // Get player script
        var playersGO = GameObject.FindGameObjectsWithTag("Player");
        if (playersGO.Length == 1)
        {
            Player playerCodeTemp = playersGO[0].GetComponent<Player>();
            if (playerCodeTemp != null)
            {
                playerCode = playerCodeTemp;
                SetInputsCallbacks();
            }
            else
                Debug.Log("Couldn't find any 'Player' script attached to the player gameObject.");
        }
        else
            Debug.Log("More than one gameObject with the tag 'Player'.");
    }
    /// <summary>
    /// Set the inputs for when the user loses the game. Usually just disable the players action maps.
    /// </summary>
    private static void LoseLevel() => ChangePlayerActionMaps();

    /*
    private static void CheckIfTapOnButton()
    {
#if UNITY_STANDALONE
        if (!EventSystem.current.IsPointerOverGameObject())
            player.Jump();
#endif
#if UNITY_ANDROID
        fingerPos = input.PlayingInputs.fingerPos.ReadValue<Vector2>();
        if (!IsPointerOverUIObject())
            player.Jump();
#endif
    }

#if UNITY_ANDROID
    private static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(eventSystem);
        eventDataCurrentPosition.position = new Vector2(fingerPos.x, fingerPos.y);
        results.Clear();
        eventSystem.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
#endif
    */

    #endregion

}