using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// Manager of the menus for the levels. There are other codes for other scenes like the main menu.
/// </summary>
public class LevelMenu_Management : MenuManagement
{
    [SerializeField] private GameObject HUD, pauseMainMenu, loseCanvas, pauseQuitMenu, winCanvas;
    private static LevelMenu_Management instance;

    // Audio
    [HideInInspector] public enum UI_AudioNames { lose, pause, unPause }
    public static AudioClip[] UI_Clips = new AudioClip[Enum.GetNames(typeof(UI_AudioNames)).Length];


    public override void OnEnable()
    {
        base.OnEnable();
        instance = this;
        GameManager.OnSceneLoaded += StartingScene;
        GameManager.OnLoseGame    += Lose;
        GameManager.OnWinGame     += Win;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        GameManager.OnSceneLoaded -= StartingScene;
        GameManager.OnLoseGame    -= Lose;
        GameManager.OnWinGame     -= Win;
    }
    public override void Start()
    {
        base.Start();
        // Get components
        string[] uiClipsPaths = { "lose", "pause", "unPause" };
        foreach (UI_AudioNames audioClip in Enum.GetValues(typeof(UI_AudioNames)))
            UI_Clips[(int)audioClip] = Resources.Load<AudioClip>($"Audio/UI_SFX/{uiClipsPaths[(int)audioClip]}");
    }

    private void StartingScene()
    {
        // Close menus that are opened by default
        CloseMenu(pauseQuitMenu);
        CloseMenu(pauseMainMenu);
        // Close loading screen
        CloseMenu(false);
    }
    private void Lose()
    {
        OpenMenu(loseCanvas, true);
        CloseMenu(HUD);
    }
    private void Win()
    {
        OpenMenu(winCanvas, true);
        CloseMenu(HUD);
    }
    public override void CheckForMenuClosing()
    {
        if (openedMenus.Count > 0)
        {
            if (openedMenus.Last() == pauseMainMenu)
            {
                CloseMenu(false);
                GameManager.OnPauseByTime?.Invoke(false);
            }
            else
                base.CheckForMenuClosing();
        }
    }

    public static void PauseButtonPressed()
    {
        if (openedMenus.Count < 1)
        {
            //AudioManager.PlayAudio(AudioManager.UI_AudioSource, UI_Clips[(int)UI_AudioNames.pause]);
            OpenMenu(instance.pauseMainMenu, false);
            GameManager.OnPauseByTime?.Invoke(true);
        }
    }

    // - - - - - BUTTON FUNCTIONS - - - - - 

    public void MainPausePanelButtonsActions(string buttonName)
    {
        FixButtonInputName(ref buttonName);

        switch (buttonName)
        {
            case "resume":
                CloseMenu(true);
                GameManager.OnPauseByTime?.Invoke(false);
                break;

            case "mainmenu":
                //AudioManager.PlayAudio(AudioManager.UI_AudioSource, buttonAudioClip);
                GameManager.SaveHighScore();
                GameManager.OnPauseByTime?.Invoke(false);
                GameManager.EnterScene(0);
                break;

            case "quit":
                OpenMenu(pauseQuitMenu, true);
                break;

            default:
                Debug.Log($"{buttonNotFoundMessage}'{buttonName}'");
                break;
        }
    }
    public void PausePanelQuitButtonsActions(string buttonName)
    {
        FixButtonInputName(ref buttonName);

        switch (buttonName)
        {
            case "yes":
                print("Closing game");
                GameManager.SaveHighScore();
                Application.Quit();
                break;

            case "no":
                CloseMenu(true);
                break;

            default:
                Debug.Log($"{buttonNotFoundMessage}'{buttonName}'");
                break;
        }
    }

}
