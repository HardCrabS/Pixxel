using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;

public class PlayGamesController : MonoBehaviour
{
    public static PlayGamesController Instance;
    public UnityEvent OnAuthenticated;

    void Start()
    {
        if (!GameData.gameData.isAuthentificated)
        {
            AuthenticateUser();
        }
        OnAuthenticated.Invoke();
    }

    static async void AuthenticateUser()
    {
        if (ShareController.CheckForInternetConnection())
        {
            string playerId = SystemInfo.deviceUniqueIdentifier;
            Task<bool> userTask = DatabaseManager.UserAlreadyInDatabase(playerId);
            bool userInDatabase = await userTask;

            if (!userInDatabase)
            {
                string playerName = "Soldier";
                DatabaseManager.WriteNewUser(playerId, playerName, "Noobe", "Sprites/Avatars/DefaultAvatar", "Sprites/UI images/Banners/DefaultBanner");
                GameData.gameData.saveData.playerInfo = new User(playerId, playerName, "Noobe", "Sprites/Avatars/DefaultAvatar", "Sprites/UI images/Banners/DefaultBanner");
                GameData.gameData.Save();
            }
            GameData.gameData.isAuthentificated = true;
        }
        else
        {
            if (GameData.gameData.saveData.playerInfo == null)
            {
                GameData.gameData.saveData.playerInfo = new User("unknown", "Warior", "Noobe", "Sprites/Avatars/DefaultAvatar", "Sprites/UI images/Banners/DefaultBanner");
                GameData.gameData.Save();
            }
        }
    }

    public static void PostToLeaderboard(string worldId)
    {
        if (!GameData.gameData.isAuthentificated)
        {
            AuthenticateUser();
        }
        string playerId = SystemInfo.deviceUniqueIdentifier;

        string worldName = RewardTemplate.SplitCamelCase(worldId);
        int score = GameData.gameData.saveData.worldBestScores[worldId];


        if (!DatabaseManager.ChildExists(playerId, worldName))
        {
            DatabaseManager.WriteNewScore(worldName, playerId, score);
        }
        else
            DatabaseManager.OverwriteTheScore(worldName, playerId, score);
    }
}