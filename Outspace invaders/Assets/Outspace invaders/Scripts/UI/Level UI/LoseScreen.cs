using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Behaviour of the game when the player get to the lose screen.
/// </summary>
public class LoseScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loseScore, loseHighScore;
    private readonly string scoreMessage = "SCORE:", highScoreMessage = "HIGHSCORE:";
    private IEnumerator countdownToMainMenuCor;


    private void OnEnable()
    {
        if (!GameManager.inLoadingScene)
        {
            SetScore();
            GoBackToMainMenuDelay();
        }
        else
            gameObject.SetActive(false);
    }

    private void SetScore()
    {
        loseScore.text = $"{scoreMessage} {Player.score}";
        loseHighScore.text = $"{highScoreMessage} {Player.highScore}";
    }
    private void GoBackToMainMenuDelay()
    {
        if (countdownToMainMenuCor != null)
            StopCoroutine(countdownToMainMenuCor);
        countdownToMainMenuCor = CountdownToMainMenu();
        StartCoroutine(countdownToMainMenuCor);
    }
    private IEnumerator CountdownToMainMenu()
    {
        yield return new WaitForSecondsRealtime(4f);
        GameManager.EnterScene(0);
    }

}