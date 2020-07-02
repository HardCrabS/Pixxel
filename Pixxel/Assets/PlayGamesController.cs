using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Events;

public class PlayGamesController : MonoBehaviour
{
    [SerializeField] bool testing = false; //TODO remove editor testing
    public static PlayGamesController Instance;
    public UnityEvent OnAuthenticated;

    bool authentificated = false;

    void Start()
    {
        AuthenticateUser();
    }

    async void AuthenticateUser()
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

        if (testing)
        {
            string playerId = "editor12345";
            Task<bool> userTask = DatabaseManager.UserAlreadyInDatabase(playerId);
            bool userInDatabase = await userTask;

            if (!userInDatabase)
            {
                DatabaseManager.WriteNewUser(playerId, "editor Name", "debil", "Sprites/Avatars/DefaultAvatar");
                GameData.gameData.saveData.playerInfo = new User(playerId, "debil", "Noobe", "Sprites/Avatars/DefaultAvatar");
                print("Writing test editor user in database");
            }
            OnAuthenticated.Invoke();
        }
        else
        {
            if (authentificated)
            {
                string playerId = PlayGamesPlatform.Instance.localUser.id;
                Task<bool> userTask = DatabaseManager.UserAlreadyInDatabase(playerId);
                bool userInDatabase = await userTask;

                if (!userInDatabase)
                {
                    string playerName = PlayGamesPlatform.Instance.localUser.userName;
                    DatabaseManager.WriteNewUser(playerId, playerName, "Noobe", "Sprites/Avatars/DefaultAvatar");
                    GameData.gameData.saveData.playerInfo = new User(playerId, playerName, "Noobe", "Sprites/Avatars/DefaultAvatar");
                    print("Writing new user in database");
                }
            }
            else
            {
                if(GameData.gameData.saveData.playerInfo == null)
                {
                    GameData.gameData.saveData.playerInfo = new User("unknown", "Warior", "Noobe", "Sprites/Avatars/DefaultAvatar");
                }
            }
            OnAuthenticated.Invoke();
        }
    }

    public static bool PostToLeaderboard(int worldIndex)
    {
        string playerId = PlayGamesPlatform.Instance.localUser.id;
        if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogError("Not authentificated to google, can't upload a score");
            return false;
        }
        string worldName = LevelSettingsKeeper.settingsKeeper.worldName;
        int score = GameData.gameData.saveData.worldsBestScores[worldIndex];

        if (!DatabaseManager.ChildExists(playerId, worldName))
        {
            string playerName = PlayGamesPlatform.Instance.localUser.userName;
            DatabaseManager.WriteNewScore(worldName, playerId, playerName, 0, score);
        }
        else
            DatabaseManager.OverwriteTheScore(worldName, playerId, score);

        return true;
        /*Social.ReportScore((GameData.gameData.saveData.worldsBestScores[worldIndex]),
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
        });*/
    }
}