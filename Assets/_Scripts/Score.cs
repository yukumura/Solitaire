using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Score class
/// </summary>
public class Score : MonoBehaviour {       

    public Text gameScoreText;
    private int gameScore = 0;

    void Start()
    {
        gameScoreText.text = string.Format("{0}", gameScore.ToString());
    }

    void Update()
    {        
    }

    /// <summary>
    /// Add score point
    /// </summary>
    /// <param name="point"></param>
    public void AddPoint(int point)
    {
        gameScore += point;
        gameScoreText.text = string.Format("{0}", gameScore.ToString());
    }

    /// <summary>
    /// Remove score point
    /// </summary>
    /// <param name="point"></param>
    public void RemovePoint(int point)
    {
        if (gameScore - point < 0)
            return;

        gameScore -= point;
        gameScoreText.text = string.Format("{0}", gameScore.ToString());
    }

    /// <summary>
    /// Reset score point
    /// </summary>
    public void ResetPoint()
    {
        gameScore = 0;
        gameScoreText.text = string.Format("{0}", gameScore.ToString());
    }

}
