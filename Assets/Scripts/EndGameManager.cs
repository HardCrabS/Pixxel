using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EndGameManager : MonoBehaviour
{
    [SerializeField] Text bestScoreText;
    [SerializeField] UnityEvent onGameOver;

    public delegate void MyDelegate();
    public event MyDelegate onMatchedBlock;

    public static EndGameManager Instance;

    bool gameIsOver = false;

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
        print("Game is over!");
        if (gameIsOver) return;
        string worldId = LevelSettingsKeeper.settingsKeeper == null ? "Twilight City"
            : LevelSettingsKeeper.settingsKeeper.worldInfo.id;

        PlayGamesController.PostToLeaderboard(worldId);

        DisplayHighscore.Instance.SetLeaderboard();
        StartCoroutine(GameOverDelayed());
        GridA.Instance.currState = GameState.wait;
        onGameOver.Invoke();
    }

    IEnumerator GameOverDelayed()
    {
        yield return new WaitForSeconds(2f);
        bestScoreText.text = Score.Instance.GetCurrentScore() + "";
        //RewardForLevel.Instance.SetRewardScreenUI();
    }
}