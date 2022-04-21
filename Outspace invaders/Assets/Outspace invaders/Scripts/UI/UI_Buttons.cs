using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Buttons : MonoBehaviour
{
    private AudioClip ButtonAudioClip;

    void Awake() => ButtonAudioClip = Resources.Load<AudioClip>($"Audio/UI_SFX/button");

    public void AssignButtonsActions(string button)
    {
        button = button.ToLower();
        button = button.Trim();
        button = button.Replace(" ", "");

        switch (button)
        {
            case "play":
                AudioManager.PlayAudio(AudioManager.UI_AudioSource, ButtonAudioClip);
                AudioManager.StopLevelSong();
                GameManager.OnLevelSetUp?.Invoke();
                break;

            case "continueplaying":
            case "pause":
                GameManager.Pause(true);
                break;

            case "mainmenu":
                GameManager.RestartGame();
                break;

            default:
                print($"button attempt failed with the name: {button}");
                break;
        }
    }

}