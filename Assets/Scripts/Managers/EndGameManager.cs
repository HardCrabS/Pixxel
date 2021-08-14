using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EndGameManager : MonoBehaviour
{
    [SerializeField] Text bestScoreText;
    [SerializeField] CanvasGroup visualizerCanvasGroup;
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
        if (gameIsOver) return;//avoid multiple calls of GameOver
        StartCoroutine(GameOverDelayed());
        gameIsOver = true;
    }

    IEnumerator GameOverDelayed()
    {
        AudioController.Instance.StartFade(2, 0);//fade out music
        BonusManager.Instance.SetAllButtonsInterraction(false);
        GridA.Instance.currState = GameState.over;
        yield return new WaitForSeconds(1);//delay to let player realize the game is over
        while (GridA.Instance.AcitivityOnBoard() || BonusManager.Instance.BoostIsActivated())
        {
            yield return new WaitForSeconds(.5f);
        }
        //if boost was activated, disable all boosts again
        BonusManager.Instance.SetAllButtonsInterraction(false);
        GridA.Instance.currState = GameState.over;//set grid state again

        string worldId = LevelSettingsKeeper.settingsKeeper == null ? "Twilight City"
            : LevelSettingsKeeper.settingsKeeper.worldLoadInfo.id;

        PlayGamesController.PostToLeaderboard(worldId);

        LeaderboardController.Instance.SetLeaderboard();
        GridA.Instance.BlocksBlackAndWhite();
        StartCoroutine(GridA.Instance.DeadlockMoveBoxesDown());
        onGameOver.Invoke();
        StartCoroutine(CanvasGroupFadeOut(visualizerCanvasGroup, 1));
        bestScoreText.text = Score.Instance.GetCurrentScore().ToString();
    }
    IEnumerator CanvasGroupFadeOut(CanvasGroup canvasGroup, float duration)
    {
        float value = 0.1f / duration;
        while(canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= value;
            yield return new WaitForSeconds(value);
        }
    }
}