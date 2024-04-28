using System;
using UnityEngine;
using UnityEngine.UI;
public class scoreCounter : MonoBehaviour
{
    public PlayerController player;
    
    public Text scoreText;
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateScore(player.score);
    }

    public void UpdateScore(int score) //To update the score, can be called by other scripts
    {
        scoreText.text = "SCORE : " + score;
    }
}