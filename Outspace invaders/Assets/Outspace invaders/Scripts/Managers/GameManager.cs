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
    public static LoadingDelegate OnSceneLoaded;
    private static IEnumerator loadingCorroutine;
    public static bool inLoadingScene = true;
    public static bool inMenu;

    // Level management
    public static readonly float LoseToResetDelay = 0.7f;
    public static bool playing, inPause;
    public delegate void LevelDelegate();
    public static LevelDelegate OnLevelStart, OnLoseGame;
    public delegate void Pausing();
    public static Pausing OnPausing;

    // Saving data
    public delegate void SavingDataDelegate();
    public static SavingDataDelegate OnSaving, OnLoading;


    private void OnEnable()
    {
        OnLevelStart += SetLevel;
        OnLoseGame += LoseGame;
        OnPausing += Pause;
        AudioManager.Enable();
    }
    private void OnDisable()
    {
        AudioManager.Disable();
        OnLevelStart -= SetLevel;
        OnLoseGame -= LoseGame;
        OnPausing -= Pause;
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
        LoadHighScore(); //Use 'ResetSavingData()' instead for restarting the saving data for testing purposes
        EnterScene();
    }
    /// <summary>
    /// Behaviour of the game when minimizing. In this case we open the pause menu.
    /// </summary>
    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus || !playing)
            return;
        if (!inPause)
            OnPausing?.Invoke();
    }


    #region Game Restarting

    /// <summary>
    /// Restart the whole game to the main menu.
    /// </summary>
    public static void EnterScene(int sceneIndex)
    {
        if (loadingCorroutine == null)
        {
            inLoadingScene = true;
            AudioManager.StopLevelSong();
            instance.StartCoroutine(loadingCorroutine = RestartToScene(sceneIndex));
        }
        else
            print("trying to enter a scene but already loading one");
    }
    public static void EnterScene() => EnterScene(SceneManager.GetActiveScene().buildIndex);
    /// <summary> 
    /// Load scene. Go to the main menu except if its the first time calling it, for testing purposes. 
    /// </summary>
    private static IEnumerator RestartToScene(int sceneIndex)
    {
        float minDelay = 0.1f, timer = 0f;
        //OnPausing?.Invoke();

        // Dont re-load the scene if we are just opening the game. It also helps to keep the game in the scene we are about to test in the editor.
        if (firstGameLoadSinceExecution)
        {
            InputsManager.SetInputManager(); // Start the input system
            firstGameLoadSinceExecution = false;
            if (instance.printTransitionStates)
                print("Loading scene... Entering desired scene. (1/2)");
            while (timer < minDelay)
            {
                yield return null;
                timer += Time.deltaTime;
            }
            goto LoadedScene;
        }

        // Load scene
        if (instance.printTransitionStates)
            print("Loading scene... Entering desired scene. (1/2)");
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(sceneIndex);
        timer = 0f;
        while (!loadingOperation.isDone && (timer < minDelay))
        {
            yield return null;
            timer += Time.deltaTime;
        }

        // Start the scene
        LoadedScene:
        OnSceneLoaded?.Invoke();
        if (instance.printTransitionStates)
            print("Loading completed. Scene started! (2/2)");

        if(SceneManager.GetActiveScene().buildIndex > 0)
            OnLevelStart?.Invoke();

        loadingCorroutine = null;
        inLoadingScene = false;
    }
    /// <summary>
    /// Set data needed for playing a specific scene and then play the level start delegate
    /// </summary>
    private static void SetLevel()
    {
        playing = true;
        
    }

    #endregion

    #region Level events: pause and lose

    private static void Pause()
    {
        inPause = !inPause;
        playing = !inPause;

        if (inPause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
    private static void LoseGame()
    {
        playing = false;
        OnPausing?.Invoke();
        instance.StopAllCoroutines();
        SaveHighScore();
        EnterScene(0);
    }

    #endregion

    #region Saving data

    private static readonly string highScoreKey = "highscore";

    public static void SaveHighScore()
    {
        if (Player.score > Player.highScore)
            Player.highScore = Player.score;
        PlayerPrefs.SetInt(highScoreKey, Player.highScore);
    }
    public static void LoadHighScore()
    {
        Player.highScore = PlayerPrefs.GetInt(highScoreKey);
    }

    #endregion

}