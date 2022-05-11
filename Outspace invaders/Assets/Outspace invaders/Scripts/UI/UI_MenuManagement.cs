using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class UI_MenuManagement : MonoBehaviour
{
    protected static List<GameObject> openedMenus = new List<GameObject>();
    private static GameObject lastClosedMenu;

    /// <summary> Open a new panel menu of the UI. </summary>
    /// <param name="panelToGo"> Enum of the panel of this class for easy use.</param>
    protected static void OpenMenu(GameObject menuToOpen, bool closeCurrentMenu)
    {
        menuToOpen.SetActive(true);
        if (closeCurrentMenu && openedMenus.Count > 0)
        {
            lastClosedMenu = openedMenus.Last();
            openedMenus.Last().SetActive(false);
        }
        openedMenus.Add(menuToOpen);             
    }
    protected static void CloseMenu(bool ReopenLastClosedMenu)
    {
        // Only close an available panel to close
        if (openedMenus.Count < 1)
            return;

        //if (openedMenus.Last() == canvasesGameObjects[(int)Menus.mainMenu])
        //    return;

        // Open previous panel if it's not the main menu panel
        //if (openedMenus[openedMenus.Count - 2] != canvasesGameObjects[(int)Menus.mainMenu])
        //    openedMenus[openedMenus.Count - 2].SetActive(true);

        // Unpause the game and re-open the HUD if closing the pause menu
        //if (openedMenus.Last() == canvasesGameObjects[(int)Menus.pause])
        //{
        //    GameManager.Pause(false, false);
        //    canvasesGameObjects[(int)Menus.HUD].SetActive(true);
        //}

        // Close current panel
        openedMenus.Last().SetActive(false);
        openedMenus.Remove(openedMenus.Last());

        // Re-open last closed menu
        if (ReopenLastClosedMenu)
        {
            lastClosedMenu.SetActive(true);
            openedMenus.Add(lastClosedMenu);
        }

        lastClosedMenu = openedMenus.Last();
    }

}