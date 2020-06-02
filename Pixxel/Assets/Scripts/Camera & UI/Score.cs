using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] Text bestScoreText;
    Text textScore;
    GoalManager goalManager;
    int currentScore = 0;

    int bestScore;
    int worldIndex;

    void Awake()
    {
        LoadBestScore();
    }
    void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        textScore = GetComponent<Text>();
        worldIndex = LevelSettingsKeeper.settingsKeeper.worldIndex;
        UpdateScore();
    }

    void UpdateScore()
    {
        textScore.text = currentScore.ToString();
        if (currentScore > bestScore)
        {
            bestScoreText.text = currentScore.ToString();
            GameData.gameData.saveData.worldsBestScores[worldIndex] = currentScore;
        }
    }

    public void AddPoints(int amount)
    {
        currentScore += amount;
        if (goalManager != null)
        {
            goalManager.CompareGoal("Score", amount);
            goalManager.UpdateGoals();
        }
        UpdateScore();
    }

    public void LoadBestScore()
    {
        bestScore = GameData.gameData.saveData.worldsBestScores[worldIndex];
        bestScoreText.text = bestScore.ToString();
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }
}