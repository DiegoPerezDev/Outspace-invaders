using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_TemporalMessages : MonoBehaviour
{
    public static UI_TemporalMessages instance;
    public static TextMeshProUGUI newRecordTMP, levelMessageTMP;
    [SerializeField] private TextMeshProUGUI newRecordText, levelMessageText;
    private static IEnumerator newRecordMessageCoroutine, newDifficultyMessageCoroutine;
    public static string levelDifficultyMessage = "Very Easy";
    public static readonly float levelIndicatorDuration = 2f, newRecordAnimationDuration = 2f;


    void Awake()
    {
        instance = this;

        // Get components
        levelMessageTMP = levelMessageText;
        newRecordTMP = newRecordText;

        // Set objects
        levelMessageTMP.gameObject.SetActive(false);
        newRecordTMP.gameObject.SetActive(false);
    }
    void OnEnable()
    {
        GameManager.OnLevelStarted += StartMessageShortShow;
        GameManager.OnLoseGame += SetMessagestAtLose;
    }
    private void OnDisable()
    {
        GameManager.OnLevelStarted -= StartMessageShortShow;
        GameManager.OnLoseGame -= SetMessagestAtLose;
    }


    // - - Delegate functions - -
    private void StartMessageShortShow() => instance.StartCoroutine(LevelDifficultyMessageAtLevelStart());
    private static void SetMessagestAtLose()
    {
        // Stop and close new difficulty message
        if (newDifficultyMessageCoroutine != null)
            instance.StopCoroutine(newDifficultyMessageCoroutine);
        levelMessageTMP.gameObject.SetActive(false);

        // Stop and close new record message
        if (newRecordMessageCoroutine != null)
            instance.StopCoroutine(newRecordMessageCoroutine);
        newRecordTMP.gameObject.SetActive(false);

        // Do the record animation again if we break a record in this match
        /*
        if (Score.newRecord)
        {
            newRecordMessageCoroutine = NewRecordMessageAnimation();
            instance.StartCoroutine(newRecordMessageCoroutine);
        }
        */
    }

    // - - Public methods - - 
    public static void ShowNewRecordMessage()
    {
        if (newRecordMessageCoroutine != null)
        {
            instance.StopCoroutine(newRecordMessageCoroutine);
            newRecordTMP.gameObject.SetActive(false);
        }
        newRecordMessageCoroutine = NewRecordMessageAnimation();
        instance.StartCoroutine(newRecordMessageCoroutine);
    }
    public static void ShowNewLevelDifficulty()
    {
        if (newDifficultyMessageCoroutine != null)
            instance.StopCoroutine(newDifficultyMessageCoroutine);
        newDifficultyMessageCoroutine = NewDifficultyMessage();
        instance.StartCoroutine(newDifficultyMessageCoroutine);
    }

    // - - - Corroutines - - -
    /// <summary> Shows a mmesage of the currend difficulty at the beggining of the level, for one second. </summary>
    private static IEnumerator LevelDifficultyMessageAtLevelStart()
    {
        levelMessageTMP.gameObject.SetActive(true);
        var timer = 0f;
        while (timer < 1f)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        levelMessageTMP.gameObject.SetActive(false);
    }
    /// <summary>  New record blinking animation in the GUI. </summary>
    private static IEnumerator NewRecordMessageAnimation()
    {
        newRecordTMP.gameObject.SetActive(true);

        var timer1 = 0f;
        var delay = 0.025f;
        var blinkingDuration = 0.25f;
        while (timer1 < newRecordAnimationDuration)
        {
            var timer2 = 0f;
            while (timer2 < blinkingDuration)
            {
                yield return new WaitForSecondsRealtime(delay);
                timer1 += delay;
                timer2 += delay;
            }
            newRecordTMP.gameObject.SetActive(!newRecordTMP.gameObject.activeInHierarchy);
        }

        newRecordTMP.gameObject.SetActive(false);
    }
    /// <summary> Temporally shows the difficulty of the level.</summary>
    /// <param name="message"> Message to display in the text. </param>
    private static IEnumerator NewDifficultyMessage()
    {
        levelMessageTMP.gameObject.SetActive(true);
        levelMessageTMP.text = levelDifficultyMessage;
        var timer = 0f;
        while (timer < levelIndicatorDuration)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        levelMessageTMP.gameObject.SetActive(false);
    }

}