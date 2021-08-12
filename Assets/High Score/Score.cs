using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] Text bestScoreText;
    [SerializeField] TextMeshProUGUI bombChanceText;
    [SerializeField]
    int[] scoreWorldLevels = new int[]  //default
        {
            300, 500, 700, 1500
        };//score needed for each world Level
    TextMeshProUGUI textScore;
    int currentScore = 0;
    int currWorldLevel = 0;

    int bestScore;
    string worldId = "Twilight City"; //default value

    public static Score Instance { get; private set; }

    void Awake()
    {
        LoadBestScore();
        Instance = this;
    }
    void Start()
    {
        textScore = GetComponent<TextMeshProUGUI>();
        if (LevelSettingsKeeper.settingsKeeper != null)
            scoreWorldLevels = LevelSettingsKeeper.settingsKeeper.worldLoadInfo.scoreWorldLevels;
        UpdateScore();
        bombChanceText.text = GridA.Instance.bombSpawnChance + "\nlv: " + (currWorldLevel + 1);
    }

    void UpdateScore()
    {
        if (textScore == null) return;
        textScore.text = currentScore.ToString("N0", new CultureInfo("en-us"));

        if (bestScoreText != null && currentScore > bestScore)
        {
            bestScoreText.text = currentScore.ToString();
            GameData.gameData.saveData.worldBestScores[worldId] = currentScore;
        }
        bool maxWorldLevelReached = currWorldLevel == scoreWorldLevels.Length - 1;
        if (!maxWorldLevelReached && currentScore > scoreWorldLevels[currWorldLevel])
        {
            currWorldLevel++;
            GridA.Instance.IncreaseBombSpawnChance(2);
            CoinsDisplay.Instance.IncreaseCoinDropChance(2);
            bombChanceText.text = GridA.Instance.bombSpawnChance + "\nlv: " + (currWorldLevel + 1);
        }
    }

    public void AddPoints(int amount)
    {
        currentScore += amount;
        if (GoalManager.Instance != null)
        {
            GoalManager.Instance.CompareGoal("Score", amount);
        }
        UpdateScore();
    }

    public void LoadBestScore()
    {
        if (LevelSettingsKeeper.settingsKeeper == null) return;
        worldId = LevelSettingsKeeper.settingsKeeper.worldLoadInfo.id;
        bestScore = GameData.gameData.saveData.worldBestScores[worldId];
        if (bestScoreText)
            bestScoreText.text = bestScore.ToString();
    }

    public int GetCurrentScore() => currentScore;
}