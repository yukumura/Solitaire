using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class for load level and exit from the game
/// </summary>
public class LevelLoader : MonoBehaviour {

    public void LoadLevel(string nameLevel)
    {
        SceneManager.LoadScene(nameLevel);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Restart the current level
    /// </summary>
    public void RestartLevel()
    {
        FindObjectOfType<DetectCondition>().ResumeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
