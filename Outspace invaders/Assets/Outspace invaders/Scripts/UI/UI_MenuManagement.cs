using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

/// <summary>
/// This class manage the menus settings and transitions. Buttons actions and specific UI behaviour are in other scripts in the UI forlder.
/// </summary>
public class UI_MenusManagement : MonoBehaviour
{
    // Menu data
    [HideInInspector] public enum Menus { none, HUD, levelMessages, pause, mainMenu, loading }
    [SerializeField] private GameObject canvasHUD, canvasLevelMessages, canvasPauseMenu, canvasMainMenu, canvasLoading;
    public static GameObject[] canvasesGameObjects = new GameObject[Enum.GetNames(typeof(Menus)).Length];
    private static List<GameObject> openedMenus = new List<GameObject>();

    // Audio
    [HideInInspector] public enum UI_AudioNames { lose, pause, unPause }
    public static AudioClip[] UI_Clips = new AudioClip[Enum.GetNames(typeof(UI_AudioNames)).Length];


    private void OnEnable()
    {
        GameManager.OnStartingScene += StartingScene;
        GameManager.OnLevelStart += LevelStart;
        GameManager.OnLoseGame += Lose;
        GameManager.OnPausingMenu += EnablePauseMenu;
    }
    private void OnDisable()
    {
        GameManager.OnStartingScene -= StartingScene;
        GameManager.OnLevelStart -= LevelStart;
        GameManager.OnLoseGame -= Lose;
        GameManager.OnPausingMenu -= EnablePauseMenu;
    }

    private void StartingScene()
    {
        // Get components
        string[] uiClipsPaths = { "lose", "pause", "unPause" };
        foreach (UI_AudioNames audioClip in Enum.GetValues(typeof(UI_AudioNames)))
            UI_Clips[(int)audioClip] = Resources.Load<AudioClip>($"Audio/UI_SFX/{uiClipsPaths[(int)audioClip]}");

        // Get game object
        canvasesGameObjects[(int)Menus.HUD] = canvasHUD;
        canvasesGameObjects[(int)Menus.levelMessages] = canvasLevelMessages;
        canvasesGameObjects[(int)Menus.pause] = canvasPauseMenu;
        canvasesGameObjects[(int)Menus.mainMenu] = canvasMainMenu;
        canvasesGameObjects[(int)Menus.loading] = canvasLoading;

        // Reset static variables
        openedMenus.Clear();
        openedMenus.Add(canvasMainMenu);

        // Set gameObjects
        canvasesGameObjects[(int)Menus.mainMenu].SetActive(true);
        canvasesGameObjects[(int)Menus.HUD].SetActive(false);
        canvasesGameObjects[(int)Menus.pause].SetActive(false);
        canvasesGameObjects[(int)Menus.loading].SetActive(false);
    }
    private void LevelStart()
    {
        canvasesGameObjects[(int)Menus.HUD].SetActive(true);
        canvasesGameObjects[(int)Menus.mainMenu].SetActive(false);
        canvasesGameObjects[(int)Menus.loading].SetActive(false);
    }
    private static void Lose() => canvasesGameObjects[(int)Menus.HUD].SetActive(false);
    /// <summary>
    /// Open or closes the pause menu, 
    /// </summary>
    private void EnablePauseMenu(bool enabling)
    {
        if (enabling)
        {
            AudioManager.PlayAudio(AudioManager.UI_AudioSource, UI_Clips[(int)UI_AudioNames.pause]);
            OpenMenu(Menus.pause);
            canvasesGameObjects[(int)Menus.HUD].SetActive(false);
        }
        // Close last menu of the UI. Open previous menu if its the case. Unpause the game if closing the pause menu.
        else
        {
            // Only close an available panel to close
            if (openedMenus.Count < 1)
                return;
            if (openedMenus.Last() == canvasesGameObjects[(int)Menus.mainMenu])
                return;

            // Open previous panel if it's not the main menu panel
            if (openedMenus[openedMenus.Count - 2] != canvasesGameObjects[(int)Menus.mainMenu])
                openedMenus[openedMenus.Count - 2].SetActive(true);

            // Unpause the game and re-open the HUD if closing the pause menu
            if (openedMenus.Last() == canvasesGameObjects[(int)Menus.pause])
            {
                GameManager.Pause(false, false);
                canvasesGameObjects[(int)Menus.HUD].SetActive(true);
            }

            // Close current panel
            openedMenus.Last().SetActive(false);
            openedMenus.Remove(openedMenus.Last());
        }
    }
    /// <summary> Open a new panel menu of the UI. </summary>
    /// <param name="panelToGo"> Enum of the panel of this class for easy use.</param>
    private static void OpenMenu(Menus panelToGo)
    {
        canvasesGameObjects[(int)(panelToGo)].SetActive(true);  // Open new menu
        openedMenus.Last().SetActive(false);                    // Close previous menu
        openedMenus.Add(canvasesGameObjects[(int)(panelToGo)]); // Add new menu to the list of opened menus
    }

}