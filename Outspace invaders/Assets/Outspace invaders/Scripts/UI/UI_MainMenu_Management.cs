using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MainMenu_Management : UI_MenuManagement
{
    [SerializeField] private GameObject quitMenu, mainMenu;
    private AudioClip buttonAudioClip;
    private readonly string buttonNotFoundMessage = "button attempt failed with the button of name: ";

    void Awake() => buttonAudioClip = Resources.Load<AudioClip>($"Audio/UI_SFX/button");
    void Start()
    {
        if (mainMenu != null)
            openedMenus.Add(mainMenu);
    }

    // - - - - - BUTTON FUNCTIONS - - - - -
    private void FixButtonInputName(ref string buttonName)
    {
        buttonName = buttonName.ToLower();
        buttonName = buttonName.Trim();
        buttonName = buttonName.Replace(" ", "");
    }
    public void MainPanelButtonsActions(string buttonName)
    {
        FixButtonInputName(ref buttonName);

        switch (buttonName)
        {
            case "play":
                print("play button");
                AudioManager.PlayAudio(AudioManager.UI_AudioSource, buttonAudioClip);
                AudioManager.StopLevelSong();
                //GameManager.OnLevelSetUp?.Invoke();
                GameManager.EnterScene(1);
                break;

            case "quit":
                OpenMenu(quitMenu, true);
                break;

            default:
                Debug.Log($"{buttonNotFoundMessage}'{buttonName}'");
                break;
        }
    }
    public void QuitPanelButtonsActions(string buttonName)
    {
        FixButtonInputName(ref buttonName);

        switch (buttonName)
        {
            case "yes":
                print("Quitting application");
                Application.Quit();
                break;

            case "no":
                CloseMenu(true);
                break;

            default:
                print($"{buttonNotFoundMessage}'{buttonName}'");
                break;
        }
    }

}