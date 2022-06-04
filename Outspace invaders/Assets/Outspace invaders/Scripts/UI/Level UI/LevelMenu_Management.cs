using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class LevelMenu_Management : MenuManagement
{
    // Menu data
    [SerializeField] private GameObject HUD, pauseMainMenu, loseCanvas, pauseQuitMenu;

    // Audio
    [HideInInspector] public enum UI_AudioNames { lose, pause, unPause }
    public static AudioClip[] UI_Clips = new AudioClip[Enum.GetNames(typeof(UI_AudioNames)).Length];


    public override void OnEnable()
    {
        base.OnEnable();
        GameManager.OnSceneLoaded += StartingScene;
        GameManager.OnLoseGame += Lose;
        InputsManager.pauseGameInput += PauseButtonPressed;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        GameManager.OnSceneLoaded -= StartingScene;
        GameManager.OnLoseGame -= Lose;
        InputsManager.pauseGameInput -= PauseButtonPressed;
    }

    private void StartingScene()
    {
        OpenMenu(loadingScreen, false);

        // Get components
        string[] uiClipsPaths = { "lose", "pause", "unPause" };
        foreach (UI_AudioNames audioClip in Enum.GetValues(typeof(UI_AudioNames)))
            UI_Clips[(int)audioClip] = Resources.Load<AudioClip>($"Audio/UI_SFX/{uiClipsPaths[(int)audioClip]}");

        // Set menus
        CloseMenu(pauseMainMenu);
        CloseMenu(pauseQuitMenu);
        CloseMenu(false);
    }
    private void Lose()
    {
        OpenMenu(loseCanvas, true);
        CloseMenu(HUD);
    }
    public override void CheckForMenuClosing()
    {
        if (openedMenus.Count > 0)
        {
            if (openedMenus.Last() == pauseMainMenu)
            {
                CloseMenu(false);
                GameManager.OnPausing?.Invoke();
            }
            else
                base.CheckForMenuClosing();
        }
    }

    private void PauseButtonPressed()
    {
        if (openedMenus.Count < 1)
        {
            AudioManager.PlayAudio(AudioManager.UI_AudioSource, UI_Clips[(int)UI_AudioNames.pause]);
            OpenMenu(pauseMainMenu, false);
                GameManager.OnPausing?.Invoke();
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
                GameManager.OnPausing?.Invoke();
                break;

            case "mainmenu":
                AudioManager.PlayAudio(AudioManager.UI_AudioSource, buttonAudioClip);
                GameManager.SaveHighScore();
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
