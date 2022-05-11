using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_HUD : MonoBehaviour
{
    public TextMeshProUGUI scoreTMP;
    [SerializeField] private TextMeshProUGUI highScoreTMP, livesTMP;
    private readonly string scoreMessage = "SCORE:\n", highScoreMessage = "HIGH SCORE:\n", livesMessage = "LIVES:\n";
    private int playerLives;
    private readonly int minRandomScorePoints = 50, maxRandomScorePoints = 500;
    
    private void OnEnable()
    {
        AlienArmy.OnAlienDestroyed += AddScore;
        Player.OnLosingLive += SubstractLive;
        RandomAlien.OnRandomAlienDestroyed += AddRandomScore;
    }
    private void OnDisable()
    {
        AlienArmy.OnAlienDestroyed -= AddScore;
        Player.OnLosingLive -= SubstractLive;
        RandomAlien.OnRandomAlienDestroyed -= AddRandomScore;
    }
    void Start()
    {
        playerLives = Player.startLives - 1;
        scoreTMP.text = $"{scoreMessage}00";
        highScoreTMP.text = $"{highScoreMessage}00";
        livesTMP.text = $"{livesMessage}{playerLives}";
    }

    private void AddScore()
    {
        Player.score += Player.scoreForKill;
        scoreTMP.text = $"{scoreMessage}{Player.score}";
    }
    private void AddRandomScore()
    {
        // Random score is a number between 'min random score point' and 'max random score point', counting 'varietion' by 'variation'
        var variation = 50;
        var scoreToAdd = (Random.Range((minRandomScorePoints - variation) / variation, maxRandomScorePoints/ variation) + variation) * variation;
        Player.score += scoreToAdd;
        scoreTMP.text = $"{scoreMessage}{Player.score}";
    }
    private void SubstractLive()
    {
        playerLives--;
        if(playerLives >= 0)
            livesTMP.text = $"{livesMessage}{Player.lives - 1}";
    }

}