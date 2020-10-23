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
    [SerializeField] bool testing = false;
    public static PlayGamesController Instance;
    public UnityEvent OnAuthenticated;

    void Start()
    {
        if (!GameData.gameData.isAuthentificated)
            AuthenticateUser();
    }

    async void AuthenticateUser()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate(async (bool success) =>
        {
            if (success)
            {
                GameData.gameData.isAuthentificated = true;

                string playerId = PlayGamesPlatform.Instance.localUser.id;
                Task<bool> userTask = DatabaseManager.UserAlreadyInDatabase(playerId);
                bool userInDatabase = await userTask;

                if (!userInDatabase)
                {
                    string playerName = PlayGamesPlatform.Instance.localUser.userName;
                    DatabaseManager.WriteNewUser(playerId, playerName, "Noobe", "Sprites/Avatars/DefaultAvatar", "Sprites/Banners/DefaultBanner");
                    GameData.gameData.saveData.playerInfo = new User(playerId, playerName, "Noobe", "Sprites/Avatars/DefaultAvatar", "Sprites/Banners/DefaultBanner");
                    GameData.gameData.Save();
                }
            }
            else
            {
                Debug.LogError("Unable to sign in Google Play Services");
                if (GameData.gameData.saveData.playerInfo == null)
                {
                    GameData.gameData.saveData.playerInfo = new User("unknown", "Warior", "Noobe", "Sprites/Avatars/DefaultAvatar", "Sprites/UI images/Banners/DefaultBanner");
                    GameData.gameData.Save();
                }
            }
            OnAuthenticated.Invoke();
        }
        );

        if (testing)
        {
            string playerId = "editor12345";
            Task<bool> userTask = DatabaseManager.UserAlreadyInDatabase(playerId);
            bool userInDatabase = await userTask;

            if (!userInDatabase)
            {
                DatabaseManager.WriteNewUser(playerId, "editor Name", "debil", "Sprites/Avatars/DefaultAvatar", "Sprites/UI images/Banners/DefaultBanner");
                GameData.gameData.saveData.playerInfo = new User(playerId, "debil", "Noobe", "Sprites/Avatars/DefaultAvatar", "Sprites/Banners/DefaultBanner");
                GameData.gameData.Save();
                print("Writing test editor user in database");
            }
            OnAuthenticated.Invoke();
        }
    }

    public static bool PostToLeaderboard(string worldId)
    {
        string playerId = PlayGamesPlatform.Instance.localUser.id;
        if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogError("Not authentificated to google, can't upload a score");
            return false;
        }
        string worldName = RewardTemplate.SplitCamelCase(LevelSettingsKeeper.settingsKeeper.worldId);
        int score = GameData.gameData.saveData.worldBestScores[worldId];

        if (!DatabaseManager.ChildExists(playerId, worldName))
        {
            DatabaseManager.WriteNewScore(worldName, playerId, score);
        }
        else
            DatabaseManager.OverwriteTheScore(worldName, playerId, score);

        return true;
    }
}