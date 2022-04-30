using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Timer Class
/// Write the time in the relative text box
/// </summary>
public class Timer : MonoBehaviour {

    private Score scoreManager;
    public Text gameTimerText;
    float gameTimer = 0.0f;


    void Start()
    {
        scoreManager = GetComponent<Score>();
        //Remove point every 10 seconds. Starting in 10 seconds
        InvokeRepeating("CallRemovePoint", 10f, 10f);
    }
    void Update()
    {
        gameTimer += Time.deltaTime;

        int seconds = (int)(gameTimer % 60);
        int minutes = (int)(gameTimer / 60) % 60;
        int hours = (int)(gameTimer / 3600) % 24;

        string timerString = string.Format("{0:0}:{1:00}:{2:00}", hours, minutes, seconds);

        gameTimerText.text = timerString;
    }

    /// <summary>
    /// Remove point 
    /// </summary>
    void CallRemovePoint()
    {
        scoreManager.RemovePoint(2);
    }
}
