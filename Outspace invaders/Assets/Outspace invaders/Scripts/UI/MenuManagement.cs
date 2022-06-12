using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Base class of the menu managers in the game. There are menu managers for each game scene type, like for the levels and the main menu.
/// <para> Mainly, it manages the opening and closing of the menus. These menus are filled in a list for the 'returning to previous menu' functionality.</para>
/// </summary>
public class MenuManagement : MonoBehaviour
{
    protected static List<GameObject> openedMenus = new List<GameObject>();
    [SerializeField] protected GameObject loadingScreen;
    protected  AudioClip buttonAudioClip;
    protected static string buttonNotFoundMessage = "button attempt failed with the button of name: ";


    // - - - - - MONOBEHAVIOUR METHODS - - - - -
    public virtual void OnEnable() => InputsManager.OnMenuBackInput += CheckForMenuClosing;
    public virtual void OnDisable() => InputsManager.OnMenuBackInput -= CheckForMenuClosing;
    public virtual void Start()
    {
        openedMenus = new List<GameObject>();
        OpenMenu(loadingScreen, false);
    }

    // - - - - - MENU MANAGEMENT METHODS - - - - -
    /// <summary> Open a new panel menu of the UI. </summary>
    /// <param name="panelToGo"> Enum of the panel of this class for easy use.</param>
    protected static void OpenMenu(GameObject menuToOpen, bool closeCurrentMenu)
    {
        if (menuToOpen == null)
        {
            print("GameObject of the menu to open was not found.");
            return;
        }

        GameManager.inMenu = true;
        if (!menuToOpen.activeInHierarchy)
            menuToOpen.SetActive(true);
        if (closeCurrentMenu && openedMenus.Count > 0 && (openedMenus.Last() != null))
            openedMenus.Last().SetActive(false);
        openedMenus.Add(menuToOpen);             
    }
    protected static void CloseMenu(bool ReopenLastClosedMenu)
    {
        // Only close an available panel to close
        if (openedMenus.Count < 1)
        {
            print("There was no menu to close");
            return;
        }

        // Close current panel
        openedMenus.Last().SetActive(false);
        openedMenus.Remove(openedMenus.Last());

        // Re-open last closed menu
        if (openedMenus.Count > 0) 
        {
            if (ReopenLastClosedMenu)
                openedMenus.Last().SetActive(true);
        } 
        else
            GameManager.inMenu = false;
    }
    protected static void CloseMenu(GameObject menuToClose)
    {
        if (menuToClose == null)
        {
            print("GameObject of the menu to close was not found.");
            return;
        }

        if (menuToClose.activeInHierarchy)
            menuToClose.SetActive(false);
        if(openedMenus.Count > 0)
        {
            if (openedMenus.Last() == menuToClose)
                openedMenus.Remove(openedMenus.Last());
        }
        else
            GameManager.inMenu = false;
    }
    public virtual void CheckForMenuClosing()
    {
        if (GameManager.inMenu)
            CloseMenu(true);
    }

    // - - - - - BUTTON METHODS - - - - -
    protected void FixButtonInputName(ref string buttonName)
    {
        buttonName = buttonName.ToLower();
        buttonName = buttonName.Trim();
        buttonName = buttonName.Replace(" ", "");
    }

}