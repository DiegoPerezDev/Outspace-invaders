using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> This specific 'GameManager' script works for one scene games.
/// <para> - Scene transitions: Scene loading and setting. </para> 
/// <para> - Level behaviour: Pause, win and lose behaviour. </para>
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Transition data
    [SerializeField] private bool printTransitionStates;
    private static bool firstGameLoadSinceExecution = true;
    public delegate void LoadingDelegate();
    public static LoadingDelegate OnStartingScene;
    public enum LoadingStates { none, loadingScene, settingScene }
    public static LoadingStates loadingState;

    // Level management
    public static readonly float LoseToResetDelay = 0.7f;
    public static bool playing, paused;
    public delegate void LevelDelegate();
    public static LevelDelegate OnLevelSetUp, OnLevelStart, OnLoseGame;
    public delegate void Pausing(bool pausing);
    public static Pausing OnPausing, OnPausingMenu;

    // Specific game data
    [HideInInspector] public enum Difficulties { veryEasy, easy, normal, hard, veryHard }
    public static Difficulties difficulty = Difficulties.veryEasy;

    // Saving data
    public delegate void SavingDataDelegate();
    public static SavingDataDelegate OnSaving, OnLoading;
    private static readonly string scoreRecordValKey = "scoreRecordValKey";


    private void OnEnable()
    {
        OnLevelSetUp += SetLevel;
        OnLoseGame += LoseGame;
    }
    private void OnDisable()
    {
        OnLevelSetUp -= SetLevel;
        OnLoseGame -= LoseGame;
    }
    void Awake()
    {
        // Set singleton
        if (instance != null)
            Destroy(transform.gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(transform.gameObject);
        }
    }
    void Start()
    {
        // Load saved data and start the game
        LoadLevelData(); //Use 'ResetSavingData()' instead for restarting the saving data for testing purposes
        RestartGame();
    }
    /// <summary>
    /// Behaviour of the game when minimizing. In this case we open the pause menu.
    /// </summary>
    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus || !playing)
            return;
        if (!paused)
            Pause(true, true);
    }


    #region Game Restarting

    /// <summary>
    /// Restart the whole game to the main menu.
    /// </summary>
    public static void RestartGame()
    {
        if (loadingState == LoadingStates.none)
        {
            loadingState = LoadingStates.loadingScene;
            instance.StartCoroutine(RestartToMainmenu());
        }
    }
    /// <summary> 
    /// Load scene. Go to the main menu except if its the first time calling it, for testing purposes. 
    /// </summary>
    private static IEnumerator RestartToMainmenu()
    {
        // Dont re-load the scene if we are just opening the game. It also helps to keep the game in the scene we are about to test in the editor.
        if (firstGameLoadSinceExecution)
        {
            InputsManager.SetInputManager();
            firstGameLoadSinceExecution = false;
            if (instance.printTransitionStates)
                print("Loading... Entering desired scene. (1/2)");
            goto StartingScene;
        }

        // Load main menu scene
        if (instance.printTransitionStates)
            print("Loading... Entering main menu. (1/2)");
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(0);
        while (!loadingOperation.isDone)
            yield return null;

        // Start the scene
        StartingScene:
        OnStartingScene?.Invoke();
        if (instance.printTransitionStates)
            print("Loading completed. Scene started! (2/2)");

        // Start the game if there is only one scene, because normally it would begin with a main menu button.
        if (SceneManager.sceneCountInBuildSettings < 2)
            OnLevelSetUp?.Invoke();
    }
    /// <summary>
    /// Set data needed for playing a specific scene and then play the level start delegate
    /// </summary>
    private static void SetLevel()
    {
        playing = true;
        difficulty = Difficulties.veryEasy;
        OnLevelStart?.Invoke();
        Pause(false, false);
    }

    #endregion

    #region Level events: pause and lose

    public static void Pause(bool withPanelChanging, bool pausing)
    {
        paused = pausing;
        playing = !pausing;

        if (pausing)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;

        OnPausing?.Invoke(pausing);
        if (withPanelChanging)
            OnPausingMenu?.Invoke(pausing);
    }
    public static void Pause(bool withPanelChanging) => Pause(withPanelChanging, !paused);
    private static void LoseGame()
    {
        playing = false;
        Pause(false, true);
        instance.StopAllCoroutines();
        RestartGame();
    }

    #endregion

    #region Saving data

    public static void SaveLevelData() => OnSaving?.Invoke();
        // PlayerPrefs.SetInt(scoreRecordValKey, Score.record);
    public static void LoadLevelData() => OnLoading?.Invoke(); 
    //Score.record = PlayerPrefs.GetInt(scoreRecordValKey);
    public static void ResetSavingData()
    {
        PlayerPrefs.SetInt(scoreRecordValKey, 0);
        SaveLevelData();
    }

    #endregion

}