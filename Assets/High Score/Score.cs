using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] Text bestScoreText;
    [SerializeField]
    int[] scoreWorldLevels = new int[]  //default
        {
            300, 500, 700, 1500 
        };//score needed for each world Level
    Text textScore;
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
        textScore = GetComponent<Text>();
        if (LevelSettingsKeeper.settingsKeeper != null)
            scoreWorldLevels = LevelSettingsKeeper.settingsKeeper.worldInfo.ScoreWorldLevels;
        UpdateScore();
    }

    void UpdateScore()
    {
        if (textScore == null) return;
        textScore.text = currentScore.ToString();
        if (currentScore > bestScore)
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
        worldId = LevelSettingsKeeper.settingsKeeper.worldInfo.id;
        bestScore = GameData.gameData.saveData.worldBestScores[worldId];
        bestScoreText.text = bestScore.ToString();
    }

    public int GetCurrentScore() => currentScore;
}