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
    
    private void OnEnable()
    {
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
    private void SubstractLive()
    {
        playerLives--;
        if(playerLives >= 0)
            livesTMP.text = $"{livesMessage}{Player.lives - 1}";
    }

}