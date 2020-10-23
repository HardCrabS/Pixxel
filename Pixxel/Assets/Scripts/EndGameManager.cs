using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameType
{
    Time,
    Moves
}

[System.Serializable]
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue;
}

public class EndGameManager : MonoBehaviour
{
    public EndGameRequirements requirements;
    [SerializeField] Text requirementText;
    [SerializeField] Text counterText;
    [SerializeField] GameObject leaderboardGameOverPanel;
    [SerializeField] GameObject trinketGameOverPanel;
    [SerializeField] Text bestScoreText;
    [SerializeField] Text postToLeaderboardText;

    private int currentCounter;
    private float timerSeconds;
    private GameObject gameOverPanel;

    public delegate void MyDelegate();
    public event MyDelegate onMatchedBlock;

    public static EndGameManager Instance;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        SetGameType();
        SetRequirements();
    }

    void SetGameType()
    {
        var levelTemplate = LevelSettingsKeeper.settingsKeeper.levelTemplate;
        requirements = levelTemplate.endGameRequirements;
        gameOverPanel = levelTemplate.isLeaderboard ? leaderboardGameOverPanel : trinketGameOverPanel;
    }
    public void CallOnMatchDelegate()
    {
        StartCoroutine(OnMatchedDelayed());
    }

    IEnumerator OnMatchedDelayed()
    {
        yield return new WaitForSeconds(0.1f);
        if (onMatchedBlock != null)
        {
            onMatchedBlock();
            yield return new WaitForSeconds(0.31f);
            StartCoroutine(GridA.Instance.MoveBoxesDown());
        }
    }
    void SetRequirements()
    {
        currentCounter = requirements.counterValue;
        if (requirements.gameType == GameType.Moves)
        {
            requirementText.text = "Moves left: ";
        }
        else
        {
            timerSeconds = 1;
            requirementText.text = "Time left: ";
        }
        counterText.text = "" + currentCounter;
    }

    public void DecreaseCounterValue()
    {
        currentCounter--;
        counterText.text = "" + currentCounter;
        if (currentCounter <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        StartCoroutine(GameOverDelayed());
    }

    IEnumerator GameOverDelayed()
    {
        yield return new WaitForSeconds(2f);
        bestScoreText.text = Score.Instance.GetCurrentScore() + "";
        RewardForLevel.Instance.CheckForLevelUpReward();
        gameOverPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (requirements.gameType == GameType.Time && currentCounter > 0)
        {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }

    public void PostToLeaderboard()
    {
        bool success = PlayGamesController.PostToLeaderboard(LevelSettingsKeeper.settingsKeeper.worldId);
        if (success)
        {
            postToLeaderboardText.text = "Score uploaded";
        }
        else
            postToLeaderboardText.text = "Failed to upload :(";
    }
}