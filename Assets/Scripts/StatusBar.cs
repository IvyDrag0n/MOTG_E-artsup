using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    public Image healthBar;

    public Image vigorBar;

    public Image clock;

    public Gradient clockGradient;
    
    private float timeLeft;
    
    public float maxTime;

    private float currentMaxTime;

    public bool isGameOver;
    
    // Start is called before the first frame update
    void Start()
    {
        clock.color =  clockGradient.Evaluate(1f); //initialize clock color
        
        currentMaxTime = maxTime + Time.time; //Set the timer dynamically (will work if the scene is reloaded)

        isGameOver = false;
    }

    // Update is called once per frame
    private void Update()
    {
        //Calculate remaining time and update clock
        timeLeft = currentMaxTime - Time.time;
        SetClock(timeLeft);

        if (timeLeft < 0) //If the time runs out, the game is over
        {
            isGameOver = true;
        }
    }

    public void SetHealth(float health) //Updates the health bar
    {
        healthBar.fillAmount = health / 100f; //value has to be between 0 and 1
    }

    public void SetVigor(float vigor) //Updates vigor bar
    {
        vigorBar.fillAmount = vigor / 100f; //value has to be between 0 and 1
    }
    
    private void SetClock(float remainingTime) //Updates clock display
    {
        clock.fillAmount = remainingTime / maxTime; //Value has to be between 0 and 1
        clock.color = clockGradient.Evaluate(clock.fillAmount); //update color
    }
}
