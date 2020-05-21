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
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] Text bestScoreText;
    [SerializeField] Score score;

    private int currentCounter;
    private float timerSeconds;
    LevelSettingsKeeper settingsKeeper;

    public delegate void MyDelegate();
    public event MyDelegate onMatchedBlock;

    void Start()
    {
        settingsKeeper = FindObjectOfType<LevelSettingsKeeper>();
        SetGameType();
        SetRequirements();
    }

    void SetGameType()
    {
        if (settingsKeeper != null)
        {
            requirements = settingsKeeper.levelTemplate.endGameRequirements;
        }
    }
    public void CallOnMatchDelegate()
    {
        if (onMatchedBlock != null)
            onMatchedBlock();
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
        bestScoreText.text = score.GetCurrentScore() + "";
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
        PlayGamesController.PostToLeaderboard(settingsKeeper.worldIndex);
    }
}
