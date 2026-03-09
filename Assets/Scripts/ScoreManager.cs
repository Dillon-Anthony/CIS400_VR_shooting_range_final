using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;

    private int currentScore = 0;
    private int highScore = 0;

    void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        ResetScore(); // Reset on scene start
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreDisplay();
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }
    }

    public void ShowFinalScore()
    {
        // Check for new high score
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + currentScore + "\nHigh Score: " + highScore;
        }
        else if (scoreText != null)
        {
            scoreText.text = "Final Score: " + currentScore + "\nHigh Score: " + highScore;
        }

        Debug.Log("Wave Complete! Score: " + currentScore + " | High Score: " + highScore);
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreDisplay();

        // Clear final score display if it exists
        if (finalScoreText != null)
        {
            finalScoreText.text = "";
        }

        Debug.Log("Score reset to 0");
    }
}