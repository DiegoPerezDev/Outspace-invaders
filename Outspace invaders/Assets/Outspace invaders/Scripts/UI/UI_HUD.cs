using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_HUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreTMP, highScoreTMP, livesTMP;
    private readonly string scoreMessage = "SCORE:\n", highScoreMessage = "HIGH SCORE:\n", livesMessage = "LIVES:\n";
    
    private void OnEnable()
    {
        Player.OnGatheringScore += AddScore;
        Player.OnLosingLive += SubstractLive;
    }
    private void OnDisable()
    {
        Player.OnGatheringScore -= AddScore;
        Player.OnLosingLive -= SubstractLive;
    }
    void Start()
    {
        scoreTMP.text = $"{scoreMessage}00";
        highScoreTMP.text = $"{highScoreMessage}00";
        livesTMP.text = $"{livesMessage}{Player.startLives - 1}";
    }
    void Update()
    {
        
    }

    private void AddScore()
    {
        Player.score += Player.scoreForKill;
        scoreTMP.text = $"{scoreMessage}{Player.score}";
    }
    private void SubstractLive()
    {
        Player.lives--;
        if(Player.lives >= 0)
            livesTMP.text = $"{livesMessage}{Player.lives - 1}";
    }

}