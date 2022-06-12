using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI scoreTMP;
    [SerializeField] private TextMeshProUGUI highScoreTMP, livesTMP;
    private readonly string scoreMessage = "SCORE:\n", highScoreMessage = "HIGH SCORE:\n", livesMessage = "LIVES:\n";
    private int playerLives;
    private static readonly int minRandomScorePoints = 50, maxRandomScorePoints = 300;
    private static int randomScoreIntervalVariation;
    private static HUD instance;
    
    private void OnEnable()
    {
        instance = this;   
        AlienArmy.OnAlienDestroyed += AddScore;
        Player.OnLosingLive += SubstractLive;
    }
    private void OnDisable()
    {
        AlienArmy.OnAlienDestroyed -= AddScore;
        Player.OnLosingLive -= SubstractLive;
    }
    void Start()
    {
        randomScoreIntervalVariation = maxRandomScorePoints / minRandomScorePoints;
        playerLives = Player.startLives;
        scoreTMP.text = $"{scoreMessage}{Player.score}";
        highScoreTMP.text = $"{highScoreMessage}{Player.highScore}";
        livesTMP.text = $"{livesMessage}{playerLives - 1}";
    }

    private void AddScore()
    {
        Player.score += Player.scoreForKill;
        scoreTMP.text = $"{scoreMessage}{Player.score}";
    }
    public static void AddRandomScore()
    {
        // Random score is a number between 'min random score point' and 'max random score point' and its internal numbers
        var randomScoreToAdd = Map(Random.Range(1, randomScoreIntervalVariation), 1, randomScoreIntervalVariation, minRandomScorePoints, maxRandomScorePoints);
        Player.score += randomScoreToAdd;
        instance.scoreTMP.text = $"{instance.scoreMessage}{Player.score}";
    }
    private void SubstractLive()
    {
        playerLives--;
        if(playerLives >= 0)
            livesTMP.text = $"{livesMessage}{Player.lives - 1}";
    }
    private static int Map(int x, int in_min, int in_max, int out_min, int out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

}