using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndGameManager : MonoBehaviour
{
    [SerializeField] Text bestScoreText;

    public delegate void MyDelegate();
    public event MyDelegate onMatchedBlock;

    public static EndGameManager Instance;

    void Awake()
    {
        Instance = this;
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
}