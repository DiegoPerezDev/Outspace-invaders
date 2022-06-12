using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This codes manages the scene transition, the data saving and loading, the pausing and the gameplay delegates.
/// </summary>
public class GameManager : MonoBehaviour
{
    // General use
    public delegate void ManagerDelegate();
    public delegate void PauseDelegate(bool pausing);
    public static GameManager instance;
    public static PauseDelegate OnPauseByTime;
    public static bool inPause;

    // Scenes transition
    public static ManagerDelegate OnSceneLoaded;
    public static bool inLoadingScene = true, inMenu;
    private static bool firstGameLoadSinceExecution = true;
    private static IEnumerator loadingCorroutine;
    [SerializeField] private bool printTransitionStates;

    // Level gameplay management
    public delegate void LevelDelegate();
    public delegate void LevelPauseDelegate(bool pausing);
    public static LevelDelegate OnLevelStart, OnLoseGame, OnWinGame;
    public static LevelPauseDelegate OnLevelPauseByFreezing;

    // Data saving
    public delegate void SavingDataDelegate();
    public static SavingDataDelegate OnSaving, OnLoading;
    private static readonly string highScoreKey = "highscore";

    #region MonoBehaviour methods

    private void OnEnable()
    {
        OnLoseGame += LoseGame;
        OnWinGame += WinGame;
        OnPauseByTime += PauseByTime;
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
        LoadHighScore();
        InputsManager.SetInputManager();
        RestartScene();
    }
#if !UNITY_EDITOR
    void OnApplicationPause(bool pauseStatus)
    {
        // Pause the game when minimizing.
        if (!pauseStatus || inPause)
            return;
        OnPauseByTime?.Invoke(!inPause);
    }
#endif

    #endregion

    #region Scene transitioning

    /// <summary>
    /// Go to a specific scene. '0' should be the main menu and each other number are the respective number of the levels.
    /// </summary>
    public static void EnterScene(int sceneIndex)
    {
        if (loadingCorroutine == null)
        {
            inLoadingScene = true;
            //AudioManager.StopLevelSong();
            instance.StartCoroutine(loadingCorroutine = StartScene(sceneIndex));
        }
        else
            print("trying to enter a scene but already loading one");
    }
    private static void EnterScene(int sceneIndex, float delay) => instance.StartCoroutine(EnterSceneAfterDelay(sceneIndex, delay));
    private static IEnumerator EnterSceneAfterDelay(int sceneIndex, float delay)
    {
        delay = delay > 0 ? 1 : delay;
        yield return new WaitForSecondsRealtime(delay);
        EnterScene(sceneIndex);
    }
    public static void RestartScene() => EnterScene(SceneManager.GetActiveScene().buildIndex);
    private static IEnumerator StartScene(int sceneIndex)
    {
        float minDelay = 0.1f, timer = 0f;

        // First time entering the scene:
        //  - Dont re-load the scene if we are just opening the game.
        //  - It also helps to keep the game in the scene we are about to test when we are using the editor.
        if (firstGameLoadSinceExecution)
        {
             // Start the input system
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
        else
            InputsManager.EnableMenuActionMaps();

        loadingCorroutine = null;
        inLoadingScene = false;
    }

#endregion

#region Methods for delegates

    private static void PauseByTime(bool pausing)
    {
        inPause = pausing;
        if (inPause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
    private static void LoseGame()
    {
        //OnPauseByTime?.Invoke();
        OnLevelPauseByFreezing?.Invoke(true);
        instance.StopAllCoroutines();
        SaveHighScore();
        EnterScene(0, 3f);
    }
    private static void WinGame()
    {
        OnLevelPauseByFreezing?.Invoke(true);
        instance.StopAllCoroutines();
        SaveHighScore();
        EnterScene(0, 3f);
    }

#endregion

#region Data save and load

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