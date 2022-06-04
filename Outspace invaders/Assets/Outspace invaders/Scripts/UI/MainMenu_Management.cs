using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu_Management : MenuManagement
{
    [SerializeField] private GameObject quitMenu, mainMenu;
    [SerializeField] private TextMeshProUGUI highScoreTmp;
    private readonly string highScoreMessage = "HIGH SCORE:";

    // - - - - MONOBEHAVIOUR METHODS - - - -
    void Awake() => buttonAudioClip = Resources.Load<AudioClip>($"Audio/UI_SFX/button");
    void Start()
    {
        //openedMenus = new List<GameObject>();
        openedMenus.Add(mainMenu);
        openedMenus.Add(loadingScreen);
    }
    public override void OnEnable()
    {
        base.OnEnable();
        GameManager.OnSceneLoaded += StartMainMenu;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        GameManager.OnSceneLoaded -= StartMainMenu;
    }   

    // - - - - START FUNCTIONS - - - -
    private void StartMainMenu()
    {
        // Hide loading panel
        CloseMenu(false);
        // Show score
        highScoreTmp.text = $"{highScoreMessage} {Player.highScore}";
    }

    // - - - - - BUTTON FUNCTIONS - - - - -
    public void MainPanelButtonsActions(string buttonName)
    {
        FixButtonInputName(ref buttonName);

        switch (buttonName)
        {
            case "play":
                AudioManager.PlayAudio(AudioManager.UI_AudioSource, buttonAudioClip);
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