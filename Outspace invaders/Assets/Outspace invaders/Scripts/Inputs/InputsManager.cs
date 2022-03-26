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

#if UNITY_ANDROID
    private static Vector2 fingerPos = new Vector2();
    private static List<RaycastResult> results = new List<RaycastResult>();
    private static EventSystem eventSystem = EventSystem.current;
#endif


    public static void SetInputManager()
    {
        input = new InputsActionsG();
        SetInputMask();
        GameManager.OnLevelStarted += LevelStart;
        GameManager.OnLoseGame += LoseLevel;
        GameManager.OnPausing += DisablePlayerActionMaps;
        GameManager.OnUnpausing += EnablePlayerActionMaps;
    }

    #region Delegate functions

    private static void LevelStart()
    {
        if (input != null)
            input.PlayerActionMap.Enable();

        // Get player script
        var playersGO = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playersGO.Length; i++)
        {
            Player playerCodeTemp = playersGO[i].GetComponent<Player>();
            if (playerCodeTemp != null)
            {
                playerCode = playerCodeTemp;
                goto PlayerFound;
            }
        }
        Debug.Log("Couldn't find any 'Player' script attached to any gameObjects with tag 'Player'.");
        return;

        PlayerFound:;
        SetPlayerInputsCallbacks();
    }

    private static void LoseLevel() => input.PlayerActionMap.Disable();

    private static void EnablePlayerActionMaps() => input.PlayerActionMap.Enable();

    private static void DisablePlayerActionMaps() => input.PlayerActionMap.Disable();

    #endregion

    #region Input system settings

    private static void SetInputMask()
    {
#if UNITY_STANDALONE
        input.bindingMask = InputBinding.MaskByGroup("keyboard");
#endif

#if UNITY_ANDROID
        input.bindingMask = InputBinding.MaskByGroup("android");
#endif
    }
    
    private static void SetPlayerInputsCallbacks()
    {
#if UNITY_STANDALONE
        //input.PlayingInputs.pause.performed += ctx => GameManager.Pause(true);
        //input.MenuInputs.back.performed += ctx => UI_MenusManagement.CloseMenu();
        if (!playerCode)
            return;

        input.PlayerActionMap.right.started += ctx => playerCode.movingRight = true;
        input.PlayerActionMap.right.canceled += ctx => playerCode.movingRight = false;
        input.PlayerActionMap.left.started += ctx => playerCode.movingLeft = true;
        input.PlayerActionMap.left.canceled += ctx => playerCode.movingLeft = false;
        input.PlayerActionMap.shoot.performed += ctx => playerCode.shootAttempt = true;
#endif
    }

    #endregion

    #region  Input functions

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