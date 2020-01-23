using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] Text bestScoreText;
    Text textScore;
    Level level;
    int currentScore = 0;

    void Start()
    {
        level = FindObjectOfType<Level>();
        textScore = GetComponent<Text>();
        UpdateScore();
    }

    void UpdateScore()
    {
        textScore.text = currentScore.ToString();
        if (currentScore > level.GetBestScore())
        {
            bestScoreText.text = currentScore.ToString();
            level.SetBestScore(currentScore);
        }
    }

    public void AddPoints(int amount)
    {
        currentScore += amount;
        UpdateScore();
    }

    public void LoadBestScore(Level level)
    {
        bestScoreText.text = level.GetBestScore().ToString();
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }
}
