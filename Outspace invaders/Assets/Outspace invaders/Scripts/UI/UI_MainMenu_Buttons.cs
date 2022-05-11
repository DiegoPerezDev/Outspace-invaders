using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MainMenu_Buttons : MonoBehaviour
{
    private AudioClip buttonAudioClip;
    private readonly string buttonNotFoundMessage = "button attempt failed with the button of name: ";

    void Awake() => buttonAudioClip = Resources.Load<AudioClip>($"Audio/UI_SFX/button");

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
                AudioManager.PlayAudio(AudioManager.UI_AudioSource, buttonAudioClip);
                AudioManager.StopLevelSong();
                GameManager.OnLevelSetUp?.Invoke();
                break;

            case "quit":
                print("falta esto");
                //UI_MenuManagement.OpenMenu();
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
                //UI_MenuManagement.CloseMenu();
                break;

            default:
                print($"{buttonNotFoundMessage}'{buttonName}'");
                break;
        }
    }


}