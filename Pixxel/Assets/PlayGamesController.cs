using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class PlayGamesController : MonoBehaviour
{
    [SerializeField] DisplayHighscore displayHighscore;

    public static PlayGamesController Instance;

    bool authentificated = false;

    void Start()
    {
        AuthenticateUser();
    }

    void AuthenticateUser()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log("Loged in Google Play Services");
                authentificated = true;
            }
            else
            {
                Debug.LogError("Unable to sign in Google Play Services");
            }
        }
        );
    }

    public delegate void OnScoresLoaded(IScore[] scores, IUserProfile[] profiles);
    public OnScoresLoaded OnScoresLoadedDelegate;

    /*public void LoadLeaderboardUsers()
    {
        if (!authentificated)
        {
            Debug.LogError("Couldn't load users info");
            return;
        }
        PlayGamesPlatform.Instance.LoadScores(
            GPGSIds.leaderboard_twilight_city,
            LeaderboardStart.PlayerCentered,
            1,
            LeaderboardCollection.Public,
            LeaderboardTimeSpan.AllTime,
        (LeaderboardScoreData data) =>
        {
            // get scores
            IScore[] scores = data.Scores;
            // get user ids
            string[] userIds = new string[scores.Length];
            for (int i = 0; i < scores.Length; i++)
            {
                userIds[i] = scores[i].userID;
            }
            // forward scores with loaded profiles
            Social.LoadUsers(userIds, profiles => displayHighscore.DisplayLeaderboardEntries(scores, profiles));
        });
    }*/
   /* void LoadScores()
    {
        PlayGamesPlatform.Instance.LoadScores(
            GPGSIds.leaderboard_twilight_city,
            LeaderboardStart.PlayerCentered,
            100,
            LeaderboardCollection.Public,
            LeaderboardTimeSpan.AllTime,
            (data) =>
            {
                mStatus = "Leaderboard data valid: " + data.Valid;
                mStatus += "\n approx:" + data.ApproximateCount + " have " + data.Scores.Length;
            });
    }*/

    public static void PostToLeaderboard(int worldIndex)
    {
        Social.ReportScore((GameData.gameData.saveData.worldsBestScores[worldIndex]),
            GPGSIds.leaderboard_twilight_city, (bool success) =>
        {
            if (success)
            {
                Debug.Log("Posted timee to leaderboard");
            }
            else
            {
                Debug.LogError("Unable to post best score to leaderboard");
            }
        });
    }

    public void ShowLeaderboard()
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_twilight_city);
    }
}