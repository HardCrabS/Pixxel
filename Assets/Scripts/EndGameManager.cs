using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
    [SerializeField] Text bestScoreText;

    private int currentCounter;
    private float timerSeconds;

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
        if (LevelSettingsKeeper.settingsKeeper == null) return;
        var levelTemplate = LevelSettingsKeeper.settingsKeeper.levelTemplate;
        requirements = levelTemplate.endGameRequirements;
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
        string worldId = LevelSettingsKeeper.settingsKeeper == null ? "TwilightCity"
            : LevelSettingsKeeper.settingsKeeper.worldInfo.GetRewardId();

        PlayGamesController.PostToLeaderboard(worldId);

        DisplayHighscore.Instance.SetLeaderboard();
        StartCoroutine(GameOverDelayed());
    }

    IEnumerator GameOverDelayed()
    {
        yield return new WaitForSeconds(2f);
        bestScoreText.text = Score.Instance.GetCurrentScore() + "";
        RewardForLevel.Instance.CheckForLevelUpReward();
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
}